using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using XLua;

public static class AssetExtern
{
    public static T GetXml<T>(this AssetBundle bundle, string path)
    {
        var stream = bundle.LoadAsset<TextAsset>(path);
        var deserializer = new XmlSerializer(typeof(T));
        var xml = (T)deserializer.Deserialize(new MemoryStream(stream.bytes));
        return xml;
    }

    public static T GetXml<T>(this TextAsset text)
    {
        var deserializer = new XmlSerializer(typeof(T));
        var xml = (T)deserializer.Deserialize(new MemoryStream(text.bytes));
        return xml;
    }
}

[XmlRoot]
public class Md5SchemeInfo
{
    public string fname { get; set; }
    public string md5 { get; set; }
    public bool preDownload { get; set; }
    public Md5SchemeInfo() { }
    public Md5SchemeInfo(string f, string m, bool pre = false)
    {
        fname = f;
        md5 = m;
        preDownload = pre;
    }
}


public static class BundleHelper
{
    #region 压缩

    #region LZMA
    public const int kPropSize = SevenZip.Compression.Lzma.Encoder.kPropSize;
    public class CProgressInfo : SevenZip.ICodeProgress
    {
        public Int64 ApprovedStart;
        public Int64 InSize;
        public System.DateTime Time;
        public void Init() { InSize = 0; }
        public void SetProgress(Int64 inSize, Int64 outSize)
        {
            if(inSize >= ApprovedStart && InSize == 0)
            {
                Time = DateTime.UtcNow;
                InSize = inSize;
            }
        }
    }

    public static string/*md5*/ CompressFileLZMA(string inFile, string outFile)
    {
        if(!File.Exists(inFile))
        {
            UnityEngine.Debug.LogError(inFile + " not found");
            return null;
        }

        SevenZip.Compression.Lzma.Encoder coder = new SevenZip.Compression.Lzma.Encoder();
        FileStream inStream = new FileStream(inFile, FileMode.Open);
        FileStream outStream = new FileStream(outFile, FileMode.Create);

        // Write the encoder properties
        coder.WriteCoderProperties(outStream); // 5 byte

        // Write the decompressed file size.
        outStream.Write(BitConverter.GetBytes(inStream.Length), 0, sizeof(long)); // 8 byte

        // Encode the file.
        CProgressInfo progressInfo = new CProgressInfo();
        Int32 dictionary = 1 << 21;
        progressInfo.ApprovedStart = dictionary;
        progressInfo.Init();
        coder.Code(inStream, outStream, inStream.Length, -1, progressInfo);
        outStream.Flush();

        var md5str = Md5(outStream);

        outStream.Close();
        inStream.Close();

        return md5str;
    }

    public static string Md5(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hash_byte = md5.ComputeHash(stream);

        var md5str = System.BitConverter.ToString(hash_byte);
        md5str = md5str.Replace("-", "");

        md5.Clear();
        return md5str;
    }

    public static string Md5(string fname)
    {
        FileStream inStream = new FileStream(fname, FileMode.Open);
        var md5str = Md5(inStream);
        inStream.Close();
        return md5str;
    }

    public static void DecompressFileLZMA(string inFile, string outFile)
    {
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        DecompressFileLZMA(input, output);

        output.Close();
        input.Close();
    }


    public static void DecompressFileLZMA(Stream input, Stream output)
    {
        SevenZip.Compression.Lzma.Decoder coder = new SevenZip.Compression.Lzma.Decoder();
        input.Seek(0, SeekOrigin.Begin);
        output.Seek(0, SeekOrigin.Begin);

        // Read the decoder properties
        byte[] properties = new byte[kPropSize];
        input.Read(properties, 0, kPropSize);

        // Read in the decompress file size.
        byte[] fileLengthBytes = new byte[sizeof(long)];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        // Decompress the file.
        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();

        output.Seek(0, SeekOrigin.Begin);
    }
    #endregion LZMA

    #endregion 压缩

}

public static class UPath
{
    public static string upath(this string self)
    {
        return self.Trim()
            .Replace("\\", "/")
            .Replace("//", "/");
    }
    public static string go(string path)
    {
        return path.Trim()
            .Replace("\\", "/")
            .Replace("//", "/");
    }
}

[LuaCallCSharp]
public class BundleSys : Singleton<BundleSys>
{

    static string mCacheRoot = "";
    public static string CacheRoot
    {
        get
        {
            if(string.IsNullOrEmpty(mCacheRoot))
            {
                var cacheDirName = "/AssetBundle/";
#if UNITY_EDITOR
                cacheDirName += PlatformName(RuntimePlatform.Android);
                mCacheRoot = Application.dataPath + "/.." + cacheDirName;
#elif UNITY_ANDROID
                cacheDirName += PlatformName(RuntimePlatform.Android);
                mCacheRoot = Application.persistentDataPath + cacheDirName;
#elif UNITY_IPHONE
                cacheDirName += PlatformName(RuntimePlatform.IPhonePlayer);
                mCacheRoot = Application.persistentDataPath + cacheDirName;
#elif UNITY_WINDOWS
                cacheDirName += PlatformName(RuntimePlatform.WindowsPlayer);
                mCacheRoot = Application.streamingAssetsPath + "/../" + cacheDirName;
#else
                cacheDirName += PlatformName(RuntimePlatform.Android);
                CacheRoot = Application.streamingAssetsPath + "/../" + cacheDirName;
#endif
            }
            return mCacheRoot;
        }
    } // set in Runtime
    static string mHttpRoot = null;
    public static string HttpRoot
    {
        get
        {
            if(string.IsNullOrEmpty(mHttpRoot))
            {
                mHttpRoot = "http://192.168.8.36:8008/share/fg/AssetBundle/";
#if UNITY_EDITOR
                mHttpRoot += PlatformName(RuntimePlatform.Android);
#elif UNITY_ANDROID
                mHttpRoot += PlatformName(RuntimePlatform.Android);
#elif UNITY_IPHONE
                mHttpRoot += PlatformName(RuntimePlatform.IPhonePlayer);
#elif UNITY_WINDOWS
                mHttpRoot += PlatformName(RuntimePlatform.WindowsPlayer);
#else
                mHttpRoot += PlatformName(RuntimePlatform.Android);
#endif
            }
            return mHttpRoot;
        }
    }

    public static string PlatformName(RuntimePlatform platform)
    {
        switch(platform)
        {
        case RuntimePlatform.Android:
            return "Android";
        case RuntimePlatform.IPhonePlayer:
            return "iOS";
        case RuntimePlatform.WindowsPlayer:
        case RuntimePlatform.WindowsEditor:
            return "Windows";
        case RuntimePlatform.OSXPlayer:
            return "OSX";
        default:
            return null;
        }
    }

    public class AssetBundleMB
    {
        public AssetBundleManifest Manifest = null;
        public Dictionary<string, AssetBundle> Bundles = new Dictionary<string, AssetBundle>();
    }

    [CSharpCallLua]
    public delegate void BundleCallback(UnityEngine.Object BundleMB);
    Dictionary<string/*rootName*/, AssetBundleMB> mLoadedBundles = new Dictionary<string,AssetBundleMB>();

    public bool SysEnter()
    {
        if(!Directory.Exists(CacheRoot))
        {
            Directory.CreateDirectory(CacheRoot);
        }
        return true;
    }

    public IEnumerator SysEnterCo()
    {
        //if(CGameRoot.Instance.UseBundle)
        {
            ////load all manifest bundles
            //foreach(var i in BundleConfig.ABResRoots)
            //{
            //    var bundleMB = mLoadedBundles[i.Value] = new AssetBundleMB();
            //    yield return _GetBundle(i.Value, i.Value, (UnityEngine.Object manifestBundle) =>
            //    {
            //        var manifest = (manifestBundle as AssetBundle).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //        bundleMB.Manifest = manifest;
            //    });
            //}

            //// streamed scene have no manifest?
            //foreach(var i in BundleConfig.ABSceneRoots)
            //{
            //    mLoadedBundles[i.Value] = new AssetBundleMB();
            //}
        }
        yield return null;//SysEnterCo();
    }

    /// <summary>
    /// 以加载后的 AudioClip 为参数调用 callBack
    /// </summary>
    public IEnumerator GetAudio(string bundleName, string resSubPath, BundleCallback callBack = null)
    {
#if !UNITY_EDITOR
        yield return GetBundle("Audio", bundleName, resSubPath, callBack);
#else
        if(ProjectConfig.Instance().UseBundle)
            yield return GetBundle(bundleName, resSubPath, callBack);
        else
        {
            //FIXME: remove this
            //yield break;

            var assetName = "ABResources/" + "Audio" + "/" + resSubPath;
            var fileUrl = "file://" + Application.dataPath + "/" + assetName;
            Debug.Log(fileUrl);

            yield return Www(fileUrl, (WWW www) =>
            {
                if(string.IsNullOrEmpty(www.error))
                {
                    var clip = WWWAudioExtensions.GetAudioClip(www);
                    if(callBack != null)
                        callBack(clip);

                }
                else
                {
                    Debug.LogError(fileUrl + ": " + www.error);
                }
            });
        }
#endif
    }

    /// <summary>
    /// 开启新协程执行, Lua 中不使用协程需要用这类方式调用
    /// </summary>
    public void GetBundleCo(string bundleName, string resSubPath, BundleCallback callBack = null)
    {
        StartCoroutine(GetBundle(bundleName, resSubPath, callBack));
    }

    /// <summary>
    /// 以加载后的 (Object)res 为参数调用 callBack 
    /// </summary>
    public IEnumerator GetBundle(string bundleName, string resSubPath, BundleCallback callBack = null)
    {
        // 从 AssetBundle 加载
        //if(CGameRoot.Instance.UseBundle)
        {
            UnityEngine.Object resObj = null;
            yield return _GetBundle(bundleName, (UnityEngine.Object bundle) =>
            {
                resObj = (bundle as AssetBundle).LoadAsset(BundleConfig.ABResourceRoot + resSubPath);
            });
            if(callBack != null)
                callBack(resObj);
            Debug.Log("<Color=green>from bundle: " + resSubPath + "</Color>");
        }
    }

    /// <summary>
    /// AssetBundle 加载, 自动处理更新和依赖, 
    /// 以加载后的 AssetBundle 为参数调用 callBack 
    /// </summary>
    IEnumerator _GetBundle(string bundlePath, BundleCallback callBack = null)
    {
        string rootName = bundlePath.Substring(0, bundlePath.IndexOf('/'));
        string bundleName = bundlePath;
        if(string.IsNullOrEmpty(rootName) || string.IsNullOrEmpty(bundleName))
        {
            yield break;
        }
        var bundleMB = mLoadedBundles[rootName];
        if(bundleMB.Bundles.ContainsKey(bundleName))
        {
            if(callBack != null)
                callBack(bundleMB.Bundles[bundleName]);
            yield break;
        }

        if(string.IsNullOrEmpty(bundleName))
            yield break;

        var version = ProjectConfig.Instance().Version.ToString();
        var subPath = rootName + "/" + bundleName;
#if UNITY_EDITOR
        var cachePath = CacheRoot + "/" + version + "/" + subPath;
#else
        var cachePath = CacheRoot + "/" + subPath;
#endif
        var fileUrl = "file://" + cachePath;
        var isLocal = true;
        if(!File.Exists(cachePath)
            //|| UpdateSys.Instance.NeedUpdate(subPath)
        )
        {
            isLocal = false;
            fileUrl = HttpRoot + "/" + version + "/" + subPath + BundleConfig.CompressedExtension;
        }
        Debug.Log(fileUrl);

        yield return Www(fileUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                if(isLocal)
                {
                    bundleMB.Bundles[bundleName] = www.assetBundle;
                }
                else
                {
                    AsyncSave(cachePath + BundleConfig.CompressedExtension, www.bytes, www.bytes.Length);

                    MemoryStream outStream = new MemoryStream();
                    BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), outStream);

                    bundleMB.Bundles[bundleName] = AssetBundle.LoadFromMemory(outStream.GetBuffer());

                    AsyncSave(cachePath, outStream.GetBuffer(), outStream.Length);
                    //UpdateSys.Instance.Updated(subPath);
                }
            }
            else
            {
                Debug.LogError(fileUrl + ": " + www.error);
            }
        });

        // Dependencies
        if(bundleMB.Manifest != null)
        {
            var deps = bundleMB.Manifest.GetAllDependencies(bundleName);
            foreach(var i in deps)
            {
                Debug.Log("Dependencies: " + i);
                yield return _GetBundle(i);
            }
        }

        if(callBack != null)
            callBack(bundleMB.Bundles[bundleName]);

        yield return null;
    }

    public static IEnumerator Www(string url, UnityAction<WWW> endCallback = null, UnityAction<float> progressCallback = null)
    {
        WWW www = new WWW(url);
        DateTime timeout = DateTime.Now + new TimeSpan(0, 0, 10);
        if(www != null)
        {
            while(!www.isDone && string.IsNullOrEmpty(www.error))
            {
                if(progressCallback != null)
                {
                    progressCallback(www.progress);
                    if(DateTime.Now > timeout && www.progress < 0.1f)
                    {
                        Debug.Log("<Colro=red>timeout: " + url + "</Color>");
                        break;
                    }
                }
                yield return null;
            }

            if(string.IsNullOrEmpty(www.error))
            {
                // 留给调用者选择是否存盘
                //if(url.Substring(0, 7) == "http://")
                //{
                //    AsyncSave(url.Replace(HttpRoot + "/" + CGameRoot.Instance.Version, CacheRoot), www.bytes);
                //}

                Debug.LogFormat("loaded <Color=green>{0}</Color> OK", url);
                if(endCallback != null)
                {
                    endCallback(www);
                }
            }
            else
            {
                Debug.LogError(url + www.error);
            }
        }
        www.Dispose();

        yield return null;
    }

    /// <summary>
    /// 在新线程中异步保存文件
    /// </summary>
    public static void AsyncSave(string fname, byte[] bytes, long Length)
    {
        var dir = Path.GetDirectoryName(fname);
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream writer = new FileStream(fname, FileMode.OpenOrCreate);
        writer.BeginWrite(bytes, 0, (int)Length, (IAsyncResult result) =>
        {
            FileStream stream = (FileStream)result.AsyncState;
            stream.EndWrite(result);
            stream.Close();
            Debug.Log("Saved:" + fname);
        }, writer);
    }
}
