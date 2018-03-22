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

using Md5SchemeDic = System.Collections.Generic.Dictionary<string, Md5SchemeInfo>;
using System.Collections;

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
    public static void BuildBundle(string indir, string outdir, BuildTarget targetPlatfrom, bool rebuild = false)
    {
        try
        {
            outdir = BundleOutDir + TargetName(BuildTarget.Android) + "/" + PlayerSettings.bundleVersion + "/" + outdir;
            if(!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);
            }

            // lua
            foreach(var f in Directory.GetFiles(indir, "*.lua*", SearchOption.AllDirectories))
            {
                File.Move(f, f.Replace(".lua", ".lua.txt"));
            }
            AssetDatabase.Refresh();

            // Create the array of bundle build details.
            var buildMap = new List<AssetBundleBuild>();
            float count = 0;
            var dirs = Directory.GetDirectories(indir, "*", SearchOption.TopDirectoryOnly);
            foreach(var dir in dirs)
            {

                ++count;
                var udir = dir.upath();
                var assetBundleName = udir.Substring(udir.LastIndexOf("/") + 1); //.Replace(BundleConfig.ABResourceDir + "/", "");//
                UnityEngine.Debug.Log("pack: " + assetBundleName);
                var ab = CreateAssetBundleBuild(udir, assetBundleName, ExcludeExtensions);
                if(ab != null)
                    buildMap.Add(ab.Value);
                EditorUtility.DisplayCancelableProgressBar("BuildBundle ...", udir, count / dirs.Length);
                BuildPipeline.BuildAssetBundles(
                    outdir,
                    buildMap.ToArray(),
                    (
                        BuildAssetBundleOptions.UncompressedAssetBundle
                      //| BuildAssetBundleOptions.DeterministicAssetBundle
                    ),
                    targetPlatfrom);
            }

            foreach(var f in Directory.GetFiles(indir, "*.lua.txt*", SearchOption.AllDirectories))
            {
                File.Move(f, f.Replace(".lua.txt", ".lua"));
            }
            AssetDatabase.Refresh();

        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static AssetBundleBuild? CreateAssetBundleBuild(string assetDir, string assetBundleName, List<string> excludes, bool rebuild = false)
    {
        var ab = new AssetBundleBuild();
        ab.assetBundleName = assetBundleName + BundleConfig.BundlePostfix;

        // 如果上次打包以来为更新过此 bundle 未编辑过则跳过
        long newWriteTime = 0;
        long lastWriteTime = 0;
        var flastWriteTime = assetDir + @"/.lastWriteTime";
        if(File.Exists(flastWriteTime))
        {
            StreamReader reader = new StreamReader(flastWriteTime);
            if(reader != null)
            {
                string s = reader.ReadLine();
                reader.Close();
                if(!long.TryParse(s, out lastWriteTime))
                {
                    lastWriteTime = 0;
                }
                newWriteTime = lastWriteTime;
            }
        }

        var assetNames = new List<string>();
        int nnew = 1; // =0
        foreach(var f in Directory.GetFiles(assetDir, "*.*", SearchOption.AllDirectories))
        {
            if(excludes.Contains(Path.GetExtension(f))
                || Path.GetFileName(f) == ".lastWriteTime")
                continue;
            assetNames.Add(f);

            var finfo = new FileInfo(f);
            if(rebuild || finfo.LastWriteTime.ToFileTimeUtc() > newWriteTime)
            {
                ++nnew;
                newWriteTime = finfo.LastWriteTime.ToFileTimeUtc();
                UnityEngine.Debug.Log(f.upath() + ": " + DateTime.FromFileTimeUtc(newWriteTime));
            }
        }
        ab.assetNames = assetNames.ToArray();

        if(nnew > 0)
        {
            StreamWriter writer = new StreamWriter(flastWriteTime);
            writer.Write(newWriteTime);
            writer.Close();
            UnityEngine.Debug.Log(assetDir + "> " + DateTime.FromFileTimeUtc(newWriteTime));

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
        string SceneOutPath = BundleOutDir + TargetName(targetPlatform) + "/" + PlayerSettings.bundleVersion + "/Level/" + outName + ".fg";

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
            "Assets\\BundleRes\\Lua\\Table"
#elif UNITY_EDITOR_OSX
            "open",
            BundleConfig.ABResourceDir + "/Lua/Table"
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
            "Convert.exe ../../table ../../client/Assets/BundleRes/Lua/Table"
#endif
        );
        pi.WorkingDirectory = "..\\tools\\table\\";
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
        pi.WorkingDirectory = "..\\tools\\table\\";
        Process.Start(pi);
    }
    #endregion 转表

    #region 安卓安装包
    [MenuItem("Build/Android_APK")]
    static void BuildAndroidApk()
    {
        var version = new FGVersion(PlayerSettings.bundleVersion);
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
    static void ExportIOSProj()
    {
        var version = new FGVersion(PlayerSettings.bundleVersion);
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
    [MenuItem("Build/iOS Sim (iL2cpp proj)")]
    static void ExportIOSProjSim()
    {
        var version = new FGVersion(PlayerSettings.bundleVersion);
        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
//        PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
        var versionCode = int.Parse(PlayerSettings.iOS.buildNumber);

        string target_dir = Environment.GetEnvironmentVariable("IosProjDir");
        if(string.IsNullOrEmpty(target_dir))
        {
            target_dir = "ios.proj.sim";
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

    [MenuItem("Build/Mac OS X")]
    static void MacOSX()
    {
        string target_dir = TARGET_DIR + "/" + APP_NAME + "-" + DATETIME + ".app";
        GenericBuild(SCENES, target_dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
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

    [MenuItem("Build/Android/AssetBundleBuild"), ExecuteInEditMode]
    public static void AndroidAssetBundleBuild()
    {
        AssetBundleBuildAll(BuildTarget.Android);
        StreamingSceneBuild(BuildTarget.Android);

        // compress
        Compress(BuildTarget.Android);

        GenMd5List(BuildTarget.Android);

        // version 
        GenVersionFile(BuildTarget.Android);

        // TODO: upload to http server
    }

    public static void Compress(BuildTarget buildTarget)
    {
        try
        {
            var version = (PlayerSettings.bundleVersion);
            // compress
            var files = (from f in Directory.GetFiles(BundleOutDir + TargetName(buildTarget) + "/" + version.ToString(), "*", SearchOption.AllDirectories)
                         where Path.GetExtension(f) != ".manifest" && Path.GetExtension(f) != BundleConfig.CompressedExtension
                         || Path.GetExtension(f) == ""
                         select f).ToArray();
            int i = 0;
            foreach(var f in files)
            {
                ++i;
                EditorUtility.DisplayCancelableProgressBar("compressing ...", (f.upath()), (float)(i) / files.Length);

                BundleHelper.CompressFileLZMA(f, f + BundleConfig.CompressedExtension);
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
        var version = new FGVersion(PlayerSettings.bundleVersion);
        writer.Write(version.ToString());
        writer.Close();
        File.Copy(versionUrl, BundleOutDir + TargetName(buildTarget) + "/" + PlayerSettings.bundleVersion + "/resversion.txt", true);
    }

    [MenuItem("Build/Android/GenMd5List"), ExecuteInEditMode]
    public static void AndroidGenMd5List()
    {
        GenMd5List(BuildTarget.Android);
        EditorUtility.ClearProgressBar();
    }

    public static void GenMd5List(BuildTarget buildTarget)
    {
        try
        {
            var version = (PlayerSettings.bundleVersion);
            var rootDir = BundleOutDir + TargetName(buildTarget) + "/" + version.ToString() + "/";
            Md5SchemeDic md5List = new Md5SchemeDic();//<string/*path*/, Md5SchemeInfo>
            var files = (from f in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories)
                         where Path.GetExtension(f) == BundleConfig.CompressedExtension
                         select f).ToArray();
            float i = 0;
            foreach(var f in files)
            {
                ++i;
                var md5 = BundleHelper.Md5(f);
                var subPath = f.upath().Replace(rootDir, "").Replace(BundleConfig.CompressedExtension, "");
                md5List[subPath] = (new Md5SchemeInfo(subPath, md5, subPath.Contains("PreDownload")));
            }

            // generate md5 sheet
            var md5Path = rootDir + "md5.yaml";
            YamlHelper.Serialize(md5List, md5Path);
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
            var version = new FGVersion(PlayerSettings.bundleVersion);
            // TODO: open this when release
            // version.Patch += 1;
            PlayerSettings.bundleVersion = version.ToString();
            //AndroidAssetBundleDelete();
            foreach(var i in BundleConfig.Instance().ABResGroups)
            {
                BuildBundle(BundleConfig.BundleResRoot + i, i, buildTarget);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    #region AndroidStreamingScene
    [MenuItem("Build/Android/StreamingSceneBuild"), ExecuteInEditMode]
    public static void AndroidStreamingSceneBuild()
    {
        StreamingSceneBuild(BuildTarget.Android);
    }

    public static void StreamingSceneBuild(BuildTarget buildTarget)
    {
        try
        {
            foreach(var StreamSceneDir in BundleConfig.Instance().ABSceneRoots)
            {
                float count = 0;
                var files = Directory.GetFiles(BundleConfig.BundleResRoot + StreamSceneDir, "*_terrain.unity", SearchOption.AllDirectories);
                foreach(var i in files)
                {
                    var f = i.upath();
                    UnityEngine.Debug.Log(f);
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