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
    private ulong newPlayer;
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!hasSpawned)
        {
            if (IsHost && IsClient)
            {
                InstantiateServerRPC(NetworkManager.Singleton.LocalClientId);
            }
        }
    }


    [ServerRpc]
    private void InstantiateServerRPC(ulong id)
    {
        if (IsServer)
        {
            newPlayer = id;
            NetworkSceneManager.OnSceneSwitched += SpawnPlayer;
            sceneSwitchProgress = NetworkSceneManager.SwitchScene("Game");
            hasSpawned = true;
        }
    }
    public void SpawnPlayer()
    {
        Debug.Log("Completed");
        Instantiate<NetworkObject>(player, player.transform.position, Quaternion.identity).SpawnWithOwnership(NetworkManager.Singleton.LocalClientId);

    }
}
