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

[Flags]
public enum ServerStatus { Online, Offline }
public class ServerInformation : NetworkBehaviour
{
    public ServerType serverType;

    public TCP tcp;
    //ServerName
    public Text serverName, serverStatuText;
    public NetworkObject playerObject;
    public void OnConnectedToServer()
    {
        if (tcp.isOnline)
        {
            //Setting up port and ip for the server to join. Has to return a bool value
            bool successSetup = MLAPI.NetworkManager.Singleton.GetComponent<UNetTransport>() is
            {
                ConnectAddress: ServerIpAddress,// Address needs to be a const variable
                ConnectPort: ServerPort // Port needs to be a const variable
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
        CheckForServer();

    }

    public void CheckForServer()
    {
        if (serverType == ServerType.Client)
            tcp = new TCP(ServerIpAddress, ServerPort);
    }

    // private void Update()
    // {
    //     if (tcp != null && tcp.isOnline && ServerStatus.text != "Online")
    //     {
    //         //ServerStatus.text = global::ServerStatus.Online.ToString();
    //         //todo recieve packet to string
    //     }
    // }

    public void ServerStatusUpdate(ServerStatus _serverStatus)
    {
        serverStatuText.text = _serverStatus.ToString();
    }






    public const string ServerIpAddress = "127.0.0.1";
    public const int ServerPort = 7777;
    //Server Type of host or client
    public class TCP : NetworkBehaviour
    {
        public int dataBufferSize = 4096;
        public bool isOnline = false;
        private string ipAddress;
        private int port;
        public TcpClient socket;
        private byte[] receiveBuffer;
        private Packet recievedDataToClient;
        private NetworkStream networkStream;
        public TCP(string _ipAddress, int _port)
        {
            ipAddress = _ipAddress;
            port = _port;
            socket = new TcpClient()
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize

            };
            receiveBuffer = new byte[dataBufferSize];
            CheckAvailability();
        }

        public void CheckAvailability()
        {
            socket.BeginConnect(ipAddress, port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            socket.EndConnect(ar);
            if (!socket.Connected)
            {
                isOnline = false;
                return;
            }
            else
            {
                networkStream = socket.GetStream();
                recievedDataToClient = new Packet();
                isOnline = true;

                if (IsServer || IsHost)
                {
                    FindObjectOfType<ServerSend>().RecievingConnection(global::ServerStatus.Online, this);
                }
                else
                {
                    networkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }

                //TODO Get Server Name
                //todo Check if hosted
                socket.Close();
            }
            socket.EndConnect(ar);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int _byteLength = networkStream.EndRead(ar);
                if (_byteLength <= 0)
                {
                    // TODO: disconnect
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);
                recievedDataToClient.Reset(HandleData(_data)); // The recieved bytes from receiveBuffer that came from the server is now being handled 
                RecieveServerStatus(recievedDataToClient);
                networkStream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // TODO: disconnect
            }
        }

        private void RecieveServerStatus(Packet recievedDataToClient)
        {
            ServerStatus serverStatus = (ServerStatus)recievedDataToClient.ReadInt();
            Debug.Log("Server is " + serverStatus);
        }

        private bool HandleData(byte[] data)
        {
            int _packetLength = 0;
            recievedDataToClient.SetBytes(data);
            if (recievedDataToClient.UnreadLength() >= 4)
            {
                _packetLength = recievedDataToClient.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= recievedDataToClient.UnreadLength())
            {
                byte[] _packetBytes = recievedDataToClient.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                    }
                });

                _packetLength = 0;
                if (recievedDataToClient.UnreadLength() >= 4)
                {
                    _packetLength = recievedDataToClient.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    networkStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch
            {
                Debug.Log("Client is offline");
            }
        }
    }
}