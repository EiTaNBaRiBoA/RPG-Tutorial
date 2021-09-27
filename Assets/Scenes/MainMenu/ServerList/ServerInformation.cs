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

    private void Awake()
    {
        //NetworkSceneManager.OnSceneSwitched += OnConnectedToServer();
    }
    public void OnConnectedToServer()
    {
        NetworkManager.Singleton.StartHost();
        FindObjectOfType<NetworkGameManager>().player = playerObject;
        FindObjectOfType<NetworkGameManager>().sceneSwitchProgress = NetworkSceneManager.SwitchScene("Game");
    }


}