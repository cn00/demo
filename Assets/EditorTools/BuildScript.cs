#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using SevenZip;

[ExecuteInEditMode]
public class BuildScript
{
    #region Common
    public const string Tag = "BuildScript";
    static string[] SCENES = FindEnabledEditorScenes();
    static string APP_NAME = "game";
    static string DATETIME = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
    static string TARGET_DIR = "bin/";

    public const string BundleOutDir = "AssetBundle/";

    public static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    public static void GenericBuild(string[] scenes, string target_dir, BuildTargetGroup targetGroup, BuildTarget build_target, BuildOptions build_options)
    {
        // EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, build_target);
        try
        {
            BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        }
        catch (Exception e)
        {
            AppLog.e(Tag, "BuildPlayer failure: " + e);
        }

        ProcessStartInfo pi = new ProcessStartInfo(
#if UNITY_EDITOR_WIN
            "explorer.exe",
            TARGET_DIR.wpath()
#elif UNITY_EDITOR_OSX
            "open",
            TARGET_DIR.upath()
#endif
        );
        pi.WorkingDirectory = ".";
        Process.Start(pi);
    }

    public static List<string> ExcludeExtensions = new List<string>()
    {
        ".tmp",
        ".bak",
        ".unity",
        ".meta",
        ".DS_Store",
    };

    public static string TargetName(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.StandaloneWindows:
                return "Windows";
            case BuildTarget.StandaloneWindows64:
                return "Windows64";
            case BuildTarget.StandaloneOSX:
                return "OSX";
            default:
                return "unknown";
        }
    }

    #region AssetBundle

    public static IEnumerator BuildAssetBundle(BuildTarget targetPlatform, bool rebuild = false, Action callback = null)
    {
        BuildConfig.ClearLogs();
        yield return null;

        var tmp = EditorUserBuildSettings.activeBuildTarget;
        var t = DateTime.Now;
        try
        {
            var outDir = BundleOutDir + TargetName(targetPlatform);
            if (rebuild)
            {
                Directory.Delete(outDir, true);
            }
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            // backup old manifest
            var oldManifestPath = outDir + "/" + TargetName(targetPlatform);
            if (File.Exists(oldManifestPath))
                File.Copy(oldManifestPath, oldManifestPath + ".old", true);

            // copy .lua|.sql to (.lua|.sql).txt
            var userTextPatterns = ".lua|.sql";
            foreach (var pt in userTextPatterns.Split('|'))
            {
                var nn = 0;
                var info = new DirectoryInfo(BuildConfig.BundleResRoot);
                var res = info.GetFiles("*" + pt, SearchOption.AllDirectories)
                    .Where(p => (rebuild || p.LastWriteTimeUtc.ToFileTimeUtc() > BuildConfig.Instance().LastBuildTime));
                foreach (var f in res)
                {
                    EditorUtility.DisplayCancelableProgressBar("copy +" + pt + "+ ...", f.FullName, (float)(++nn) / res.Count());

                    var ftxt = f.FullName.Replace(pt, pt + ".txt");
                    File.Copy(f.FullName, ftxt, true);
                }
            }
            yield return null;

            // this is the right time to update LastBuildTime if i continue edit lua while BuildAssetBundle
            BuildConfig.Instance().LastBuildTime = DateTime.Now.ToFileTimeUtc();
            AssetDatabase.Refresh();
            yield return null;

            var options = (
                BuildAssetBundleOptions.None
              | BuildAssetBundleOptions.CompleteAssets
              | BuildAssetBundleOptions.ChunkBasedCompression
              | BuildAssetBundleOptions.DeterministicAssetBundle
            //   | BuildAssetBundleOptions.AppendHashToAssetBundleName
            );

            if (rebuild)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            var manifest = BuildPipeline.BuildAssetBundles( outDir, options, targetPlatform );
            yield return null;

            // zip
            var outRoot = BundleOutDir + TargetName(targetPlatform)
                + "/" + BuildConfig.Instance().Version;
            AssetBundleManifest oldManifest = null;
            if (File.Exists(oldManifestPath + ".old"))
                oldManifest = AssetBundle.LoadFromFile(oldManifestPath + ".old").LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            var allAssetBundles = manifest.GetAllAssetBundles().ToList();
            allAssetBundles.Add(TargetName(targetPlatform));
            var n = 0;
            foreach (var i in allAssetBundles)
            {
                var finfo = new FileInfo(outDir + "/" + i);
                // size
                var bundleInfo = BuildConfig.Instance().GetBundleInfo(i);
                if (bundleInfo != null)
                {
                    bundleInfo.Size = (ulong)finfo.Length;
                }

                // compare hash
                var hash = manifest.GetAssetBundleHash(i);
                var oldhash = default(Hash128);
                if (oldManifest != null)
                    oldhash = oldManifest.GetAssetBundleHash(i);
                var path = outDir + "/" + i;
                var lzmaPath = path + BuildConfig.CompressedExtension;
                if (hash != oldhash || !File.Exists(lzmaPath))
                {
                    EditorUtility.DisplayCancelableProgressBar("compressing ...", i, (float)(++n) / allAssetBundles.Count);
                    AppLog.d(Tag, "{0} {2} => {1}", i, hash, oldhash);

                    // TODO: encode bundle
                    
                    BundleHelper.CompressFileLZMA(path, lzmaPath);
                }

                // copy
                var outPath = outRoot + "/" + i + BuildConfig.CompressedExtension;
                var dir = Path.GetDirectoryName(outPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Copy(lzmaPath, outPath, true);
                yield return null;
            }
        }
        finally
        {
            // foreach (var f in Directory.GetFiles(BundleConfig.BundleResRoot, "*.lua.txt*", SearchOption.AllDirectories))
            // {
            //     File.Delete(f);
            // }
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
            AppLog.d(Tag, "BuildAssetBundle coast: {0}", DateTime.Now - t);
        }
        yield return null;
        if(callback != null)
            callback();
        yield return null;
    }

    #endregion AssetBundle

    public static void BuildStreamingScene(string scene, BuildTarget targetPlatform)
    {
        var outDir = BundleOutDir + TargetName(targetPlatform);
        string path = outDir + "/"
            + scene.Replace(BuildConfig.BundleResRoot, "")
            .Replace(".unity", BuildConfig.BundlePostfix);
        BuildStreamingScene(new[] { scene }, path, targetPlatform);
    }
    public static void BuildStreamingScene(string[] scenes, string outPath, BuildTarget targetPlatform)
    {
        var dir = Path.GetDirectoryName(outPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        BuildPipeline.BuildPlayer(
            scenes,
            outPath,
            targetPlatform,
            BuildOptions.BuildAdditionalStreamedScenes);

        var lzmaPath = outPath + BuildConfig.CompressedExtension;
        BundleHelper.CompressFileLZMA(outPath, lzmaPath);

        // copy
        var outLzmaPath = lzmaPath.Replace(TargetName(targetPlatform)
            , TargetName(targetPlatform) + "/" + BuildConfig.Instance().Version);
        dir = Path.GetDirectoryName(outLzmaPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        File.Copy(lzmaPath, outLzmaPath, true);
    }

    #endregion Common

    #region 转表
    [MenuItem("Tools/Open Xls Folder")]
    static void OpenExcelDir()
    {
        ProcessStartInfo pi = new ProcessStartInfo(
#if UNITY_EDITOR_WIN
            "explorer.exe",
#elif UNITY_EDITOR_OSX
            "open",
#endif
            "..\\table"
        );
        pi.WorkingDirectory = ".";
        Process.Start(pi);
    }

    [MenuItem("Tools/Open LuaTable Folder")]
    static void OpenLuaDir()
    {
        ProcessStartInfo pi = new ProcessStartInfo(
#if UNITY_EDITOR_WIN
            "explorer.exe",
            "Assets\\BundleRes\\lua\\table"
#elif UNITY_EDITOR_OSX
            "open",
            "Assets/BundleRes/lua/table"
#endif
        );
        pi.WorkingDirectory = ".";
        Process.Start(pi);
    }


    [MenuItem("Tools/Auto Convert Xls to Lua")]
    static void ConvertXlsToLua()
    {
        Console.WriteLine("");
        ProcessStartInfo pi = new ProcessStartInfo(
#if UNITY_EDITOR_WIN
            "Convert.exe",
            "../../table ../../client/Assets/BundleRes/Lua/Table"
#elif UNITY_EDITOR_OSX
            "mono",
            "Convert.exe ../../table ../../client/Assets/BundleRes/lua/table"
#endif
        );
        pi.WorkingDirectory = "../tools/table/";
        Process.Start(pi);

        //LuaHelper.Init();
    }

    [MenuItem("Tools/Open GUI Xls Convert Tool")]
    static void ConvertXlsToLuaGUI()
    {
        Console.WriteLine("");
        ProcessStartInfo pi = new ProcessStartInfo(
            "GameManager.exe"
        );
        pi.WorkingDirectory = "../tools/table";
        Process.Start(pi);
    }
    #endregion 转表

    #region 安卓安装包
    [MenuItem("Build/AndroidApk")]
    public static void BuildAndroidApk()
    {
        var version = BuildConfig.Instance().Version;
        // TODO: open this when release
        // version.Minor += 1;
        version.Patch = 0;
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.Android.bundleVersionCode += 1;
        var versionCode = PlayerSettings.Android.bundleVersionCode;

        string target_dir = TARGET_DIR + APP_NAME + ".apk";
        GenericBuild(SCENES, target_dir, BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }

    #endregion 安卓打包

    #region 🍎安装包

    [MenuItem("Build/iOS (iL2cpp proj)")]
    public static void BuildIosIL2cppProj()
    {
        var version = BuildConfig.Instance().Version;
        //version.Minor += 1;
        version.Patch = 0;
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        string target_dir = Environment.GetEnvironmentVariable("IosProjDir");
        if (string.IsNullOrEmpty(target_dir))
        {
            target_dir = "ios.proj";
        }
        var option = BuildOptions.EnableHeadlessMode
            | BuildOptions.SymlinkLibraries
            //| BuildOptions.Il2CPP
            | BuildOptions.AcceptExternalModificationsToPlayer
            ;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        version.Patch = 0;
        if (Environment.GetEnvironmentVariable("configuration") == "Release")
        {
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        }
        else
        {
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            option |= BuildOptions.AllowDebugging;
        }
        GenericBuild(SCENES, target_dir, BuildTargetGroup.iOS, BuildTarget.iOS, option);
    }
    [MenuItem("Build/iOS (iL2cpp proj sim)")]
    public static void BuildIosIL2cppProjSim()
    {
        var version = BuildConfig.Instance().Version;
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        // PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
        // var versionCode = int.Parse(PlayerSettings.iOS.buildNumber);

        string target_dir = Environment.GetEnvironmentVariable("IosProjDir");
        if (string.IsNullOrEmpty(target_dir))
        {
            target_dir = "ios.proj.sim";
        }

        var option = BuildOptions.EnableHeadlessMode
            | BuildOptions.SymlinkLibraries
            // | BuildOptions.Il2CPP
            // | BuildOptions.Development
            | BuildOptions.AcceptExternalModificationsToPlayer
            | BuildOptions.AllowDebugging
        ;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        version.Patch = 0;
        if (Environment.GetEnvironmentVariable("configuration") == "Release")
        {
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        }
        else
        {
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            option |= BuildOptions.AllowDebugging;
        }
        GenericBuild(SCENES, target_dir, BuildTargetGroup.iOS, BuildTarget.iOS, option);
    }

    [MenuItem("Build/Mac OS X")]
    public static void BuildMacOSX()
    {
        string target_dir = TARGET_DIR + APP_NAME + "-" + "mac.app";
        target_dir.CreateDir();
        GenericBuild(SCENES, target_dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX, BuildOptions.None);
    }


    [MenuItem("Build/iOS/AssetBundle"), ExecuteInEditMode]
    public static void BuildiOSAssetBundle()
    {
        AssetBundleBuildAll(BuildTarget.iOS);
        //StreamingSceneBuild(BuildTarget.Android);

        //// compress
        //Compress(BundleOutDir + TargetName(BuildTarget.Android) + "/" + PlayerSettings.bundleVersion, BuildTarget.Android);

        GenBundleManifest(BuildTarget.iOS);

        // version 
        GenVersionFile(BuildTarget.iOS);

        // TODO: upload to http server
    }


    [MenuItem("Build/iOS/Manifest")]
    public static void BuildiOSManifest()
    {
        GenBundleManifest(BuildTarget.iOS);
        EditorUtility.ClearProgressBar();
    }
    #endregion 🍎打包

    #region Windows

    [MenuItem("Build/Windows")]
    public static void BuildWindows()
    {
        string target_dir = TARGET_DIR + "/" + DATETIME + "/" + APP_NAME + ".exe";
        GenericBuild(SCENES, target_dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    #endregion Windows

    #region 安卓资源包

    [MenuItem("Build/Android/AssetBundle"), ExecuteInEditMode]
    public static void BuildAndroidAssetBundle()
    {
        AssetBundleBuildAll(BuildTarget.Android);
        //StreamingSceneBuild(BuildTarget.Android);

        //// compress
        //Compress(BundleOutDir + TargetName(BuildTarget.Android) + "/" + PlayerSettings.bundleVersion, BuildTarget.Android);

        GenBundleManifest(BuildTarget.Android);

        // version 
        GenVersionFile(BuildTarget.Android);

        // TODO: upload to http server
    }

    public static void GenVersionFile(BuildTarget buildTarget)
    {
        var versionUrl = BundleOutDir + TargetName(buildTarget) + "/resversion.txt";
        StreamWriter writer = new StreamWriter(versionUrl);
        var version = BuildConfig.Instance().Version;
        writer.Write(version.ToString());
        writer.Close();
        File.Copy(versionUrl, BundleOutDir + TargetName(buildTarget) + "/" + version + "/resversion.txt", true);
    }

    [MenuItem("Build/Android/Manifest")]
    public static void BuildAndroidManifest()
    {
        GenBundleManifest(BuildTarget.Android);
        EditorUtility.ClearProgressBar();
    }

    // TODO: replace to Build BundleConfig
    public static void GenBundleManifest(BuildTarget buildTarget)
    {
        try
        {
            var version = BuildConfig.Instance().Version;
            var rootDir = BundleOutDir + TargetName(buildTarget) + "/";
            var sourceDir = rootDir + version.ToString() + "/";
            if (!Directory.Exists(sourceDir))
            {
                Directory.CreateDirectory(sourceDir);
            }

            var manifestbf = sourceDir + BuildConfig.ManifestName + BuildConfig.CompressedExtension;
            if (File.Exists(manifestbf))
            {
                File.Delete(manifestbf);
            }

            var files = Directory.GetFiles(sourceDir, "*" + BuildConfig.CompressedExtension, SearchOption.AllDirectories);
            float i = 0;
            foreach (var f in files)
            {
                ++i;
                var bf = f.upath().Replace(version.ToString() + "/", string.Empty).Replace(BuildConfig.CompressedExtension, string.Empty);
                var md5 = BundleHelper.Md5(bf);
                var subPath = bf.Replace(rootDir, string.Empty);
                var bundleInfo = BuildConfig.Instance().GetBundleInfo(subPath);
                if (bundleInfo != null)
                {
                    bundleInfo.Md5 = md5;
                    bundleInfo.Version = version.ToString();
                }
                else
                {
                    bundleInfo = new BuildConfig.BundleInfo()
                    {
                        Name = subPath,
                        Md5 = md5,
                        Version = version.ToString(),
                    };
                }
            }
            BuildConfig.Instance().Save();

            // generate md5 sheet
            var manifestPath = rootDir + BuildConfig.ManifestName;
            if (File.Exists(manifestPath))
            {
                File.Copy(manifestPath, manifestPath + ".bak", true);
            }
            YamlHelper.Serialize(BuildConfig.Instance().AllBundles, manifestPath);
            BundleHelper.CompressFileLZMA(manifestPath, manifestbf);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static void AssetBundleBuildAll(BuildTarget buildTarget)
    {
        try
        {
            var version = BuildConfig.Instance().Version;
            // TODO: open this when release
            // version.Patch += 1;
            PlayerSettings.bundleVersion = version.ToString();
            //AndroidAssetBundleDelete();
            EditorCoroutine.StartCoroutine(BuildAssetBundle(buildTarget, true));
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    #region AndroidStreamingScene
    [MenuItem("Build/Android/StreamingScene"), ExecuteInEditMode]
    public static void BuildAndroidStreamingScene()
    {
        BuildStreamingScene(BuildTarget.Android);
    }

    public static void BuildStreamingScene(BuildTarget buildTarget)
    {
        try
        {
            float count = 0;
            var files = Directory.GetFiles(BuildConfig.BundleResRoot, "*.unity", SearchOption.AllDirectories);
            foreach (var i in files)
            {
                var f = i.upath();
                AppLog.d(Tag, f);
                EditorUtility.DisplayCancelableProgressBar("StreamingScene ...", f, count / files.Length);
                BuildStreamingScene(f, buildTarget);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
    #endregion

    #endregion 安卓资源包
}
#endif
