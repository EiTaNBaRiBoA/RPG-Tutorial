using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Transports;
using MLAPI.SceneManagement;
using System.Net.Sockets;
using System.Net;
using System;
using MLAPI.Transports.UNET;

public enum ServerType { Host, Client }
public class ServerInformation : NetworkBehaviour
{
    public const string ipAddress = "127.0.0.1";
    public const int port = 7777;
    //Server Type of host or client
    public ServerType serverType;
    public TCP tcp;
    //ServerName
    public Text serverName, ServerStatus;
    public NetworkObject playerObject;
    public void OnConnectedToServer()
    {
        if (tcp.isOnline)
        {
            //Setting up port and ip for the server to join. Has to return a bool value
            bool successSetup = MLAPI.NetworkManager.Singleton.GetComponent<UNetTransport>() is
            {
                ConnectAddress: ipAddress,// Address needs to be a const variable
                ConnectPort: port // Port needs to be a const variable
            };
            if (successSetup)
                NetworkManager.Singleton.StartClient();
        }
    }
    public void HostServer()
    {
        NetworkManager.Singleton.StartHost();
        FindObjectOfType<NetworkGameManager>().ServerInit();

    }


    private void Start()
    {
        if (serverType == ServerType.Client)
            tcp = new TCP(ipAddress, port);

    }
    private void Update()
    {
        if (tcp != null && tcp.isOnline && ServerStatus.text != "Online")
        {
            ServerStatus.text = "Online";
        }
    }
    public class TCP
    {
        public bool isOnline = false;
        private string ipAddress;
        private int port;
        public TcpClient socket;
        public TCP(string _ipAddress, int _port)
        {
            ipAddress = _ipAddress;
            port = _port;
            socket = new TcpClient();
            CheckAvailability();
        }

        public void CheckAvailability()
        {
            socket.BeginConnect(ipAddress, port, ConnectCallBack, null);
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            if (!socket.Connected)
            {
                isOnline = false;
                return;
            }
            else
            {
                isOnline = true;
                //TODO Get Server Name
                //todo Check if hosted
                socket.Close();
            }
            socket.EndConnect(ar);
        }
    }
}
