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

    private void Awake()
    {

        NetworkSceneManager.OnSceneSwitched += SpawnPlayerServerRpc;
    }
    void Update()
    {
        if (!hasSpawned)
        {
            if (IsHost)
            {
                InstantiateServerRPC(NetworkManager.Singleton.LocalClientId);
            }
            if (IsClient)
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }


    [ServerRpc]
    private void InstantiateServerRPC(ulong id)
    {
        if (IsServer)
        {
            Debug.Log("Switching scenes");
            newPlayer = id;
            sceneSwitchProgress = NetworkSceneManager.SwitchScene("Game");
            hasSpawned = true;
        }
    }
    [ServerRpc]
    public void SpawnPlayerServerRpc()
    {
        Debug.Log("Completed");
        Instantiate<NetworkObject>(player, player.transform.position, Quaternion.identity).SpawnWithOwnership(newPlayer);

    }
}
