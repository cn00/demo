using System;
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
        public int Port = 8008;
        public bool Runing = false;

        public string BasePath = "./AssetBundle/";

        public Thread thread = null;
        public void WatchDog(object processID)
        {
            UnityEngine.Debug.LogFormat("Watching parent processID: {0}!", processID);
            Process masterProcess = Process.GetProcessById((int)processID);
            while (masterProcess == null || !masterProcess.HasExited)
            {
                Thread.Sleep(1000);
            }

            UnityEngine.Debug.LogFormat("Exiting because parent process has exited!");
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
                AppLog.d("Starting up asset bundle server.", Port);
                AppLog.d("Port: {0}", Port);
                AppLog.d("Directory: {0}", BasePath);

            }
        }

        static void WriteFile(HttpListenerContext ctx, string basePath, bool detailedLogging)
        {
            HttpListenerRequest request = ctx.Request;
            string rawUrl = request.RawUrl;
            if(string.IsNullOrEmpty(rawUrl) || rawUrl == "/")
                rawUrl = "index.html";
            string path = basePath + rawUrl;

            if (detailedLogging)
                UnityEngine.Debug.LogFormat("Requesting file: '{0}'. \nRelative url: {1} \nFull url: '{2}' \nAssetBundleDirectory: '{3}''"
                    , path, request.RawUrl, request.Url, basePath);
            else
                Console.Write("Requesting file: '{0}' ... ", request.RawUrl);

            var response = ctx.Response;
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    string filename = Path.GetFileName(path);
                    //response is HttpListenerContext.Response...
                    response.ContentLength64 = fs.Length;
                    response.SendChunked = false;
                    response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    response.AddHeader("Content-disposition", "attachment; filename=" + filename);

                    byte[] buffer = new byte[64 * 1024];
                    int read;
                    using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                    {
                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, read);
                            bw.Flush(); //seems to have no effect
                        }

                        bw.Close();
                    }

                    UnityEngine.Debug.LogFormat("completed: " + rawUrl);
                    //				response.StatusCode = (int)HttpStatusCode.OK;
                    //				response.StatusDescription = "OK";
                    response.OutputStream.Close();
                    response.Close();
                }
            }
            catch (System.Exception exc)
            {
                UnityEngine.Debug.LogFormat(" failed.");
                UnityEngine.Debug.LogFormat("Requested file failed: '{0}'. Relative url: {1} Full url: '{2} AssetBundleDirectory: '{3}''", path, request.RawUrl, request.Url, basePath);
                UnityEngine.Debug.LogFormat("Exception {0}: {1}'", exc.GetType(), exc.Message);
                response.Abort();
            }
        }

        public void StartBtn()
        {
            if (thread != null && thread.IsAlive && Runing)
                thread.Abort();
            thread = new Thread(Main);
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

    }
}
#endif //UNITY_EDITOR
