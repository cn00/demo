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
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;

#endif

public static class AssetExtern
{
    public static T LoadABAsset<T>(this AssetBundle bundle, string subPath) where T : UnityEngine.Object
    {
        var asset = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + subPath);
        Debug.Log($"{AssetSys.Tag}, {subPath}");
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
            Debug.LogErrorFormat(inFile + " not found");
            return;
        }

        //        var cmd = "7z a \"" + outFile + ".7z\" \"" + inFile + "\" -p123456";
        //        Debug.LogFormat("BundleHelper", cmd);
        //        p7zip_executeCommand(cmd);

        // if(true)return;

        var outDir = Path.GetDirectoryName(outFile);
        if (!Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        SevenZip.Compression.Lzma.Encoder coder = new SevenZip.Compression.Lzma.Encoder();
        FileStream inStream = new FileStream(inFile, FileMode.Open);
        FileStream outStream = new FileStream(outFile, FileMode.OpenOrCreate);
        outStream.Position = 0;

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
                var cacheDirName = "abcache/";
                #if UNITY_EDITOR
                cacheDirName += PlatformName() + "/";

                mCacheRoot = Application.dataPath + "/../" + cacheDirName;
                #else //!UNITY_EDITOR
                #if UNITY_ANDROID || UNITY_IPHONE
                mCacheRoot = Application.persistentDataPath + "/" + cacheDirName;
                #else // UNITY_WINDOWS
                mCacheRoot = Application.streamingAssetsPath + "/" + cacheDirName;
                #endif
                #endif
            }

            return mCacheRoot;
        }
    } // set in Runtime

    public static string PlatformName()
    {
        string name = PlatformName(Application.platform);;
        # if UNITY_IOS
        name = PlatformName(RuntimePlatform.IPhonePlayer);
        # elif UNITY_ANDROID
        name = PlatformName(RuntimePlatform.Android);
        # elif UNITY_STANDALONE_WIN
        name = PlatformName(RuntimePlatform.WindowsPlayer);
        # elif UNITY_OSX
        name = PlatformName(RuntimePlatform.OSXPlayer);
        # endif
        return name;
    }

    /// <summary>
    /// http://ip:port/path/to/root/platform/
    /// </summary>
    /// <value>The http root.</value>
    public static string WebRoot;

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
                return platform.ToString();
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
        var cfg = AudioSettings.GetConfiguration();
        cfg.dspBufferSize = 0;
        AudioSettings.Reset(cfg);

        if (!Directory.Exists(CacheRoot))
        {
            Directory.CreateDirectory(CacheRoot);
        }
        yield return base.Init();
    }

    IEnumerator LoadManifest()
    {
        if (mManifest == null)
        {
            string manifestBundleName = PlatformName();
            var old = AssetBundle.GetAllLoadedAssetBundles().ToList().Find(i => i.name == ""); //AssetBundleManifest);
            if(old)
            {
                Debug.Log( $"{Tag} unload AssetBundleManifest {old.name}:{old}");
                old.Unload(true);
            }

            mLoadedBundles.Remove(manifestBundleName);

            yield return GetBundle(manifestBundleName, (bundle) =>
            {
                mManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                UnityEngine.Debug.Log(Tag+": load manifest: " + manifestBundleName);
            });
        }
    }


    /// <summary>
    /// 同步方式加载资源, 用于加载少量小型资源
    /// </summary>
    public static T GetAssetSync<T>(string assetSubPath, bool autoDownload = false) where T : UnityEngine.Object
    {
        #if UNITY_EDITOR
        var UseBundle = BuildConfig.Instance().UseBundle;
        if (!UseBundle)
        {
            return AssetDatabase.LoadAssetAtPath<T>(BuildConfig.BundleResRoot + assetSubPath);
        }
        else
        #endif // UNITY_EDITOR
        {
            string bundleName = GetBundlePath(assetSubPath); // dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
            var bundle = GetBundleSync(bundleName, autoDownload);
            T asset = null;
            if (bundle != null)
            {
                asset = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + assetSubPath);
            }

            // try streaming asset
            if (asset == null)
            {
                UnityEngine.Debug.LogWarningFormat(Tag+": [{0}({2}):{1}] not find.", bundleName, BuildConfig.BundleResRoot + assetSubPath, bundle);
            }

            return asset;
        }
    }

    public static object GetStreamingAsset(string assetSubPath, bool isAssetBundle = false)// where T : object
    {
        var tstart = DateTime.Now;
        UnityWebRequest www;
        if (isAssetBundle)
        {
            string bundleName = GetBundlePath(assetSubPath);
            www = UnityWebRequestAssetBundle.GetAssetBundle("file://" + Application.streamingAssetsPath + "/" + bundleName);
            www.SendWebRequest();
            while (!www.isDone)
            {
                Thread.Sleep(100);
            }
            var bundle = AssetBundle.LoadFromMemory(www.downloadHandler.data);
            return bundle.LoadAsset<UnityEngine.Object>(assetSubPath);
        }

        object asset = null;
        #if UNITY_ANDROID && !UNITY_EDITOR
        {
            var fullpath = Application.streamingAssetsPath + "/" + assetSubPath;
            Debug.Log($"GetStreamingAsset:{fullpath}");
            www = UnityWebRequest.Get(fullpath);
            www.timeout = 9;
            www.SendWebRequest();
            while (!www.isDone) // && !www.isNetworkError) //(DateTime.Now - tstart).Seconds > Math.Min(120, www.timeout))
            {
                Thread.Sleep(100);
            }

            asset = www.downloadHandler.text;
        }
        #else
        if (File.Exists(Application.streamingAssetsPath + "/" + assetSubPath))
        {
            asset = File.ReadAllText(Application.streamingAssetsPath + "/" + assetSubPath);
            return asset;
        }
        #endif
        return asset;
    }

    public static string GetBundlePath(string assetSubPath)
    {
        var dirs = assetSubPath.Split('/');
        if (dirs.Length < 2)
        {
            UnityEngine.Debug.LogWarning(Tag+": bundle not found for: " + assetSubPath);
            return null;
        }

        string bundleName = dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;

        // bundleName = mManifest.GetAllAssetBundles().First(i => i.StartsWith(bundleName));
        return bundleName;
    }

    /// <summary>
    /// 异步方式加载资源, 以加载后的 (Object)res 为参数调用 callBack
    /// </summary>
    public static IEnumerator GetAsset(string assetSubPath, Action<UnityEngine.Object> callBack = null)
    {
        yield return GetAsset<UnityEngine.Object>(assetSubPath, callBack);
    }

    public static IEnumerator GetAsset<T>(string assetSubPath, Action<T> callBack = null) where T : UnityEngine.Object
    {
        T resObj = null; //default(T);

        #if UNITY_EDITOR
        var UseBundle = BuildConfig.Instance().UseBundle;
        if (!UseBundle)
        {
            resObj = AssetDatabase.LoadAssetAtPath<T>(BuildConfig.BundleResRoot + assetSubPath);
        }
        else
        #endif // UNITY_EDITOR
        {
            string bundleName = GetBundlePath(assetSubPath).ToLower(); // dirs[0] + '/' + dirs[1] + BuildConfig.BundlePostfix;
            yield return GetBundle(bundleName, (bundle) =>
            {
                UnityEngine.Debug.Log(Tag+": from bundle: " + assetSubPath);
                resObj = bundle.LoadAsset<T>(BuildConfig.BundleResRoot + assetSubPath);
                UnityEngine.Debug.LogFormat(Tag+": {0}:{1}", assetSubPath, resObj?.GetType());
            });
        }
        if (callBack != null)
            callBack(resObj);
    }

    public static UnityEngine.Object GetAssetSync(string assetPath)
    {
        return GetAssetSync<UnityEngine.Object>(assetPath);
    }

    public static AssetBundle GetBundleSync(string bundlePath, bool autoDownload = false)
    {
        if (string.IsNullOrEmpty(bundlePath))
        {
            UnityEngine.Debug.LogWarningFormat(Tag+": bundlePath [{0}] not correct.", bundlePath);
            return null;
        }

        bundlePath = bundlePath.ToLower();

        AssetBundle bundle = null;
        if (!Instance.mLoadedBundles.TryGetValue(bundlePath, out bundle))
        {
            var cachePath = CacheRoot + bundlePath;
            if (!File.Exists(cachePath) && autoDownload)
            {
                var dl = Download(bundlePath);
                while (dl.MoveNext()) ;
            }

            if (File.Exists(cachePath))
                bundle = AssetBundle.LoadFromFile(cachePath);

            if (bundle == null) // try Resources.Load
            {
                bundle = Resources.Load<AssetBundle>(bundlePath);
            }

            UnityEngine.Debug.LogFormat(Tag+": GetBundleSync: {0}", bundlePath);
        }

        if (bundle != null)
        {
            Instance.mLoadedBundles[bundlePath] = bundle;
        }
        else
        {
            UnityEngine.Debug.LogWarningFormat(Tag+ "[{0}] did not download yet.", bundlePath);
        }
        return bundle;
    }

    /// <summary>
    /// AssetBundle 加载, 自动处理更新和依赖,
    /// 以加载后的 AssetBundle 为参数调用 callBack
    /// </summary>
    public static IEnumerator GetBundle(string bundlePath, Action<UnityEngine.AssetBundle> callBack = null)
    {
        if (string.IsNullOrEmpty(bundlePath))
        {
            UnityEngine.Debug.LogWarningFormat(Tag+": bundlePath [{0}] not correct.", bundlePath);
            yield break;
        }

        if(bundlePath != PlatformName() && Instance.mManifest == null) // Manifest it self
            yield return Instance.LoadManifest();

        // Dependencies
        if (Instance.mManifest != null) // 加载 manifest 时本身为空
        {
            UnityEngine.Debug.LogFormat(Tag+": GetAllDependencies for {0}", bundlePath);
            var deps = Instance.mManifest.GetAllDependencies(bundlePath);
            foreach (var i in deps)
            {
                UnityEngine.Debug.LogFormat(Tag+": Dependencies: {0} +> {1}", bundlePath, i);
                yield return GetBundle(i);
            }
        }

        AssetBundle bundle = null;
        if (Instance.mLoadedBundles.TryGetValue(bundlePath, out bundle))
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
        var fi = new FileInfo(cachePath);
        if (!fi.Exists || fi.Length == 0 || needUpdate)
        {
            isLocal = false;
            // fileUrl = WebRoot + PlatformName() + "/" + version + "/" + bundlePath + BuildConfig.CompressedExtension;
            fileUrl = WebRoot + PlatformName() + "/" + bundlePath;// + BuildConfig.CompressedExtension;
        }

        UnityEngine.Debug.Log($"{Tag}: fileUrl={fileUrl} => cachePath:{cachePath}");

        var cachDir = cachePath.Substring(0, cachePath.LastIndexOf('/'));
        cachDir.CreateDir();

        if (!isLocal)
        {
            // var lzmapath = cachePath + BuildConfig.CompressedExtension;
            yield return Download(fileUrl, cachePath);
            // FileStream lzmaStream = new FileStream(lzmapath, FileMode.Open);
            // var thread = new Thread(() =>
            // {
            //     var outStream = new FileStream(cachePath, FileMode.OpenOrCreate);
            //     BundleHelper.DecompressFileLZMA(lzmaStream, outStream);
            //     outStream.Close();
            // });
            // thread.Start();
            // while (thread.IsAlive)
            // {
            //     UnityEngine.Debug.LogFormat(Tag+": 解压中。。。{0}", fileUrl);
            //     yield return new WaitForSeconds(0.3f);
            // }
            // lzmaStream.Close();

            if (AssetSys.Instance.IsLoaded(bundlePath))
            {
                UnloadBundle(bundlePath);
            }
        }
        var outStream = new FileStream(cachePath, FileMode.Open);
        Instance.mLoadedBundles[bundlePath] = AssetBundle.LoadFromStream(outStream);
        outStream.Close();


        if (callBack != null)
            callBack(Instance.mLoadedBundles[bundlePath]);

        yield return null;
    }


    public static IEnumerator Download(string url, string path = null, Action<float> processingcb = null)
    {
        if (path == null)
            path = Path.GetTempFileName().upath();
        if (!url.StartsWith("http"))
        {
            // var version = BuildConfig.Instance().Version.ToString();
            url = WebRoot + PlatformName() + "/" + url;
        }

        var cachDir = path.Substring(0, path.LastIndexOf('/'));
        cachDir.CreateDir();

        if(File.Exists(path))
            File.Delete(path);

        var webRequest = UnityWebRequest.Get(url);
        webRequest.downloadHandler = new DownloadHandlerFile(path);
        // yield return
            webRequest.SendWebRequest();
        while (!webRequest.isDone)
        {
            if (processingcb != null)
                processingcb(webRequest.downloadProgress);
            yield return new WaitForSeconds(0.3f);
        }

        // var tmpPath = path + ".tmp";

        // System.IO.Stream responseStream = null;
        // System.Net.WebHeaderCollection heads = null;
        // System.IO.FileStream temfs = null;
        // long contentLength = 0;

            // var webRequest = System.Net.HttpWebRequest.Create(url) as HttpWebRequest;
            // webRequest.Timeout = TimeoutMillisecond;
        // try
        // {
        //
        //     // webRequest.KeepAlive = true; // default = true
        //
        //     //打开上次下载的文件或新建文件
        //     temfs = new System.IO.FileStream(tmpPath, System.IO.FileMode.OpenOrCreate);
        //     var startPos = temfs.Seek(temfs.Length, SeekOrigin.Current);
        //     UnityEngine.Debug.LogFormat(Tag+": download: {0} +{1}", url, temfs.Length);
        //
        //     //webRequest.AllowReadStreamBuffering = true; // wrong
        //     if (temfs.Length > 0)
        //     {
        //         webRequest.AddRange((int) temfs.Length); //设置Range值
        //     }
        //
        //     //https://blog.csdn.net/u011966339/article/details/72829891
        //     var response = webRequest.GetResponse() as HttpWebResponse;
        //     heads = response.Headers;
        //     contentLength = response.ContentLength;
        //
        //     responseStream = response.GetResponseStream();
        // }
        // catch (WebException we)
        // {
        //     temfs.Close();
        //     File.Delete(tmpPath);
        //     Debug.LogError($"{Tag} url:{url}, msg:{we.Message}, dump:{we.Dump()}");
        //     DialogSys.Alert(url + "\n" + we.Message + we.StackTrace, "error");
        //     yield break;
        // }
        // catch (Exception e)
        // {
        //     Debug.Log($"{e.Message} {e.StackTrace}");
        //     if (temfs!=null){
        //         temfs.Close();
        //         File.Delete(tmpPath);
        //     }
        //     yield break;
        // }
        //
        // int bufsize = 10 * 1024 * 1024; // 10M
        //
        // var totalLength = temfs.Length + contentLength;
        // byte[] buffer = new byte[bufsize];
        // int readSize = responseStream.Read(buffer, 0, bufsize);
        // double downloadedLength = temfs.Length;
        // var thread = new Thread(() =>
        // {
        //     while (totalLength > downloadedLength)
        //     {
        //         temfs.Write(buffer, 0, readSize);
        //         downloadedLength = temfs.Length;
        //
        //         readSize = responseStream.Read(buffer, 0, bufsize);
        //     }
        // });
        // thread.Start();
        //
        // while (thread.IsAlive)
        // {
        //     var msg = string.Format(" {0:0.00}/{1:0.00}M [{2}]"
        //                               , 1f*downloadedLength / (1024 * 1024)
        //                               , 1f*totalLength      / (1024 * 1024)
        //                               , readSize);
        //     LuaSys.Instance.GlobalEnv.Global.Set("LoadingString", msg);
        //     LuaSys.Instance.GlobalEnv.Global.Set("LoadingValue", 1.0f * downloadedLength / totalLength);
        //     if(processingcb != null)
        //         processingcb(msg);
        //     yield return new WaitForSeconds(0.5f);
        // }
        //
        // temfs.Flush(true);
        // UnityEngine.Debug.LogFormat(Tag+": download ok {0}:{1}", url, totalLength);
        //
        // cb?.Invoke(temfs);
        // temfs.Close();
        //
        // //下载完成重命名
        // // File.Move(path + ".tmp", path);
        // File.WriteAllBytes(path, File.ReadAllBytes(tmpPath));
        // File.Delete((tmpPath));

        // UpdateCachInfo(path, heads.Get("ETag"), contentLength);

        // yield return null;
    }

    static void UpdateCachInfo(string path, string etag, long length)
    {
        // TODO: update download info: etag content-length
        var dbPath = CacheRoot + "db.db";
        IntPtr db;
        var errno = SQLite.SQLite3.Open(dbPath, out db);
        UnityEngine.Debug.LogFormat(Tag+": open db: {0}", errno);
        var sql = string.Format("insert into cache_info (id, path, etag, length) VALUES ( last_insert_rowid(), '{0}',  '{1}' , '{2}' ) on conflict(path) do update set etag = excluded.etag, length=excluded.length;", path, etag, length);
        errno = SQLite.SQLite3.Exec(db, sql);
        UnityEngine.Debug.LogFormat(Tag+": {0}:{1}", errno, sql);
        SQLite.SQLite3.Close(db);
    }

    public static bool IsUrlAvailable(string url)
    {
        var b = false;
        try
        {
            var webRequest = System.Net.HttpWebRequest.Create(url) as HttpWebRequest;
            webRequest.Timeout = TimeoutMillisecond;
            webRequest.Method = "HEAD";
            var response = webRequest.GetResponse() as HttpWebResponse;

            // var heads = response.Headers;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
                b = true;
        }
        catch (WebException e)
        {
            UnityEngine.Debug.Log($"{url}: {e}");
        }

        return b;
    }

    public static int TimeOutSeconds = 3600 * 0 + 60 * 0 + 5;
    public static int TimeoutMillisecond = 1000 * 5;

    public static void UnloadBundle(string path, bool unloadAllLoadedObjects = false)
    {
        AssetBundle outBundle = null;
        if (Instance.mLoadedBundles.TryGetValue(path, out outBundle) && outBundle != null)
        {
            outBundle.Unload(unloadAllLoadedObjects);
            Instance.mLoadedBundles.Remove(path);
            UnityEngine.Debug.LogFormat(Tag+": UnloadBundle: {0}, {1}", path, unloadAllLoadedObjects);
        }
    }

    /// <summary>
    /// 在新线程中异步保存文件
    /// </summary>
    public static void AsyncSave(string fpath, byte[] bytes)
    {
        var dir = Path.GetDirectoryName(fpath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FileStream writer = new FileStream(fpath, FileMode.OpenOrCreate);
        writer.BeginWrite(bytes, 0, bytes.Length, (IAsyncResult result) =>
        {
            FileStream stream = (FileStream) result.AsyncState;
            stream.EndWrite(result);
            stream.Close();
            stream.Dispose();
            UnityEngine.Debug.Log(Tag+": Saved:" + fpath);
        }, writer);
    }

    public static void TrimBom(ref byte[] textBytes)
    {
        if(textBytes[0] == 0xef)
        {
            textBytes[0] = (byte)' ';
            textBytes[1] = (byte)' ';
            textBytes[2] = (byte)' ';
        }

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