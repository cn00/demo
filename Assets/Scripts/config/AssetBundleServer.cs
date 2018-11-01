using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace AssetBundleServer
{
    [Serializable]
    public class Server
    {
        const string Tag = "AssetBundleServer";
        public int Port = 8008;
        
        [NonSerialized]
        public bool Runing = false;

        public string BasePath = "AssetBundle";

        public Thread thread = null;
        public void WatchDog(object processID)
        {
            Log("Watching parent processID: {0}!", processID);
            Process masterProcess = Process.GetProcessById((int)processID);
            while (masterProcess == null || !masterProcess.HasExited)
            {
                Thread.Sleep(1000);
            }

            Log("Exiting because parent process has exited!");
            Environment.Exit(0);
        }

        private async Task Listen(HttpListener l)
        {
            try
            {
                bool detailedLogging = false;
                var ctx = await l.GetContextAsync();
                WriteFile(ctx, BasePath, detailedLogging);
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("screw you guys, I'm going home!");
            }
        }
        public void Main()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://*:{0}/", Port));
            if (!listener.IsListening)
            {
                listener.Start();

                Task.Factory.StartNew(async () =>
                {
                    while (Runing)
                        await Listen(listener);
                    listener.Stop();
                }, TaskCreationOptions.LongRunning);
                AppLog.d(Tag, "Starting up asset bundle server.", Port);
                AppLog.d(Tag, "Port: {0}", Port);
                AppLog.d(Tag, "Directory: {0}", BasePath);

            }
        }

        static void WriteFile(HttpListenerContext ctx, string basePath, bool detailedLogging)
        {
            HttpListenerRequest request = ctx.Request;
            var qidx = request.RawUrl.IndexOf('?');
            string rawUrl = qidx < 0 ? request.RawUrl : request.RawUrl.Substring(0,qidx);

            // var parameters = qidx < 0 ? null : request.RawUrl.Substring( qidx + 1 ).Split('&').ToList();
            // var version = parameters.Find(i => i.StartsWith("version="));

            // if(string.IsNullOrEmpty(rawUrl) || rawUrl == "/")
            //     rawUrl = "index.html";
            string path = basePath + rawUrl;

            if (detailedLogging)
                Log("Requesting file: '{0}'. \nRelative url: {1} \nFull url: '{2}' \nAssetBundleDirectory: '{3}''"
                    , path, request.RawUrl, request.Url, basePath);
            else
                Log("Requesting file: '{0}' ... ", request.RawUrl);

            var response = ctx.Response;
            try
            {
                Stream fs = null;
                if(Directory.Exists(path))
                {
                    var index = string.Join("/\n", Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly));
                    index += "/\n";
                    index += string.Join("\n", Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly));
                    index += "\n";
                    index = index.Replace("//", "/").Replace(basePath, "");
                    fs = new MemoryStream(System.Text.Encoding.Default.GetBytes(index));
                }
                else if(File.Exists(path))
                {
                    fs = File.OpenRead(path);
                }
                if(fs != null)
                {
                    string filename = Path.GetFileName(path);
                    //response is HttpListenerContext.Response...
                    response.ContentLength64 = fs.Length;
                    response.SendChunked = false;
                    // response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    response.ContentEncoding = System.Text.Encoding.UTF8;
                    response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
                    response.AddHeader("Content-disposition", "attachment; filename=" + filename);

                    byte[] buffer = new byte[64 * 1024];
                    int read;
                    // using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                    {
                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            response.OutputStream.Write(buffer, 0, read);
                        }

                    }

                    Log(request.RemoteEndPoint.Address + ": " + rawUrl);
                    response.StatusDescription = "OK";

                    fs.Dispose();
                }
                else
                {
                    Error(rawUrl + "not found");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                response.OutputStream.Close();
                response.Close();
            }
            catch (System.Exception exc)
            {
                Error("Requested failed path: '{0}'. \nRawUrl: {1} \nUrl: '{2}' \nbasePath: '{3}' \nException: {4}: {5}:"
                    , path, request.RawUrl, request.Url, basePath, exc.GetType(), exc.Message);
                response.Abort();
            }
        }

        public void StartBtn()
        {
            if (thread != null && thread.IsAlive && Runing)
                thread.Abort();
            thread = new Thread(Main);
            Log("http thread: " + thread.ManagedThreadId);
            Runing = true;
            thread.Start();
        }
        public void StopBtn()
        {
            if (thread != null && thread.IsAlive && Runing)
                thread.Abort();
            thread = null;
            Runing = false;
        }

        static void Log(string fmt, params object[] args)
        {
            AppLog.d(Tag, fmt, args);
        }

        static void Error(string fmt, params object[] args)
        {
            AppLog.e(fmt, args);
        }

    }
}
#endif //UNITY_EDITOR
