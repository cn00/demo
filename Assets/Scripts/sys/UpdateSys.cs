using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using XLua;
using System.Text;
using UnityEngine.Networking;
using BundleManifest = System.Collections.Generic.List<BuildConfig.BundleInfo>;
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

    BundleManifest mLocalManifest = new  BundleManifest();//<string/*path*/, Md5SchemeInfo>
    BundleManifest mRemoteManifest = new BundleManifest();//<string/*path*/, Md5SchemeInfo>

    object mDiffListLock = new object();
    List<BuildConfig.BundleInfo> mDiffList = new List<BuildConfig.BundleInfo>();// <string/*path*/, Md5SchemeInfo>

    bool mAllDownloadOK = false;

    public IEnumerator GetLocalVersion()
    {
        var cachePath = AssetSys.CacheRoot + "resversion.txt";
        var localVersionUrl = "file://" + cachePath;
        AppLog.d(Tag, "GetLocalVersion: {0}", localVersionUrl);

        if(File.Exists(cachePath))
        {
            yield return AssetSys.Www(localVersionUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    // 覆盖硬编码版本号
                    BuildConfig.Instance().Version = new AppVersion(www.text.Trim());
                }
                else
                {
                    AppLog.e(Tag, www.error);
                }
            });
        }
        mLocalVersion = BuildConfig.Instance().Version.V;
        AppLog.d(Tag, "LocalVersion {0}", mLocalVersion.ToString());

        yield return null;
    }

    public IEnumerator GetRemoteVersion()
    {
        var remoteVersionUrl = AssetSys.WebRoot + "resversion.txt";
        AppLog.d(Tag, remoteVersionUrl);
        yield return AssetSys.Www(remoteVersionUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                mRemoteVersion = new Version(www.text.Trim());
                AppLog.d(Tag, "RemoteVersion {0}", mRemoteVersion.ToString());
            }
            else
            {
                mRemoteVersion = mLocalVersion;
            }
        });
        yield return null;
    }

    public IEnumerator GetLocalManifest()
    {
        var cachePath = BuildConfig.LocalManifestPath;

        if(!File.Exists(cachePath))
        {
            yield break;
        }

        //var s = File.ReadAllText(cachePath);
        //mLocalManifest = YamlHelper.Deserialize<BundleManifest>(s);

        var localMd5Url = "file://" + cachePath;
        yield return AssetSys.Www(localMd5Url, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                mLocalManifest = YamlHelper.Deserialize<BundleManifest>(www.text);
                // AppLog.d(Tag, "LocalManifest: \n" + www.text);
            }
            else
            {
                AppLog.e(Tag, "get local md5 list error: " + www.error);
            }
        });

        yield return null;
    }

    public IEnumerator GetRemoteManifest()
    {
        var remoteManifestUrl = AssetSys.WebRoot + mRemoteVersion + "/" + BuildConfig.ManifestName + BuildConfig.CompressedExtension;

        byte[] bytes = null;
        bool err = false;
        yield return AssetSys.Www(remoteManifestUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                bytes = www.bytes;
            }
            else
            {
                err = true;
            }
        });
        if(err)
            yield break;

        var outStream = new MemoryStream();
        BundleHelper.DecompressFileLZMA(new MemoryStream(bytes), outStream);
        //AssetSys.AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);

        var s = outStream.GetBuffer().Utf8String();
        // AppLog.d(Tag, "RemoteManifest: \n" + s);
        mRemoteManifest = YamlHelper.Deserialize<BundleManifest>(s);
        outStream.Dispose();
        yield return null;
    }

    /// <summary>
    /// 下载差异资源
    /// </summary>
    public IEnumerator DownloadDiffFiles()
    {
        int count = 0;
        for(var idx = 0; idx < mDiffList.Count; ++idx)
        {
            var i = mDiffList[idx];
            var subPath = i.Name;
            var cachePath = AssetSys.CacheRoot + subPath;
            var cacheLzmaPath = AssetSys.CacheRoot + subPath + BuildConfig.CompressedExtension;
            var diffFileUrl = AssetSys.WebRoot +  mRemoteVersion + "/" + subPath + BuildConfig.CompressedExtension;
            AppLog.d(Tag, diffFileUrl);

            //yield return AssetSys.Www(fileUrl, (WWW www) =>
            var task = AssetSys.Www(diffFileUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    var dir = Path.GetDirectoryName(cachePath);
                    if(!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    //// 异步存盘
                    //MemoryStream outStream = new MemoryStream();
                    //BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), outStream);
                    //AssetSys.AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);
                    AssetSys.AsyncSave(cacheLzmaPath, www.bytes);

                    // or 同步存盘
                    AppLog.d(Tag, "update: {0}", cachePath);
                    var fstream = new FileStream(cachePath, FileMode.Create);
                    BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), fstream);

                    ++count;
                    if(count == mDiffList.Count)
                        mAllDownloadOK = true;
                    Updated(subPath);
                }
                else
                {
                    AppLog.e(Tag, "DownloadDiffFiles: " + i + www.error);
                }
            });

            // TODO: fixed this task group
            if(count % 10 == 0)
            {
                yield return StartCoroutine(task);
            }
            else
            {
                 StartCoroutine(task);
            }
        }
    }

    /// <summary>
    /// 需要更新的资源
    /// </summary>
    public void Diff()
    {
         foreach(var r in mRemoteManifest)
        {
            var l = mLocalManifest.Find(i => i.Name == r.Name);
            if(l == null || l.Md5 != r.Md5 )
            {
                AppLog.d(Tag, "diff: {0}:[{1} {2}]", r.Name, r.Md5, (l != null ? l.Md5 : ""));
                mDiffList.Add(r);
            }
        }
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    public IEnumerator CheckUpdate()
    {
        #if UNITY_EDITOR
        if(!BuildConfig.Instance().UseBundle)
            yield break;
        #endif
        // TODO: update to new bundle system use Manifest hash128

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
            var www = UnityWebRequest.Get("");

            BuildConfig.Instance().Version = mRemoteVersion;

            // 更新完成后保存
            var cacheUrl = AssetSys.CacheRoot + "resversion.txt";
            var strRemoteVersion = mRemoteVersion.ToString();
            // File.WriteAllText(cacheUrl, strRemoteVersion);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(strRemoteVersion);
            AssetSys.AsyncSave(cacheUrl, bytes);
        }
        //else
        //{
        //    AppLog.d(Tag, "no update");
        //}

        yield return null;
    }

    public bool NeedUpdate(string subPath)
    {
        BuildConfig.BundleInfo info = mDiffList.Find(i=>i.Name == subPath);
        return info != null;
    }

    /// <summary>
    /// 将更新过的md5更新到本地
    /// </summary>
    public void Updated(string subPath)
    {
        // AppLog.d(Tag, "Updated: {0}", subPath);
        //lock(mDiffListLock)
        {

            var newi = mDiffList.Find(i => i.Name == subPath);
            if(newi != null)
            {
                var old = mLocalManifest.Find(i => i.Name == subPath);
                mLocalManifest.Remove(old);
                mLocalManifest.Add(newi);
                SaveManifest(mLocalManifest, BuildConfig.LocalManifestPath);

                mDiffList.Remove(newi);
                AppLog.d(Tag, "Updated: {0}={1}", newi.Name, newi.Md5);
            }
            
            AssetSys.Instance.UnloadBundle(subPath, false);
            AssetSys.Instance.GetBundleSync(subPath);

        }
    }

    public static void SaveManifest(BundleManifest manifest, string path)
    {
        var yaml = YamlHelper.Serialize(manifest, path);
    }
}
