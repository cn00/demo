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
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class BuildScript
{
    #region Common
    public const string Tag = "BuildScript";
    static string[] SCENES = FindEnabledEditorScenes();
    static string APP_NAME = "game";
    static string DATETIME = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
    static string TARGET_DIR = "bin/";

    public const string BundleOutDir = "ab/";

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
            var res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
            UnityEngine.Debug.Log($"{res.summary}");
        }
        catch (Exception e)
        {
            Debug.LogError(Tag+"BuildPlayer failure: " + e);
        }
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
            if (rebuild && Directory.Exists(outDir))
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

            BuildConfig.Instance().RefreshGroups();
            yield return null;

            AssetDatabase.Refresh();
            yield return null;
            AssetDatabase.SaveAssets();
            yield return null;

            // this is the right time to update LastBuildTime if i continue edit lua while BuildAssetBundle
            BuildConfig.Instance().LastBuildTime = DateTime.Now.ToFileTimeUtc();

            var options = (BuildAssetBundleOptions)BuildConfig.Instance().BundleBuildOptions;

            if (rebuild)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            var manifest = BuildPipeline.BuildAssetBundles( outDir, options, targetPlatform );
            yield return null;

            // zip
            var version = BuildConfig.Instance().Version.ToString();
            var outRoot = BundleOutDir + TargetName(targetPlatform)
                + "/" + version;
            AssetBundleManifest oldManifest = null;
            if (File.Exists(oldManifestPath + ".old"))
                oldManifest = AssetBundle.LoadFromFile(oldManifestPath + ".old").LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            var allAssetBundles = manifest.GetAllAssetBundles().ToList();
            allAssetBundles.Add(TargetName(targetPlatform));

            // collect news
            var n = 0;
            foreach (var i in allAssetBundles)
            {
                var finfo = new FileInfo(outDir + "/" + i);
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
                    Debug.LogFormat(Tag+"{0} {2} => {1}", i, hash, oldhash);

                    // TODO: encode bundle

                    BundleHelper.CompressFileLZMA(path, lzmaPath);

                    var bundleInfo = BuildConfig.Instance().GetBundleInfo(i);
                    if (bundleInfo != null)
                    {
                        bundleInfo.Size = (ulong)finfo.Length;
                        bundleInfo.Hash = hash.ToString();
                        bundleInfo.Version = version;
                    }
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
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
            Debug.LogFormat(Tag+"BuildAssetBundle coast: {0}", DateTime.Now - t);
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

    public static void GenVersionFile(BuildTarget buildTarget)
    {
        var versionUrl = BundleOutDir + TargetName(buildTarget) + "/resversion.txt";
        StreamWriter writer = new StreamWriter(versionUrl);
        var version = BuildConfig.Instance().Version;
        writer.Write(version.ToString());
        writer.Close();
        File.Copy(versionUrl, BundleOutDir + TargetName(buildTarget) + "/" + version + "/resversion.txt", true);
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

            // generate md5 sheet
            var manifestPath = rootDir + BuildConfig.ManifestName;
            if (File.Exists(manifestPath))
            {
                File.Copy(manifestPath, manifestPath + ".bak", true);
            }

            // YamlHelper.Serialize(BuildConfig.Instance().AllBundles, manifestPath);
            var manifest = AppBundleManifest.Instance();
            manifest.BundleInfos = BuildConfig.Instance().AllBundles;
            manifest.Save();

            // BundleHelper.CompressFileLZMA(manifestPath, manifestbf);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
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
}
#endif