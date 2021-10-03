using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Messaging;
using System;

public class NetworkGameManager : NetworkManager
{
    private void Awake()
    {
        // NetworkSceneManager.OnSceneSwitched += () => FindObjectOfType<PlayerSpawner>().SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }
    public void ServerInit()
    {
        if (IsServer)
        {
            NetworkSceneManager.SwitchScene("Game");
        }
    }



}

#region serverSend
public class ServerSend : NetworkBehaviour
{
    public void RecievingConnection(ServerStatus serverStatus, ServerInformation.TCP tcp)
    {
        if (!IsServer || !IsHost) return;
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write((int)serverStatus);
            SendTCPData(packet, tcp);
        }
    }

    private void SendTCPData(Packet packet, ServerInformation.TCP tcp)
    {
        packet.WriteLength();
        tcp.SendData(packet);
    }
}
#endregion