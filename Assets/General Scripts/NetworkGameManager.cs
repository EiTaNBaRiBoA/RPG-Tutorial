using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;

public class NetworkGameManager : NetworkManager
{
    #region Serverside
    [Header("Server")]
    public NetworkObject player;
    private bool hasSpawned = false;
    public SceneSwitchProgress sceneSwitchProgress;
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (IsHost && !hasSpawned && sceneSwitchProgress.IsCompleted)
        {
            Instantiate<NetworkObject>(player, player.transform.position, Quaternion.identity).SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
            hasSpawned = true;
        }
    }
}
