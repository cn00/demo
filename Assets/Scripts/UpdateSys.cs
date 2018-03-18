using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using XLua;

using YamlDotNet.Helpers;

using Md5SchemeDic = System.Collections.Generic.Dictionary<string, Md5SchemeInfo>;
using Md5Scheme = System.Collections.Generic.List<Md5SchemeInfo>;

[LuaCallCSharp]
public class UpdateSys : SingletonMB<UpdateSys>
{

    string mLocalMd5Path
    {
        get
        {
#if UNITY_EDITOR
            var version = ProjectConfig.Instance().Version.ToString();
            return BundleSys.CacheRoot + version + "/md5.xml";
#else
            return BundleSys.CacheRoot + "/md5.xml";
#endif
        }
    }

    Version LocalVersion;
    Version RemoteVersion;

    Md5SchemeDic mLocalMd5 = new  Md5SchemeDic();//<string/*path*/, Md5SchemeInfo>
    Md5SchemeDic mRemoteMd5 = new Md5SchemeDic();//<string/*path*/, Md5SchemeInfo>
    Md5SchemeDic mDiffList = new Md5SchemeDic();// <string/*path*/, Md5SchemeInfo>

    bool mAllDownloadOK = false;

    public bool SysEnter()
    {
        return true;
    }

    public IEnumerator SysEnterCo()
    {

        yield return GetLocalVersion();
        yield return GetRemoteVersion();

        // TODO: 检查更新打开这一行
        // yield return CheckUpdate();

        yield return null;// base.SysEnterCo();
    }

    public IEnumerator GetLocalVersion()
    {
        var cacheUrl = BundleSys.CacheRoot + "/resversion.txt";
        var localVersionUrl = "file://" + cacheUrl;
        Debug.Log(localVersionUrl);

        if(File.Exists(cacheUrl))
        {
            yield return BundleSys.Www(localVersionUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    // 覆盖硬编码版本号
                    ProjectConfig.Instance().Version = new FGVersion(www.text.Trim());
                }
                else
                {
                    Debug.LogError(www.error);
                }
            });
        }
        LocalVersion = ProjectConfig.Instance().Version.V;
        Debug.LogFormat("LocalVersion {0}", LocalVersion.ToString());

        yield return null;
    }

    public IEnumerator GetRemoteVersion()
    {
        var remoteVersionUrl = BundleSys.HttpRoot + "/resversion.txt";
        Debug.Log(remoteVersionUrl);
        yield return BundleSys.Www(remoteVersionUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                RemoteVersion = new Version(www.text.Trim());
                Debug.LogFormat("RemoteVersion {0}", RemoteVersion.ToString());
            }
            else
            {
                RemoteVersion = LocalVersion;
                Debug.LogError(remoteVersionUrl + ": " + www.error);
            }
        });
        yield return null;
    }

    public IEnumerator GetLocalMd5List()
    {
        var cachePath = mLocalMd5Path;
        var localMd5Url = "file://" + cachePath;

        if(!File.Exists(cachePath))
        {
            yield break;
        }
        yield return BundleSys.Www(localMd5Url, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                mLocalMd5 = YamlHelper.Deserialize<Md5SchemeDic>(www.text);
            }
            else
            {
                Debug.LogError("get local md5 list error: " + www.error);
            }
        });

        yield return null;
    }

    public IEnumerator GetRemoteMd5List()
    {
        var cachePath = mLocalMd5Path;
        var remoteMd5Url = BundleSys.HttpRoot + "/" + RemoteVersion + "/md5.xml" + BundleConfig.CompressedExtension;

        byte[] bytes = null;
        yield return BundleSys.Www(remoteMd5Url, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                bytes = www.bytes;
            }
            else
            {
                Debug.LogError(remoteMd5Url+ ": " + www.error);
            }
        });
        MemoryStream outStream = new MemoryStream();
        BundleHelper.DecompressFileLZMA(new MemoryStream(bytes), outStream);
        BundleSys.AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);

        mRemoteMd5 = YamlHelper.Deserialize<Md5SchemeDic>(outStream.ToString());

        yield return null;
    }

    /// <summary>
    /// 下载"必要"资源
    /// </summary>
    public void DownloadDiffFiles()
    {
        int count = 0;
        foreach(var i in mDiffList.Where(info=>info.Value.preDownload))
        {
            var subPath = i.Value.fname;
#if UNITY_EDITOR
            var cachePath = BundleSys.CacheRoot + "/" + RemoteVersion + "/" + subPath;
#else
        var cachePath = BundleSys.CacheRoot + "/" + subPath;
#endif
            var diffFileUrl = BundleSys.HttpRoot + "/" +  RemoteVersion + "/" + subPath + BundleConfig.CompressedExtension;
            Debug.Log(diffFileUrl);

            //yield return BundleSys.Www(fileUrl, (WWW www) =>
            StartCoroutine(BundleSys.Www(diffFileUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    MemoryStream outStream = new MemoryStream();
                    BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), outStream);

                    BundleSys.AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);
                    ++count;
                    if(count == mDiffList.Count)
                        mAllDownloadOK = true;
                    Updated(subPath);
                }
                else
                {
                    Debug.LogError("DownloadDiffFiles: " + i + www.error);
                }
            }));

        }
    }

    /// <summary>
    /// 需要更新的资源
    /// </summary>
    public void Diff()
    {
        foreach(var i in mRemoteMd5)
        {
            if(mLocalMd5.ContainsKey(i.Key))
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
            yield return GetLocalMd5List();
            yield return GetRemoteMd5List();

            Diff();

            DownloadDiffFiles();

            ProjectConfig.Instance().Version = RemoteVersion;

            // 更新完成后保存
            var cacheUrl = BundleSys.CacheRoot + "/resversion.txt";
            var strRemoteVersion = RemoteVersion.ToString();
            byte[] bytes = System.Text.Encoding.Default.GetBytes(strRemoteVersion);
            BundleSys.AsyncSave(cacheUrl, bytes, bytes.Length);
        }
        //else
        //{
        //    Debug.Log("no update");
        //}

        yield return null;
    }

    public bool NeedUpdate(string subPath)
    {
        Md5SchemeInfo info = null;
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
            mLocalMd5[subPath] = mDiffList[subPath];
            SaveMd5List(mLocalMd5, mLocalMd5Path);
        }
    }

    public static void SaveMd5List(Md5SchemeDic dic, string path)
    {
        var yaml = YamlHelper.Serialize(dic, path);

        // test
        var obj = YamlHelper.Deserialize<Md5SchemeDic>(yaml);
        //XMLHelper.XmlSerialize(dic.Values.ToList(), mLocalMd5Path);
    }
}
