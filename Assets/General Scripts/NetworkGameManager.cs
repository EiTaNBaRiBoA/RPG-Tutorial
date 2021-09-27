using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Messaging;

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
            Debug.Log("Switching scenes");
            NetworkSceneManager.SwitchScene("Game");
        }
    }

}
