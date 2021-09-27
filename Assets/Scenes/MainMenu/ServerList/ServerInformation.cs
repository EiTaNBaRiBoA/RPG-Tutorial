using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Transports;
using MLAPI.SceneManagement;

public class ServerInformation : NetworkBehaviour
{
    public Text serverName, ServerStatus;
    public NetworkObject playerObject;
    public void OnConnectedToServer()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void HostServer()
    {
        NetworkManager.Singleton.StartHost();
        FindObjectOfType<NetworkGameManager>().ServerInit();

    }


}