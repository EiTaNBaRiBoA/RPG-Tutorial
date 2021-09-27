using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Transports;


public class ServerInformation : NetworkBehaviour
{
    public Text serverName, ServerStatus;

    private void OnConnectedToServer()
    {
        NetworkManager.Singleton.StartClient();
    }


}