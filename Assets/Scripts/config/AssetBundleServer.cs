using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace AssetBundleServer
{
    [Serializable]
    public class Server : InspectorDraw
    {
        public int Port = 8008;
        public bool Runing = false;

        string BasePath = "./AssetBundle/";

        Thread thread = null;
        public static void WatchDog(object processID)
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

        public void Main()
        {
            bool detailedLogging = false;

            AppLog.d("Starting up asset bundle server.", Port);
            AppLog.d("Port: {0}", Port);
            AppLog.d("Directory: {0}", BasePath);

            HttpListener listener = new HttpListener();

            /*
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName ());
            foreach (IPAddress ip in host.AddressList)
            {
                //if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    UnityEngine.Debug.LogFormat(ip.AddressFamily.ToString() + " - " + ip.ToString());
                }
            }
            */


            listener.Prefixes.Add(string.Format("http://*:{0}/", Port));
            listener.Start();

            while (true)
            {
                UnityEngine.Debug.LogFormat("Waiting for request...");

                HttpListenerContext context = listener.GetContext();

                WriteFile(context, BasePath, detailedLogging);
                // Thread.Sleep(1000);
            }
        }

        static void WriteFile(HttpListenerContext ctx, string basePath, bool detailedLogging)
        {
            HttpListenerRequest request = ctx.Request;
            string rawUrl = request.RawUrl;
            string path = basePath + rawUrl;

            if (detailedLogging)
                UnityEngine.Debug.LogFormat("Requesting file: '{0}'. Relative url: {1} Full url: '{2} AssetBundleDirectory: '{3}''"
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

        public void Start()
        {
            if (thread != null && thread.IsAlive && Runing)
                thread.Abort();
            thread = new Thread(Main);
            thread.Start();
            Runing = true;
        }
        public void Stop()
        {
            if (thread != null && thread.IsAlive && Runing)
                thread.Abort();
            thread = null;
            Runing = false;
        }
        public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
        {
            // using (var verticalScope = new EditorGUILayout.VerticalScope("box"))
            {
                Runing = (thread != null && thread.IsAlive);
                var nodeStyle = new GUIStyle();
                nodeStyle.normal.textColor = (Runing ? Color.green : Color.red);
                nodeStyle.active.textColor = (Runing ? Color.green : Color.red);
                EditorGUILayout.LabelField("Running", Runing.ToString(), nodeStyle);
                Port = EditorGUILayout.IntField("port", Port);
                var rect = EditorGUILayout.GetControlRect();
                if (GUI.Button(rect.Split(0, 4), "Start") && !Runing)
                {
                    Start();
                }
                if (GUI.Button(rect.Split(1, 4), "Stop"))
                {
                    Stop();
                }
            }
        }
    }
}
#endif //UNITY_EDITOR
