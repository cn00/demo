#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using SevenZip;

using BundleManifest = System.Collections.Generic.List<BundleConfig.BundleInfo>;

[ExecuteInEditMode]
public class BuildScript
{
    #region Common

    static string[] SCENES = FindEnabledEditorScenes();
    static string APP_NAME = "game";
    static string DATETIME = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
    static string TARGET_DIR = "bin/";

    const string BundleOutDir = "AssetBundle/";

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if(!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTargetGroup targetGroup, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, build_target);
        string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        if(res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
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
    };

    static string TargetName(BuildTarget target)
    {
        switch(target)
        {
        case BuildTarget.Android:
            return "Android";
        case BuildTarget.iOS:
            return "iOS";
        case BuildTarget.StandaloneWindows:
        case BuildTarget.StandaloneWindows64:
            return "Windows";
        case BuildTarget.StandaloneOSXIntel:
        case BuildTarget.StandaloneOSXIntel64:
        case BuildTarget.StandaloneOSXUniversal:
            return "OSX";
        default:
            return null;
        }
    }

    #region AssetBundle
    public static void BuildBundleGroup(string indir, BuildTarget targetPlatform, bool rebuild = false)
    {
        try
        {
            // lua
            foreach(var f in Directory.GetFiles(indir, "*.lua", SearchOption.AllDirectories))
            {
                var ftxt = f.Replace(".lua", ".lua.txt");
                File.Copy(f, ftxt);
                AssetDatabase.ImportAsset(ftxt);
            }

            // Create the array of bundle build details.
            var buildMap = new List<AssetBundleBuild>();
            float count = 0;
            var dirs = Directory.GetDirectories(indir, "*", SearchOption.TopDirectoryOnly);
            foreach(var dir in dirs)
            {
                ++count;
                var udir = dir.upath();
                var assetBundleName = udir.Substring(udir.LastIndexOf('/')+1);
                AppLog.d("pack: " + assetBundleName);
                var ab = CreateAssetBundleBuild(udir, assetBundleName, ExcludeExtensions, rebuild);
                if(ab != null)
                    buildMap.Add(ab.Value);
                EditorUtility.DisplayCancelableProgressBar("BuildBundle ...", udir, count / dirs.Length);
            }
            if(buildMap.Count == 0)
                return;

            var outdir = BundleOutDir + TargetName(targetPlatform)
                //+ "/" + BundleConfig.Instance().Version 
                + "/" + indir.upath().Replace(BundleConfig.BundleResRoot, "");
            if(!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);
            }
            BuildPipeline.BuildAssetBundles(
                outdir,
                buildMap.ToArray(),
                (
                BuildAssetBundleOptions.None
                //| BuildAssetBundleOptions.UncompressedAssetBundle
                | BuildAssetBundleOptions.ChunkBasedCompression
                ),
                targetPlatform
            );

            Compress(outdir, targetPlatform);
        }
        finally
        {
            foreach(var f in Directory.GetFiles(indir, "*.lua.txt*", SearchOption.AllDirectories))
            {
                File.Delete(f);
            }
            EditorUtility.ClearProgressBar();
        }
    }

    static AssetBundleBuild? CreateAssetBundleBuild(string assetDir, string assetBundleName, List<string> excludes, bool rebuild = false)
    {
        var ab = new AssetBundleBuild();
        ab.assetBundleName = assetBundleName + BundleConfig.BundlePostfix;

        var bundleInfo = BundleConfig.Instance().GetBundleInfo(assetDir.Replace(BundleConfig.BundleResRoot, "") + BundleConfig.BundlePostfix);
        long lastBuildTime = long.Parse(bundleInfo.BuildTime);

        var assetNames = new List<string>();
        int nnew = 0;
        foreach(var f in Directory.GetFiles(assetDir, "*.*", SearchOption.AllDirectories))
        {
            if(excludes.Contains(Path.GetExtension(f)))
                continue;
            assetNames.Add(f);

            var finfo = new FileInfo(f);
            if(rebuild || finfo.LastWriteTime.ToFileTime() > lastBuildTime)
            {
                ++nnew;
                 //AppLog.d(f.upath() + ": " + DateTime.FromFileTime(modifyTime));
            }
        }

        if(nnew > 0 && assetNames.Count() > 0)
        {
            ab.assetNames = assetNames.ToArray();
            bundleInfo.BuildTime = DateTime.Now.ToFileTime().ToString();
            bundleInfo.Version = BundleConfig.Instance().Version.ToString();
            AppLog.d(assetDir + " > " + DateTime.FromFileTime(long.Parse(bundleInfo.BuildTime)));
            return ab;
        }
        else
        {
            return null;
        }
    }

    #endregion AssetBundle

    public static void StreamingSceneBuild(string scene, string path, BuildTarget targetPlatform)
    {
        StreamingSceneBuild(new[] { scene }, path, targetPlatform);
    }
    public static void StreamingSceneBuild(string[] scenes, string outName, BuildTarget targetPlatform)
    {
        string SceneOutPath = BundleOutDir + TargetName(targetPlatform) + "/" + BundleConfig.Instance().Version + "/Level/" + outName + ".fg";

        var dir = Path.GetDirectoryName(SceneOutPath);
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        BuildPipeline.BuildPlayer(
            scenes,
            SceneOutPath,
            targetPlatform,
            BuildOptions.BuildAdditionalStreamedScenes);
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
    static void BuildAndroidApk()
    {
        var version = BundleConfig.Instance().Version;
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
    static void BuildIosIL2cppProj()
    {
        var version = BundleConfig.Instance().Version;
        //version.Minor += 1;
        version.Patch = 0;
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        string target_dir = Environment.GetEnvironmentVariable("IosProjDir");
        if (string.IsNullOrEmpty(target_dir))
        {
            target_dir = "ios.proj";
        }
        var option = BuildOptions.EnableHeadlessMode | BuildOptions.SymlinkLibraries | BuildOptions.Il2CPP;
        version.Patch = 0;
        if(Environment.GetEnvironmentVariable("configuration") == "Release")
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
    static void BuildIosIL2cppProjSim()
    {
        var version = BundleConfig.Instance().Version;
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
//        PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
        var versionCode = int.Parse(PlayerSettings.iOS.buildNumber);

        string target_dir = Environment.GetEnvironmentVariable("IosProjDir");
        if(string.IsNullOrEmpty(target_dir))
        {
            target_dir = "ios.proj.sim";
        }

        var option = BuildOptions.EnableHeadlessMode 
            | BuildOptions.SymlinkLibraries 
            | BuildOptions.Il2CPP
            | BuildOptions.AcceptExternalModificationsToPlayer
        ;
        version.Patch = 0;
        if(Environment.GetEnvironmentVariable("configuration") == "Release")
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
    static void BuildMacOSX()
    {
        string target_dir = TARGET_DIR + "/" + APP_NAME + "-" + DATETIME + ".app";
        GenericBuild(SCENES, target_dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
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
    static void BuildWindows()
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

    public static void Compress(string indir, BuildTarget targetPlatform)
    {
        try
        {
            var outRoot = BundleOutDir + TargetName(targetPlatform)
                  + "/" + BundleConfig.Instance().Version;

            // compress
            var files = (from f in Directory.GetFiles(indir, "*", SearchOption.AllDirectories)
                         where Path.GetExtension(f) != ".manifest"
                         && Path.GetExtension(f) != BundleConfig.CompressedExtension
                         || Path.GetExtension(f) == ""
                         select f.upath()).ToArray();
            int i = 0;
            foreach(var f in files)
            {
                ++i;
                EditorUtility.DisplayCancelableProgressBar("compressing ...", f, (float)(i) / files.Length);

                BundleHelper.CompressFileLZMA(f, f.Replace(BundleOutDir + TargetName(targetPlatform), outRoot) + BundleConfig.CompressedExtension);
                //File.Delete(f);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static void GenVersionFile(BuildTarget buildTarget)
    {
        var versionUrl = BundleOutDir + TargetName(buildTarget) + "/resversion.txt";
        StreamWriter writer = new StreamWriter(versionUrl);
        var version = BundleConfig.Instance().Version;
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

    public static void GenBundleManifest(BuildTarget buildTarget)
    {
        try
        {
            var version = BundleConfig.Instance().Version;
            var rootDir = BundleOutDir + TargetName(buildTarget) + "/";
            var sourceDir = rootDir + version.ToString() + "/";

            var manifestbf = sourceDir + BundleConfig.ManifestName + BundleConfig.CompressedExtension;
            if(File.Exists(manifestbf))
            {
                File.Delete(manifestbf);
            }

            var manifest = new BundleManifest();//<string/*path*/, BundleInfo>
            var files = Directory.GetFiles(sourceDir, "*" + BundleConfig.CompressedExtension, SearchOption.AllDirectories);
            float i = 0;
            foreach(var f in files)
            {
                ++i;
                var bf = f.upath().Replace(version.ToString() + "/", string.Empty).Replace(BundleConfig.CompressedExtension, string.Empty);
                var md5 = BundleHelper.Md5(bf);
                var subPath = bf.Replace(rootDir, string.Empty);
                var bundleInfo = BundleConfig.Instance().GetBundleInfo(subPath);
                if(bundleInfo != null)
                {
                    bundleInfo.Md5 = md5;
                }
                else
                {
                    bundleInfo = new BundleConfig.BundleInfo()
                    {
                        Name = subPath,
                        Md5 = md5,
                        Version = version.ToString(),
                    };
                }
                manifest.Add(bundleInfo);
            }
            BundleConfig.Instance().Save();

            // generate md5 sheet
            var manifestPath = rootDir + BundleConfig.ManifestName;
            if(File.Exists(manifestPath))
            {
                File.Copy(manifestPath, manifestPath + ".bak", true);
            }
            YamlHelper.Serialize(manifest, manifestPath);
            BundleHelper.CompressFileLZMA(manifestPath, manifestbf);

            //var Groups = BundleOutDir + TargetName(buildTarget) + "/" + version.ToString() + "/" + "Groups.yaml";
            //YamlHelper.Serialize(BundleConfig.Instance().Groups, Groups);
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
            var version = BundleConfig.Instance().Version;
            // TODO: open this when release
            // version.Patch += 1;
            PlayerSettings.bundleVersion = version.ToString();
            //AndroidAssetBundleDelete();
            foreach(var i in BundleConfig.Instance().ABResGroups)
            {
                BuildBundleGroup(BundleConfig.BundleResRoot + i, buildTarget);
            }
            BundleConfig.Instance().Save();

            //var rootDir = BundleOutDir + TargetName(buildTarget) + "/" + version.ToString() + "/" + "Groups.yaml";
            //YamlHelper.Serialize(BundleConfig.Instance().Groups, rootDir);
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
        StreamingSceneBuild(BuildTarget.Android);
    }

    public static void StreamingSceneBuild(BuildTarget buildTarget)
    {
        try
        {
            foreach(var StreamSceneDir in new string[]{ "Scene" } )
            {
                float count = 0;
                var files = Directory.GetFiles(BundleConfig.BundleResRoot + StreamSceneDir, "*.unity", SearchOption.AllDirectories);
                foreach(var i in files)
                {
                    var f = i.upath();
                    AppLog.d(f);
                    EditorUtility.DisplayCancelableProgressBar("StreamingScene ...", f, count / files.Length);
                    StreamingSceneBuild(
                        new[] { f },
                        Path.GetFileNameWithoutExtension(f),
                        buildTarget);
                }
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
