using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using XLua;
using System.Text;

using BundleManifest = System.Collections.Generic.Dictionary<string, BundleInfo>;

public static class BytesExtension
{
    public static string Utf8String(this byte[] self)
    {
        return Encoding.UTF8.GetString(self);
    }
}

[LuaCallCSharp]
public class UpdateSys : SingleMono<UpdateSys>
{

    Version mLocalVersion;
    Version mRemoteVersion;

    BundleManifest mLocalManifest = new  BundleManifest();//<string/*path*/, Md5SchemeInfo>
    BundleManifest mRemoteManifest = new BundleManifest();//<string/*path*/, Md5SchemeInfo>
    BundleManifest mDiffList = new BundleManifest();// <string/*path*/, Md5SchemeInfo>

    bool mAllDownloadOK = false;

    public bool SysEnter()
    {
        return true;
    }

    public override IEnumerator Init()
    {

        yield return GetLocalVersion();
        yield return GetRemoteVersion();

        // TODO: 检查更新打开这一行
        yield return CheckUpdate();

        yield return base.Init();
    }

    public IEnumerator GetLocalVersion()
    {
        var cacheUrl = AssetSys.CacheRoot + "/resversion.txt";
        var localVersionUrl = "file://" + cacheUrl;
        AppLog.d(localVersionUrl);

        if(File.Exists(cacheUrl))
        {
            yield return AssetSys.Www(localVersionUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    // 覆盖硬编码版本号
                    ProjectConfig.Instance.Version = new AppVersion(www.text.Trim());
                }
                else
                {
                    AppLog.e(www.error);
                }
            });
        }
        mLocalVersion = ProjectConfig.Instance.Version.V;
        AppLog.d("LocalVersion {0}", mLocalVersion.ToString());

        yield return null;
    }

    public IEnumerator GetRemoteVersion()
    {
        var remoteVersionUrl = AssetSys.HttpRoot + "/resversion.txt";
        AppLog.d(remoteVersionUrl);
        yield return AssetSys.Www(remoteVersionUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                mRemoteVersion = new Version(www.text.Trim());
                AppLog.d("RemoteVersion {0}", mRemoteVersion.ToString());
            }
            else
            {
                mRemoteVersion = mLocalVersion;
                AppLog.e(remoteVersionUrl + ": " + www.error);
            }
        });
        yield return null;
    }

    public IEnumerator GetLocalManifest()
    {
        var cachePath = BundleConfig.LocalManifestPath;
        var localMd5Url = "file://" + cachePath;

        if(!File.Exists(cachePath))
        {
            yield break;
        }
        yield return AssetSys.Www(localMd5Url, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                AppLog.d("deserialize mLocalManifest {0}", www.text);
                mLocalManifest = YamlHelper.Deserialize<BundleManifest>(www.text);
                AppLog.d("deserialize mLocalManifest 1");
            }
            else
            {
                AppLog.e("get local md5 list error: " + www.error);
            }
        });

        yield return null;
    }

    public IEnumerator GetRemoteManifest()
    {
        var cachePath = BundleConfig.LocalManifestPath;
        var remoteManifestUrl = AssetSys.HttpRoot + "/" + mRemoteVersion + "/" + BundleConfig.ManifestName + BundleConfig.CompressedExtension;

        byte[] bytes = null;
        yield return AssetSys.Www(remoteManifestUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                bytes = www.bytes;
            }
            else
            {
                AppLog.e(remoteManifestUrl+ ": " + www.error);
            }
        });
        var outStream = new MemoryStream();
        BundleHelper.DecompressFileLZMA(new MemoryStream(bytes), outStream);
        AssetSys.AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);

        AppLog.d("deserialize mRemoteManifest 0");
        mRemoteManifest = YamlHelper.Deserialize<BundleManifest>(outStream);
        AppLog.d("deserialize mRemoteManifest 1");
        outStream.Dispose();
        yield return null;
    }

    /// <summary>
    /// 下载"必要"资源
    /// </summary>
    public void DownloadDiffFiles()
    {
        int count = 0;
        foreach(var i in mDiffList)//.Where(info=>info.Value.preDownload))
        {
            var subPath = i.Key;
#if UNITY_EDITOR
            var cachePath = AssetSys.CacheRoot + "/" + mRemoteVersion + "/" + subPath;
#else
            var cachePath = AssetSys.CacheRoot + "/" + subPath;
#endif
            var diffFileUrl = AssetSys.HttpRoot + "/" +  mRemoteVersion + "/" + subPath + BundleConfig.CompressedExtension;
            AppLog.d(diffFileUrl);

            //yield return AssetSys.Www(fileUrl, (WWW www) =>
            StartCoroutine(AssetSys.Www(diffFileUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    //// 异步存盘
                    //MemoryStream outStream = new MemoryStream();
                    //BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), outStream);
                    //AssetSys.AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);

                    // or 同步存盘
                    var fstream = new FileStream(cachePath, FileMode.Create);
                    BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), fstream);

                    ++count;
                    if(count == mDiffList.Count)
                        mAllDownloadOK = true;
                    Updated(subPath);
                }
                else
                {
                    AppLog.e("DownloadDiffFiles: " + i + www.error);
                }
            }));

        }
    }

    /// <summary>
    /// 需要更新的资源
    /// </summary>
    public void Diff()
    {
        foreach(var i in mRemoteManifest)
        {
            if(mLocalManifest.ContainsKey(i.Key))
            {
                continue;
            }
            mDiffList[i.Key] = i.Value;
        }
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    public IEnumerator CheckUpdate()
    {
        var isOK = false;
        // 是否需要更新
        //if(LocalVersion != RemoteVersion)// 允许回档到历史版本?
        {
            // download md5 list & uncompress & clean zip
            yield return GetLocalManifest();
            yield return GetRemoteManifest();

            Diff();

            DownloadDiffFiles();

            ProjectConfig.Instance.Version = mRemoteVersion;

            // 更新完成后保存
            var cacheUrl = AssetSys.CacheRoot + "resversion.txt";
            var strRemoteVersion = mRemoteVersion.ToString();
            byte[] bytes = System.Text.Encoding.Default.GetBytes(strRemoteVersion);
            AssetSys.AsyncSave(cacheUrl, bytes, bytes.Length);
        }
        //else
        //{
        //    AppLog.d("no update");
        //}

        yield return null;
    }

    public bool NeedUpdate(string subPath)
    {
        BundleInfo info = null;
        if(mDiffList.TryGetValue(subPath, out info))
        {
            return info != null;
        }
        return false;
    }

    /// <summary>
    /// 将更新过的md5更新到本地
    /// </summary>
    public void Updated(string subPath)
    {
        if(mDiffList.ContainsKey(subPath))
        {
            mLocalManifest[subPath] = mDiffList[subPath];
            SaveManifest(mLocalManifest, BundleConfig.LocalManifestPath);
            mDiffList.Remove(subPath);
        }
    }

    public static void SaveManifest(BundleManifest dic, string path)
    {
        var yaml = YamlHelper.Serialize(dic, path);
    }
}
