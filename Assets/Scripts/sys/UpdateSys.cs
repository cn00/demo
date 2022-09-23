﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using XLua;
using System.Text;
using UnityEngine.Networking;
using BundleManifest = System.Collections.Generic.List<BundleInfo>;
using Version = System.Version;

public static class BytesExtension
{
    public static string Utf8String(this byte[] self)
    {
        return Encoding.UTF8.GetString(self);
    }
}

public class UpdateSys : SingleMono<UpdateSys>
{
    const string Tag = "UpdateSys";
    Version mLocalVersion;
    Version mRemoteVersion;

    BundleManifest      mLocalManifest ;
    AssetBundleManifest mRemoteManifest;

    [SerializeField]
    List<string> mDiffList = new List<string>();// <string/*path*/, Md5SchemeInfo>

    bool mAllDownloadOK = false;

    public IEnumerator GetLocalVersion()
    {
        var cachePath = AssetSys.CacheRoot + "resversion.txt";
        var localVersionUrl = cachePath;
        UnityEngine.Debug.LogFormat(Tag+": GetLocalVersion: {0}", localVersionUrl);

        if(File.Exists(cachePath))
        {
            mLocalVersion = new Version(File.ReadAllText(cachePath));
        }
        else
        {
            mLocalVersion = new Version("1.0.0");
            File.WriteAllText(cachePath, "1.0.0");
        }

        UnityEngine.Debug.LogFormat(Tag+": LocalVersion {0}", mLocalVersion.ToString());

        yield return null;
    }

    public IEnumerator GetRemoteVersion()
    {
        var remoteVersionUrl = AssetSys.WebRoot + AssetSys.PlatformName() + "/" + "resversion.txt";

        var webRequest = UnityWebRequest.Get(remoteVersionUrl);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        mRemoteVersion = new Version(DownloadHandlerBuffer.GetContent(webRequest));
        Debug.LogFormat(Tag+": RemoteVersion {0}", mRemoteVersion.ToString());
        yield return null;
    }

    IEnumerator GetLocalManifest()
    {
        if (File.Exists(LocalManifestPath))
        {
            mLocalManifest = YamlHelper.Deserialize<BundleManifest>(File.ReadAllText(LocalManifestPath));
        }
        else
        {
            mLocalManifest = new BundleManifest();
        }

        yield return null;
    }

    public IEnumerator GetRemoteManifest()
    {
        var remoteManifestUrl = AssetSys.WebRoot + AssetSys.PlatformName() + "/" + AssetSys.PlatformName();

        var old = AssetBundle.GetAllLoadedAssetBundles().ToList().Find(i => i.name == ""); //AssetBundleManifest);
        if(old)
        {
            Debug.LogFormat($"{Tag} unload AssetBundleManifest {old.name}:{old}");
            old.Unload(true);
        }


        var temp = AssetSys.CacheRoot + AssetSys.PlatformName() + "/" + AssetSys.PlatformName()+".remote";
        if(File.Exists(temp))
            File.Delete(temp);

        var webRequest = UnityWebRequest.Get(remoteManifestUrl);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        var s = DownloadHandlerBuffer.GetContent(webRequest);
        var ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(s));
        mRemoteManifest = AssetBundle.LoadFromStream(ms).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        File.Delete(temp);

    }

    private static string CompressedExtension = "";
    /// <summary>
    /// 下载差异资源
    /// </summary>
    public IEnumerator DownloadDiffFiles()
    {
        int count = 0;
        for(var idx = 0; idx < mDiffList.Count; ++idx)
        {
            var i = mDiffList[idx];
            var subPath = i.Replace("assets/appres/", "");
            var cachePath = AssetSys.CacheRoot + subPath;

            var dir = Path.GetDirectoryName(cachePath);
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            yield return AssetSys.Download(subPath, subPath, fs =>
            {
                Updated(subPath);
                ++count;
                if (count == mDiffList.Count)
                    mAllDownloadOK = true;
            });
        }
    }

    /// <summary>
    /// 需要更新的资源
    /// </summary>
    public void Diff()
    {

         foreach(var r in mRemoteManifest.GetAllAssetBundles())
         {
             var cachePath = AssetSys.CacheRoot + r;
            var rh = mRemoteManifest.GetAssetBundleHash(r);
            var l = mLocalManifest.Find(i => i.Name == r);
            if(!File.Exists(cachePath) || l == null || l.Hash != rh.ToString() )
            {
                UnityEngine.Debug.LogFormat(Tag+": diff: {0}:[{1} {2}]", r, rh, (l != null ? l.Hash : ""));
                mDiffList.Add(r);
            }
        }
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    public IEnumerator CheckUpdate()
    {
        LocalManifestPath = AssetSys.CacheRoot + "manifest.yaml";
        #if UNITY_EDITOR
        var UseBundle = BuildConfig.Instance().UseBundle;
        if (!UseBundle) yield break;
        #endif

        // var isOK = false;
        // 是否需要更新
        yield return GetLocalVersion();
        yield return GetRemoteVersion();
        //if(LocalVersion != RemoteVersion)// 允许回档到历史版本?
        {
            // download md5 list & uncompress & clean zip
            yield return GetLocalManifest();
            yield return GetRemoteManifest();

            Diff();

            // yield return DownloadDiffFiles();


            // 更新完成后保存
            var cacheUrl = AssetSys.CacheRoot + "resversion.txt";
            var strRemoteVersion = mRemoteVersion.ToString();
            // File.WriteAllText(cacheUrl, strRemoteVersion);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(strRemoteVersion);
            AssetSys.AsyncSave(cacheUrl, bytes);
        }

        yield return null;
    }

    public bool NeedUpdate(string subPath)
    {
        var info = mDiffList.Find(i=>i == subPath);
        return info != null;
    }

    private string LocalManifestPath;
    /// <summary>
    /// 将更新过的md5更新到本地
    /// </summary>
    public void Updated(string subPath)
    {
        // Debug.Log(Tag+": Updated: {0}", subPath);
        //lock(mDiffListLock)
        {

            var newi = mDiffList.Find(i => i == subPath);
            var hash = mRemoteManifest.GetAssetBundleHash(newi).ToString();
            var binfo = mLocalManifest.Find(i => i.Name == subPath);
            if (binfo == null)
            {
                binfo = new BundleInfo()
                {
                    Name = subPath,
                    Hash = hash
                };
                mLocalManifest.Add(binfo);
            }
            if(newi != null)
            {
                UnityEngine.Debug.LogFormat(Tag+": Updated: {0} {1}=>{2}", newi, binfo.Hash, hash);
                binfo.Hash = hash;
                SaveManifest(mLocalManifest, LocalManifestPath);

                mDiffList.Remove(newi);
            }

            AssetSys.UnloadBundle(subPath, false);
            AssetSys.GetBundleSync(subPath);

        }
    }

    public static void SaveManifest(BundleManifest manifest, string path)
    {
        var yaml = YamlHelper.Serialize(manifest, path);
    }
}