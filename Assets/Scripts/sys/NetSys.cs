using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;

using Net;

/*
    // public class UdpSocket : Socket
    // {
    //     public UdpSocket(SocketInformation socketInformation)
    //     :base(socketInformation)
    //     {}

    //     public UdpSocket(SocketType socketType, ProtocolType protocolType)
    //     :base(socketType, protocolType)
    //     {}

    //     public UdpSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
    //     :base(addressFamily, socketType, protocolType)
    //     {}
    // }
*/

public class NetSys : SingleMono<NetSys>
{
    public const string Tag = "NetSys";
    int DebugListenPort = 7201;
    int GameListenPort = 9988;
    Server m_debugServer = null;


    Server m_gameServer = null;


    public override void Awake()
    {
        StartDebugTcpServer(DebugListenPort);
        StartGameUdpServer(GameListenPort);
    }

    // Use this for initialization
    void Start()
    {
        if(m_debugServer != null)
        {
            AppLog.OnMessageReceived += SendDebugMessage;
        }

        // var t = new System.Threading.Thread(ReceiveMessage);
        // t.Start();
    }

    // Update is called once per frame
    void Update()
    {

		// perframe msg

		// 1 second msg

		// 5 seconds msg

    }

    void OnDestroy()
    {
        AppLog.OnMessageReceived -= SendDebugMessage;
        if (m_debugServer != null)
            m_debugServer.Stop();
    }

    public void BroadcastAddressLoop()
    {
        var endpoint = new IPEndPoint(IPAddress.Broadcast, DebugListenPort);
        var ips = LocalIpAddressStr().Aggregate((i, j) => i.ToString() + "=" + j.ToString());
        SendGameMessage(null, endpoint);
    }

    public void StartGameUdpServer(int port)
    {
        var endpoint = new IPEndPoint(IPAddress.Any, port);
        var server = new Server(endpoint, SocketType.Dgram, ProtocolType.Udp);
        // server.GetSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        server.GetSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 500);
        server.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
        {
            // server.SendCacheMessage(e.GetClient());
            Debug.LogFormat(Tag+"Client {0} Connected", e.GetClient().GetGuid());
        };

        server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
        {
            Debug.LogFormat(Tag+"Client {0} Disconnected", e.GetClient().GetGuid());
        };

        server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
        {
            Debug.LogFormat(Tag+"Client {0} Message: {1}", e.GetClient().GetGuid(), e.GetMessage());
        };

        server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
        {
            //Debug.Log(Tag+"Sent message: '{0}' to client {1}", e.GetMessage(), e.GetClient().GetGuid());
        };

        m_gameServer = server;
    }

    public void SendGameMessage(byte[] data, EndPoint endPoint)
    {
        m_gameServer.GetSocket().SendTo(data, endPoint);
    }

    public void ReceiveDebugMessage()
    {
        while(true)
        {
            Debug.Log(Tag+" ******* ReceiveMessage ******* ");
            if (m_debugServer != null)
            {
                EndPoint endpoint = new IPEndPoint(IPAddress.Any, DebugListenPort);
                byte[] data = new byte[1024];
                var recv = m_debugServer.GetSocket().ReceiveFrom(data, ref endpoint);
                UnityEngine.Debug.Log("------- receive from udp: " + System.Text.Encoding.ASCII.GetString(data, 0, recv));
            }
            // System.Threading.Thread.Sleep(1);
        }
    }

    public void SendDebugMessage(object sender, AppLog.OnMessageReceivedHandler e)
    {
        // m_debugServer.BroadcastMessage(e.Message);
        // var endpoint = new IPEndPoint(IPAddress.Broadcast, DebugListenPort);
        // m_debugServer.GetSocket().SendTo(System.Text.Encoding.UTF8.GetBytes(e.Message), endpoint);
    }
    public void StartDebugTcpServer(int port)
    {
        var endpoint = new IPEndPoint(IPAddress.Any, port);
        // var server = new TcpServer.Server(endpoint, SocketType.Dgram, ProtocolType.Udp);
        var server = new Net.Server(endpoint);
        // server.GetSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        // server.GetSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
        server.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
		{
            server.SendCacheMessage(e.GetClient());
        };

        server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
        {
            Debug.LogFormat(Tag+"Client {0} Disconnected", e.GetClient().GetGuid());
        };

        server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
        {
            // server.GetSocket().SendTo(System.Text.Encoding.UTF8.GetBytes(e.GetMessage()), endpoint);
			server.BroadcastMessage(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:\n\t{1}", e.GetClient().GetGuid(), e.GetMessage())), e.GetClient());
        };

        server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
        {
			//Debug.Log(Tag+"Sent message: '{0}' to client {1}", e.GetMessage(), e.GetClient().GetGuid());
		};

        m_debugServer = server;

    }

    public static IPAddress[] LocalIpAddress()
    {
        var hostname = Dns.GetHostName();
        Debug.Log($"hostname:{hostname}");
        // if (!hostname.EndsWith(".local"))
        //     hostname = hostname + ".local";

        IPHostEntry ipHost = Dns.GetHostEntry(hostname);

        var s = ipHost.AddressList
            .Select(i => i.ToString())
            .Aggregate((i,j) => i.ToString() + "=" + j.ToString());
        Debug.LogFormat(Tag+"{0}: {1}", hostname, s);

        return ipHost.AddressList;
    }
    public static string[] LocalIpAddressStr()
    {
        return LocalIpAddress().Select(i => i.ToString()).ToArray();
    }

    public static List<string> LocalIpAddressStrList()
    {
        return LocalIpAddressStr().ToList();
    }

}