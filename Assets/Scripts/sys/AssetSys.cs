using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        var asset = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + subPath);
        AppLog.d(subPath);
        return asset;
    }

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

    public static void CompressFileLZMA(string inFile, string outFile)
    {
        if(!File.Exists(inFile))
        {
            AppLog.e(inFile + " not found");
            return;
        }

        var outDir = Path.GetDirectoryName(outFile);
        if(!Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
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

        outStream.Close();
        inStream.Close();
    }

    public static string Md5(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
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

    public static string Base64(byte[] bytes)
    {
        return System.Convert.ToBase64String(bytes);
    }

    public static void DecompressFileLZMA(string inFile, string outFile)
    {
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        DecompressFileLZMA(input, output);

        output.Close();
        input.Close();
    }


    public static void DecompressFileLZMA(Stream input,  Stream output)
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

public class DataObject : UnityEngine.Object
{
    public byte[] Data { get; protected set; }
    public DataObject(byte[] data)
    {
        Data = data;
    }
    public override string ToString()
    {
        return Encoding.UTF8.GetString(Data);
    }
}

[LuaCallCSharp]
public class AssetSys : SingleMono<AssetSys>
{
    static string mCacheRoot = "";
    /// <summary>
    /// Application.dataPath + "/AssetBundle/${PlatformName}/" 
    /// </summary>
    public static string CacheRoot
    {
        get
        {
            if(string.IsNullOrEmpty(mCacheRoot))
            {
                var cacheDirName = "AssetBundle/";
#if UNITY_EDITOR
#   if UNITY_IOS
                cacheDirName += PlatformName(RuntimePlatform.IPhonePlayer) + "/";
#   elif UNITY_ANDROID
                cacheDirName += PlatformName(RuntimePlatform.Android) + "/";
#   else
                cacheDirName += PlatformName(Application.platform) + "/";
#   endif
                mCacheRoot = Application.dataPath + "/../" + cacheDirName;
#else //!UNITY_EDITOR
#   if UNITY_ANDROID || UNITY_IPHONE
                mCacheRoot = Application.persistentDataPath + "/" + cacheDirName;
#   else // UNITY_WINDOWS
                mCacheRoot = Application.streamingAssetsPath + "/" + cacheDirName;
#   endif
#endif
            }
            return mCacheRoot;
        }
    } // set in Runtime
    static string mHttpRoot = null;
    /// <summary>
    /// http://ip:port/path/to/root/platform/
    /// </summary>
    /// <value>The http root.</value>
    public static string HttpRoot
    {
        get
        {
            if(string.IsNullOrEmpty(mHttpRoot))
            {
				mHttpRoot = BuildConfig.Instance ().ServerRoot;
#if UNITY_EDITOR
                mHttpRoot += BuildScript.TargetName(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
                mHttpRoot += PlatformName(Application.platform);
#endif
                mHttpRoot += "/";
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
        case RuntimePlatform.OSXEditor:
            return "OSX";
        default:
            return null;
        }
    }

    public AssetBundleManifest mManifest = null;
    Dictionary<string, AssetBundle> mLoadedBundles = new Dictionary<string,AssetBundle>();

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
// #if !UNITY_EDITOR
        yield return GetBundle("ui/boot.bd");
// #endif
        yield return base.Init();
    }

    /// <summary>
    /// 同步方式加载资源, 用于加载少量小型资源
    /// </summary>
    public T GetAssetSync<T>(string assetSubPath) where T : UnityEngine.Object
    {
        var trim = new char[] { ' ', '.', '/' };
        assetSubPath = assetSubPath.upath().TrimStart(trim).TrimEnd(trim);
        var dirs = assetSubPath.Split('/');
        string bundleName = dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
        var bundle = GetBundleSync(bundleName);
        T asset = null;
        if(bundle != null)
        {
            asset = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + assetSubPath);
        }
        if(asset == null)
        {
            AppLog.w("[{0}({2}):{1}] not exist.", bundleName, BuildConfig.BundleResRoot + assetSubPath, bundle);
        }
        return asset;
    }

    public string GetBundlePath (string assetSubPath)
    {
        var dirs = assetSubPath.Split('/');
        string bundleName = dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
        return bundleName;
    }

    /// <summary>
    /// 异步方式加载资源, 以加载后的 (Object)res 为参数调用 callBack 
    /// </summary>
    public IEnumerator GetAsset(string assetSubPath, Action<UnityEngine.Object> callBack = null)
    {
        yield return GetAsset<UnityEngine.Object>(assetSubPath, callBack);
    }

    public IEnumerator GetAsset<T>(string assetSubPath, Action<T> callBack = null) where T : UnityEngine.Object
    {
        UnityEngine.Object resObj = null;
#if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
#endif
        {
//            var trim = new char[] { ' ', '.', '/' };
            //assetSubPath = assetSubPath.upath().TrimStart(trim).TrimEnd(trim);
            var dirs = assetSubPath.Split('/');

#if UNITY_EDITOR
            string manifestBundleName = BuildScript.TargetName(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
            // "iOS";// dirs[0] + '/' + dirs[0];
#else
            string manifestBundleName = PlatformName(Application.platform);
#endif
            AppLog.d("load manifest: " + manifestBundleName);
            yield return GetBundle(manifestBundleName, (bundle) =>
            {
                var manifext = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                mManifest = manifext;
            });

            string bundleName = dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
            AppLog.d("from bundle: " + assetSubPath);
            yield return GetBundle(bundleName, (bundle) =>
            {
                // text
                var textPath = assetSubPath;
                if(textPath.IsText())
                {
                    if(textPath.EndsWith(".lua"))
                        textPath += ".txt";
                    var textAsset = bundle.LoadAsset<TextAsset>(BuildConfig.BundleResRoot + textPath);
                    resObj = new DataObject(textAsset.bytes);
                }
                else
                {
                    resObj = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + assetSubPath);
                }
            });
        }
#if UNITY_EDITOR
        // 编辑器从原始文件加载资源
        else
        {
            // text
            AppLog.d("from file: " + assetSubPath);
            if(assetSubPath.IsText())
            {
                resObj = new DataObject(File.ReadAllBytes(BuildConfig.BundleResRoot + assetSubPath));
            }
            else
            {
                resObj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(BuildConfig.BundleResRoot + assetSubPath);
            }
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

        AssetBundle bundle = null;
        if(!mLoadedBundles.TryGetValue(bundlePath, out bundle))
        {
            var cachePath = CacheRoot + "/" + bundlePath;
            bundle = AssetBundle.LoadFromFile(cachePath);
            if(bundle == null)
            {
                bundle = Resources.Load<AssetBundle>(bundlePath);
            }
            mLoadedBundles[bundlePath] = bundle;
            AppLog.w("GetBundleSync: {0}", bundlePath);
        }

        if(bundle == null)
        {
            AppLog.e("[{0}] did not download yet.", bundlePath);
        }
        return bundle;
    }

    /// <summary>
    /// AssetBundle 加载, 自动处理更新和依赖, 
    /// 以加载后的 AssetBundle 为参数调用 callBack 
    /// </summary>
    public IEnumerator GetBundle(string bundleName, Action<UnityEngine.AssetBundle> callBack = null)
    {
        string bundlePath = bundleName;
        if(string.IsNullOrEmpty(bundlePath))
        {
            AppLog.e("bundlePath [{0}] not correct.", bundlePath);
            yield break;
        }

        AssetBundle bundle = null;
        if(mLoadedBundles.TryGetValue(bundlePath, out bundle))
        {
            if(callBack != null)
                callBack(bundle);
            yield break;
        }

        var version = BuildConfig.Instance().Version.ToString();
        var subPath = bundlePath;
        var cachePath = CacheRoot + subPath;
        var fileUrl = "file://" + cachePath;

        var isLocal = true;
        if(!File.Exists(cachePath)
        //|| UpdateSys.Instance.NeedUpdate(subPath)
        )
        {
            isLocal = false;
            fileUrl = HttpRoot + version + "/" + subPath + BuildConfig.CompressedExtension;
        }

        AppLog.d(fileUrl);
        yield return Www(fileUrl, (WWW www) =>
        {
            if(string.IsNullOrEmpty(www.error))
            {
                if(isLocal)
                {
                    mLoadedBundles[bundlePath] = www.assetBundle;
                }
                else
                {
#if UNITY_EDITOR
                    AsyncSave(cachePath + BuildConfig.CompressedExtension, www.bytes);
#endif
                    MemoryStream outStream = new MemoryStream();
                    BundleHelper.DecompressFileLZMA(new MemoryStream(www.bytes), outStream);

                    // TODO: decode buffer
                    mLoadedBundles[bundlePath] = AssetBundle.LoadFromMemory(outStream.GetBuffer());

                    AsyncSave(cachePath, outStream.GetBuffer());
                    //UpdateSys.Instance.Updated(subPath);
                }
            }
            else
            {
                AppLog.e(fileUrl + ": " + www.error);
            }
        });

        // Dependencies
        if(mManifest != null)
        {
            var deps = mManifest.GetAllDependencies(bundlePath);
            foreach(var i in deps)
            {
                AppLog.d("Dependencies: " + i);
                yield return GetBundle(i);
            }
        }

        if(callBack != null)
            callBack(mLoadedBundles[bundlePath]);

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
                //yield return www;
                if(progressCallback != null)
                {
                    progressCallback(www.progress);
                    if(DateTime.Now > timeout && www.progress < 0.1f)
                    {
                        AppLog.d("<Colro=red>timeout: " + url + "</Color>");
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

                AppLog.d("loaded <Color=green>{0}</Color> OK", url);
                if(endCallback != null)
                {
                    endCallback(www);
                }
            }
            else
            {
                AppLog.e(url + ": " + www.error);
            }
        }
        www.Dispose();

        yield return null;
    }

    public void UnloadBundle(string path, bool unloadAllLoadedObjects = false)
    {
        AssetBundle outBundle = null;
        if(mLoadedBundles.TryGetValue(path, out outBundle) && outBundle != null)
        {
            outBundle.Unload(unloadAllLoadedObjects);
            AppLog.d("UnloadBundle: {0}, {1}", path, unloadAllLoadedObjects);
        }
    }

    // TODO: not complete
    public static T WwwSync<T>(string url) where T : UnityEngine.Object
    {
        WWW www = new WWW(url);
        while(!www.isDone && string.IsNullOrEmpty(www.error))
        {
            Thread.Sleep(1000);
        }
        UnityEngine.Object obj = new DataObject(www.bytes);
        www.Dispose();
        return (T)obj;
    }

    /// <summary>
    /// 在新线程中异步保存文件
    /// </summary>
    public static void AsyncSave(string fname, byte[] bytes)
    {
        var dir = Path.GetDirectoryName(fname);
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream writer = new FileStream(fname, FileMode.Create);
        writer.BeginWrite(bytes, 0, bytes.Length, (IAsyncResult result) =>
        {
            FileStream stream = (FileStream)result.AsyncState;
            stream.EndWrite(result);
            stream.Close();
            stream.Dispose();
            AppLog.d("Saved:" + fname);
        }, writer);
    }
}
