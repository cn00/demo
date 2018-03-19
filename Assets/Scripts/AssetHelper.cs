using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using XLua;

public static class AssetExtern
{
    public static T LoadABAsset<T>(this AssetBundle bundle, string subPath) where T : UnityEngine.Object
    {
        var asset = bundle.LoadAsset<T>(BundleConfig.ABResRoot + subPath);
        AppLog.d(subPath);
        return asset;
    }

    //public static T GetScriptableObject<T>(this WWW www) where T : UnityEngine.Object
    //{
    //}


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
public class DataObject : UnityEngine.Object
{
    public byte[] Data { get; protected set; }
    public DataObject(byte[] data)
    {
        Data = data;
    }
    public override string ToString()
    {
        return "DataObject:" + base.ToString();
    }
}

[LuaCallCSharp]
public class AssetHelper : SingletonMB<AssetHelper>
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
    public delegate void AssetCallback<T>(T obj) where T: UnityEngine.Object;
    Dictionary<string/*rootName*/, AssetBundleMB> mLoadedBundles = new Dictionary<string,AssetBundleMB>();

    public bool SysEnter()
    {
        if(!Directory.Exists(CacheRoot))
        {
            Directory.CreateDirectory(CacheRoot);
        }
        return true;
    }

    public override IEnumerator Init()
    {
        yield return base.Init();
    }

    /// <summary>
    /// 开启新协程执行, Lua 中不使用协程需要用这类方式调用
    /// </summary>
    public void GetBundleCo(string assetSubPath, AssetCallback<UnityEngine.Object> callBack = null)
    {
        StartCoroutine(GetAsset(assetSubPath, callBack));
    }

    /// <summary>
    /// 同步方式加载资源, 用于加载少量小型资源
    /// </summary>
    public T GetAssetSync<T>(string assetSubPath) where T : UnityEngine.Object
    {
        var trim = new char[] { ' ', '.', '/' };
        assetSubPath = assetSubPath.upath().TrimStart(trim).TrimEnd(trim);
        var dirs = assetSubPath.Split('/');
        string bundleName = dirs[0] + '/' + dirs[1] + BundleConfig.BundlePostfix;
        var bundle = GetBundleSync(bundleName);
        T asset = null;
        if(bundle != null)
        {
            asset = bundle.LoadAsset<T>(BundleConfig.ABResRoot + assetSubPath);
        }
        if(asset == null)
        {
            AppLog.w("[{0}/{1}] not exist.", bundleName, assetSubPath);
        }
        return asset;
    }

    /// <summary>
    /// 异步方式加载资源, 以加载后的 (Object)res 为参数调用 callBack 
    /// </summary>
    public IEnumerator GetAsset(string assetSubPath, AssetCallback<UnityEngine.Object> callBack = null)
    {
        yield return GetAsset<UnityEngine.Object>(assetSubPath, callBack);
    }

    public IEnumerator GetAsset<T>(string assetSubPath, AssetCallback<T> callBack = null) where T : UnityEngine.Object
    {
        UnityEngine.Object resObj = null;
#if UNITY_EDITOR
        if(ProjectConfig.Instance.UseBundle)
#endif
        {
            var trim = new char[] { ' ', '.', '/' };
            //assetSubPath = assetSubPath.upath().TrimStart(trim).TrimEnd(trim);
            var dirs = assetSubPath.Split('/');
            string bundleName = dirs[0] + '/' + dirs[1] + BundleConfig.BundlePostfix;
            yield return GetBundle(bundleName, (bundle) =>
            {
                // text
                var textPath = assetSubPath;
                if(textPath.IsText())
                {
                    if(textPath.EndsWith(".lua"))
                        textPath += ".txt";
                    var textAsset = bundle.LoadAsset<TextAsset>(BundleConfig.ABResRoot + textPath);
                    resObj = new DataObject(textAsset.bytes);
                }
                else
                {
                    resObj = bundle.LoadAsset<T>(BundleConfig.ABResRoot + assetSubPath);
                }
            });
            AppLog.d("from <Color=yellow>bundle</Color>: " + assetSubPath);
        }
#if UNITY_EDITOR
        // 编辑器从原始文件加载资源
        else
        {
            // text
            if(assetSubPath.IsText())
            {
                resObj = new DataObject(File.ReadAllBytes(BundleConfig.ABResRoot + assetSubPath));
            }
            else
            {
                resObj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(BundleConfig.ABResRoot + assetSubPath);
            }
            AppLog.d("from <Color=green>file</Color>: " + assetSubPath);
        }
#endif
        if(callBack != null)
            callBack((T)resObj);
    }

    public AssetBundle GetBundleSync(string bundlePath)
    {
        if(string.IsNullOrEmpty(bundlePath))
        {
            AppLog.e("bundlePath [{0}] not correct.", bundlePath);
            return null;
        }

        string rootName = bundlePath.Substring(0, bundlePath.IndexOf('/'));
        AssetBundleMB bundleMB = null;
        if(!mLoadedBundles.TryGetValue(rootName, out bundleMB))
        {
            bundleMB = mLoadedBundles[rootName] = new AssetBundleMB();
        }

        AssetBundle bundle = null;
        if(bundleMB.Bundles.ContainsKey(bundlePath))
        {
            bundle = bundleMB.Bundles[bundlePath];
        }
        else
        {
            bundle = AssetBundle.LoadFromFile(bundlePath);
            bundleMB.Bundles[bundlePath] = bundle;
        }
        return bundle;
    }

    /// <summary>
    /// AssetBundle 加载, 自动处理更新和依赖, 
    /// 以加载后的 AssetBundle 为参数调用 callBack 
    /// </summary>
    public IEnumerator GetBundle(string bundlePath, AssetCallback<UnityEngine.AssetBundle> callBack = null)
    {
        var bundle = GetBundleSync(bundlePath);
        if(bundle != null)
        {
            if(callBack != null)
                callBack(bundle);
            yield break;
        }

        string rootName = bundlePath.Substring(0, bundlePath.IndexOf('/'));
        AssetBundleMB bundleMB = null;
        if(!mLoadedBundles.TryGetValue(rootName, out bundleMB))
        {
            bundleMB = mLoadedBundles[rootName] = new AssetBundleMB();
        }

        var version = ProjectConfig.Instance.Version.ToString();
        var subPath = bundlePath;
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

        yield return Www(fileUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                if(isLocal)
                {
                    bundleMB.Bundles[bundlePath] = www.assetBundle;
                }
                else
                {
                    AsyncSave(cachePath + BundleConfig.CompressedExtension, www.bytes, www.bytes.Length);

                    MemoryStream outStream = new MemoryStream();
                    BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), outStream);

                    bundleMB.Bundles[bundlePath] = AssetBundle.LoadFromMemory(outStream.GetBuffer());

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
            var deps = bundleMB.Manifest.GetAllDependencies(bundlePath);
            foreach(var i in deps)
            {
                AppLog.d("Dependencies: " + i);
                yield return GetBundle(i);
            }
        }

        if(callBack != null)
            callBack(bundleMB.Bundles[bundlePath]);

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
