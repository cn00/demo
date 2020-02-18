using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using XLua;
using XLua.LuaDLL;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;

#endif

public static class AssetExtern
{
    public static T LoadABAsset<T>(this AssetBundle bundle, string subPath) where T : UnityEngine.Object
    {
        var asset = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + subPath);
        AppLog.d(AssetSys.Tag, subPath);
        return asset;
    }

    public static T GetXml<T>(this AssetBundle bundle, string path)
    {
        var stream = bundle.LoadAsset<TextAsset>(path);
        var deserializer = new XmlSerializer(typeof(T));
        var xml = (T) deserializer.Deserialize(new MemoryStream(stream.bytes));
        return xml;
    }

    public static T GetXml<T>(this TextAsset text)
    {
        var deserializer = new XmlSerializer(typeof(T));
        var xml = (T) deserializer.Deserialize(new MemoryStream(text.bytes));
        return xml;
    }
}

public static class BundleHelper
{
    [DllImport(Lua.LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int p7zip_executeCommand(string cmd);

    public static string Tag = "BundleHelper";

    #region 压缩

    #region LZMA

    public const int kPropSize = SevenZip.Compression.Lzma.Encoder.kPropSize;

    public class CProgressInfo : SevenZip.ICodeProgress
    {
        public Int64 ApprovedStart;
        public Int64 InSize;
        public System.DateTime Time;

        public void Init()
        {
            InSize = 0;
        }

        public void SetProgress(Int64 inSize, Int64 outSize)
        {
            if (inSize >= ApprovedStart && InSize == 0)
            {
                Time = DateTime.UtcNow;
                InSize = inSize;
            }
        }
    }

    public static void CompressFileLZMA(string inFile, string outFile)
    {
        if (!File.Exists(inFile))
        {
            AppLog.e(Tag, inFile + " not found");
            return;
        }

//        var cmd = "7z a \"" + outFile + ".7z\" \"" + inFile + "\" -p123456";
//        AppLog.d("BundleHelper", cmd);
//        p7zip_executeCommand(cmd);

        // if(true)return;

        var outDir = Path.GetDirectoryName(outFile);
        if (!Directory.Exists(outDir))
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

public class AssetSys : SingleMono<AssetSys>
{
    public const string Tag = "AssetSys";
    static string mCacheRoot = "";

    /// <summary>
    /// Application.dataPath + "/AssetBundle/${PlatformName}/" 
    /// </summary>
    public static string CacheRoot
    {
        get
        {
            if (string.IsNullOrEmpty(mCacheRoot))
            {
                var cacheDirName = "ab/";
#if UNITY_EDITOR
# if UNITY_IOS
                cacheDirName += PlatformName(RuntimePlatform.IPhonePlayer) + "/";
# elif UNITY_ANDROID
                cacheDirName += PlatformName(RuntimePlatform.Android) + "/";
# else
                cacheDirName += PlatformName(Application.platform) + "/";
# endif
                mCacheRoot = Application.dataPath + "/../" + cacheDirName;
#else //!UNITY_EDITOR
#   if UNITY_ANDROID
                mCacheRoot = Application.persistentDataPath + "/" + cacheDirName;
#   elif UNITY_IPHONE
                mCacheRoot = Application.persistentDataPath + "/" + cacheDirName;
#   else // UNITY_WINDOWS
                mCacheRoot = Application.streamingAssetsPath + "/" + cacheDirName;
#   endif
#endif
            }

            return mCacheRoot;
        }
    } // set in Runtime

    /// <summary>
    /// http://ip:port/path/to/root/platform/
    /// </summary>
    /// <value>The http root.</value>
    public static string HttpRoot;

    public static string PlatformName(RuntimePlatform platform)
    {
        switch (platform)
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

    public AssetBundleManifest Manifest
    {
        get { return mManifest; }
    }

    Dictionary<string, AssetBundle> mLoadedBundles = new Dictionary<string, AssetBundle>();

    public Dictionary<string, AssetBundle> LoadedBundles
    {
        get { return mLoadedBundles; }
    }

    public bool IsLoaded(string bundleName)
    {
        return mLoadedBundles.Keys.Contains(bundleName) && mLoadedBundles[bundleName] != null;
    }

    public override IEnumerator Init()
    {
        if (!Directory.Exists(CacheRoot))
        {
            Directory.CreateDirectory(CacheRoot);
        }
#if UNITY_EDITOR
        if (BuildConfig.Instance().UseBundle)
#endif
        {
            if (mManifest == null)
            {
#if UNITY_EDITOR
                string manifestBundleName =
                    BuildScript.TargetName(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
                string manifestBundleName = PlatformName(Application.platform);
#endif
                yield return GetBundle(manifestBundleName, (bundle) =>
                {
                    var manifext = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    mManifest = manifext;
                    AppLog.d(Tag, "load manifest: " + manifestBundleName);
                });
            }

            yield return GetBundle("ui/boot.bd");
        }

        yield return base.Init();
    }

    /// <summary>
    /// 同步方式加载资源, 用于加载少量小型资源
    /// </summary>
    public T GetAssetSync<T>(string assetSubPath) where T : UnityEngine.Object
    {
        // var trim = new char[] { ' ', '.', '/' };
        // assetSubPath = assetSubPath.upath().TrimStart(trim).TrimEnd(trim);
        string bundleName = GetBundlePath(assetSubPath); // dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
        var bundle = GetBundleSync(bundleName);
        T asset = null;
        if (bundle != null)
        {
            asset = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + assetSubPath);
        }

        // if(asset == null)
        // {
        //     AppLog.w(Tag, "[{0}({2}):{1}] not find.", bundleName, BuildConfig.BundleResRoot + assetSubPath, bundle);
        // }
        return asset;
    }

    public string GetBundlePath(string assetSubPath)
    {
        var dirs = assetSubPath.Split('/');
        if (dirs.Length < 2)
        {
            AppLog.w(Tag, "bundle not found for: " + assetSubPath);
            return null;
        }

        string bundleName = dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
        // bundleName = mManifest.GetAllAssetBundles().First(i => i.StartsWith(bundleName));
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
        T resObj = null; //default(T);
#if UNITY_EDITOR
        if (BuildConfig.Instance().UseBundle)
#endif
        {
            string bundleName = GetBundlePath(assetSubPath); // dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
            AppLog.d(Tag, "from bundle: " + assetSubPath);
            yield return GetBundle(bundleName, (bundle) =>
            {
                // text
                if (assetSubPath.IsText())
                {
                    var textPath = assetSubPath;
                    if (textPath.EndsWith(".lua"))
                        textPath += ".txt";
                    resObj = bundle.LoadAsset<TextAsset>(BuildConfig.BundleResRoot + textPath) as T;
                    if (resObj == null)
                    {
                        AppLog.e(Tag, textPath + " not found.");
                    }
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
            resObj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(BuildConfig.BundleResRoot + assetSubPath);
        }
#endif
        AppLog.d(Tag, "{0}:{1}", assetSubPath, resObj?.GetType());
        if (callBack != null)
            callBack(resObj);
    }

    public AssetBundle GetBundleSync(string bundlePath)
    {
        if (string.IsNullOrEmpty(bundlePath))
        {
            AppLog.w(Tag, "bundlePath [{0}] not correct.", bundlePath);
            return null;
        }

        AssetBundle bundle = null;
        if (!mLoadedBundles.TryGetValue(bundlePath, out bundle))
        {
            var cachePath = CacheRoot + bundlePath;
            if (File.Exists(cachePath))
                bundle = AssetBundle.LoadFromFile(cachePath);
            if (bundle == null) // try Resources.Load
            {
                bundle = Resources.Load<AssetBundle>(bundlePath);
            }

            AppLog.d(Tag, "GetBundleSync: {0}", bundlePath);
        }

        if (bundle != null)
        {
            mLoadedBundles[bundlePath] = bundle;
        }

        // else
        // {
        //     AppLog.w(Tag, "[{0}] did not download yet.", bundlePath);
        // }
        return bundle;
    }

    /// <summary>
    /// AssetBundle 加载, 自动处理更新和依赖, 
    /// 以加载后的 AssetBundle 为参数调用 callBack 
    /// </summary>
    public IEnumerator GetBundle(string bundleName, Action<UnityEngine.AssetBundle> callBack = null)
    {
        string bundlePath = bundleName;
        if (string.IsNullOrEmpty(bundlePath))
        {
            AppLog.w(Tag, "bundlePath [{0}] not correct.", bundlePath);
            yield break;
        }

        AssetBundle bundle = null;
        if (mLoadedBundles.TryGetValue(bundlePath, out bundle))
        {
            if (callBack != null)
                callBack(bundle);
            yield break;
        }

        var version = BuildConfig.Instance().Version.ToString();
        var cachePath = CacheRoot + bundlePath;
        var fileUrl = "file://" + cachePath;        

        var isLocal = true;
        var needUpdate = UpdateSys.Instance.NeedUpdate(bundlePath);
        if (!File.Exists(cachePath)
            || needUpdate
        )
        {
            isLocal = false;
            fileUrl = HttpRoot + version + "/" + bundlePath + BuildConfig.CompressedExtension;
        }

        AppLog.d(Tag, fileUrl);
        bool err = false;

        {
            if (isLocal)
            {
                WWW www = new WWW(fileUrl);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                    mLoadedBundles[bundlePath] = www.assetBundle;
                www.Dispose();
            }
            else
            /*
            {
                var bytes = www.bytes;
#if UNITY_EDITOR
                AsyncSave(cachePath + BuildConfig.CompressedExtension, bytes); //保留.lzma
#endif
                MemoryStream outStream = new MemoryStream();
                var thread = new Thread(() =>
                {
                    BundleHelper.DecompressFileLZMA(new MemoryStream(bytes), outStream);

                    AsyncSave(cachePath, outStream.GetBuffer());

                    lock (mLoadedBundles)
                    {
                        if (IsLoaded(bundlePath))
                        {
                            UnloadBundle(bundlePath);
                        }
                    }
                });
                thread.Start();
                while (thread.IsAlive)
                {
                    AppLog.d(Tag, "子线程 {0} 工作中。。。", fileUrl);
                    yield return new WaitForSeconds(0.3f);
                }

                AppLog.d(Tag, "子线程完成 {0}", fileUrl);
                // TODO: decode buffer
                mLoadedBundles[bundlePath] = AssetBundle.LoadFromMemory(outStream.GetBuffer());
            }
            */
            {
                
                FileStream lzmaStream = null;
                yield return Download(fileUrl, cachePath + BuildConfig.CompressedExtension, fs => {
                    lzmaStream = fs;
                });
                var outStream = new FileStream(cachePath, FileMode.Create);
                var thread = new Thread(() =>
                {
                    BundleHelper.DecompressFileLZMA(lzmaStream, outStream);
    
                    lock (mLoadedBundles)
                    {
                        if (IsLoaded(bundlePath))
                        {
                            UnloadBundle(bundlePath);
                        }
                    }
                });
                thread.Start();
                while (thread.IsAlive)
                {
                    AppLog.d(Tag, "子线程解压中。。。{0}", fileUrl);
                    yield return new WaitForSeconds(0.3f);
                }
                
                mLoadedBundles[bundlePath] = AssetBundle.LoadFromStream(outStream);
                lzmaStream.Close();
                outStream.Close();
            }
        }


        if (err)
            yield break;

        // Dependencies
        if (mManifest != null) // 加载 manifest 时本身为空
        {
            var deps = mManifest.GetAllDependencies(bundlePath);
            foreach (var i in deps)
            {
                AppLog.d(Tag, "Dependencies: {0} +> {1}", bundlePath, i);
                yield return GetBundle(i);
            }
        }

        if (callBack != null)
            callBack(mLoadedBundles[bundlePath]);

        yield return null;
    }

    public static IEnumerator Download(string url, string path)
    {
        yield return Download(url, path, fs =>
        {
            fs.Close();
                      
            //下载完成重命名
            File.Move(path + ".tmp", path);
        });
    }


    public static IEnumerator Download(string url, string path, Action<FileStream> cb)
    {
        var cachePath = path + ".tmp";
        var cachDir = cachePath.Substring(0, cachePath.LastIndexOf('/'));
        cachDir.CreateDir();

        HttpWebRequest webRequest = System.Net.HttpWebRequest.Create(url) as HttpWebRequest;
        webRequest.Timeout = 1000*60*10;
        //webRequest.AllowReadStreamBuffering = true;

        long countLength = webRequest.GetResponse().ContentLength;

        //打开上次下载的文件或新建文件 
        System.IO.FileStream fs = new System.IO.FileStream(cachePath, System.IO.FileMode.OpenOrCreate);
        var startPos = fs.Seek(0, SeekOrigin.End);
        AppLog.d(Tag, "skip:{0}, {1}", fs.Length, startPos);

        if (fs.Length > 0)
        {
            webRequest.AddRange((int) fs.Length); //设置Range值
        }

        System.Net.WebResponse res = webRequest.GetResponse();
        System.IO.Stream ns = res.GetResponseStream();
        int bufsize = 10 * 1024 * 1024; // 10M

        byte[] buffer = new byte[bufsize];
        int readSize = ns.Read(buffer, 0, bufsize);
        double downloadedLength = fs.Length;
        while (countLength > downloadedLength)
        {
            fs.Write(buffer, 0, readSize);
            fs.Flush(true);
            downloadedLength = fs.Length ;
            AppLog.d(Tag, string.Format(" {0:F}M / {1:F}M [{2}] {3}"
                , downloadedLength * 1.0 / (1024 * 1024)
                , countLength * 1.0 / (1024 * 1024), readSize, url));
            
            readSize = ns.Read(buffer, 0, bufsize);
            yield return null;
        }
        AppLog.d(Tag, "download {0}:{1}",countLength, downloadedLength);

//        webRequest.BeginGetRequestStream( (IAsyncResult result) =>{
//            FileStream stream = (FileStream) result.AsyncState;
//            stream.EndWrite(result);
//            stream.Close();
//            stream.Dispose();
//        },fs);
        cb(fs);
        
        yield return null;
    }
    
    public static int TimeOutSeconds = 3600 * 0 + 60 * 0 + 5;

    public static IEnumerator Www(string url, UnityAction<WWW> endCallback = null,
        UnityAction<float> progressCallback = null)
    {
        WWW www = new WWW(url);
        DateTime timeout = DateTime.Now +
                           new TimeSpan(TimeOutSeconds / 3600, (TimeOutSeconds % 3600) / 60, TimeOutSeconds % 60);
        if (www != null)
        {
            while (!www.isDone && string.IsNullOrEmpty(www.error))
            {
                //yield return www;
                if (progressCallback != null)
                {
                    progressCallback(www.progress);
                    if (DateTime.Now > timeout && www.progress < 0.1f)
                    {
                        AppLog.d(Tag, "timeout: " + url);
                        break;
                    }
                }

                yield return null;
            }

            if (www.progress >= 1 && string.IsNullOrEmpty(www.error))
            {
                AppLog.d(Tag, "loaded {0} OK {1}", url, www.progress);
            }
            else
            {
                AppLog.e(Tag, "{0}: {1}", www.error, url);
            }

            if (endCallback != null)
            {
                // 留给调用者选择是否存盘
                //if(url.Substring(0, 7) == "http://")
                //{
                //    AsyncSave(url.Replace(HttpRoot + "/" + CGameRoot.Instance.Version, CacheRoot), www.bytes);
                //}
                endCallback(www);
            }
        }
//        www.Dispose();

        yield return null;
    }

    public void UnloadBundle(string path, bool unloadAllLoadedObjects = false)
    {
        AssetBundle outBundle = null;
        if (mLoadedBundles.TryGetValue(path, out outBundle) && outBundle != null)
        {
            outBundle.Unload(unloadAllLoadedObjects);
            mLoadedBundles.Remove(path);
            AppLog.d(Tag, "UnloadBundle: {0}, {1}", path, unloadAllLoadedObjects);
        }
    }

    // TODO: not complete
    public static WWW WwwSync<T>(string url) where T : UnityEngine.Object
    {
        WWW www = new WWW(url);
        while (!www.isDone && string.IsNullOrEmpty(www.error))
        {
            Thread.Sleep(1000);
        }

        return www;
    }

    /// <summary>
    /// 在新线程中异步保存文件
    /// </summary>
    public static void AsyncSave(string fname, byte[] bytes)
    {
        AppLog.d(Tag, fname);
        var dir = Path.GetDirectoryName(fname);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream writer = new FileStream(fname, FileMode.OpenOrCreate);
        writer.BeginWrite(bytes, 0, bytes.Length, (IAsyncResult result) =>
        {
            FileStream stream = (FileStream) result.AsyncState;
            stream.EndWrite(result);
            stream.Close();
            stream.Dispose();
            AppLog.d(Tag, "Saved:" + fname);
        }, writer);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(AssetSys))]
public class AssetSysEditor : Editor
{
    AssetSys Target = null;

    public void OnEnable()
    {
        Target = (AssetSys) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var foldout = Inspector.tmpFoldout["LoadedBundles"];
        Inspector.DrwaDic("LoadedBundles", Target.LoadedBundles, ref foldout);
        Inspector.tmpFoldout["LoadedBundles"] = foldout;
    }
}
#endif