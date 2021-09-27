using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Messaging;

public class NetworkGameManager : NetworkManager
{
    #region Serverside
    [Header("Server")]
    public NetworkObject player;
    private bool hasSpawned = false;
    public SceneSwitchProgress sceneSwitchProgress = null;
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (IsHost && !hasSpawned)
        {
            if (sceneSwitchProgress == null)
            {
                sceneSwitchProgress = NetworkSceneManager.SwitchScene("Game");

            }
        }
        if (!IsHost && !hasSpawned && IsConnectedClient)
        {
            if (sceneSwitchProgress == null)
                InstantiateServerRPC();
        }

        if (sceneSwitchProgress != null && sceneSwitchProgress.IsCompleted && !hasSpawned)
        {
            hasSpawned = true;
            Instantiate<NetworkObject>(player, player.transform.position, Quaternion.identity).SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }


    [ServerRpc]
    private void InstantiateServerRPC()
    {
        sceneSwitchProgress = NetworkSceneManager.SwitchScene("Game");
    }
}
