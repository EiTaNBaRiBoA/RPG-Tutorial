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
            sceneSwitchProgress = NetworkSceneManager.SwitchScene("Game");
            hasSpawned = true;
            StartCoroutine(SpawnPlayer());
        }
    }
    public IEnumerator SpawnPlayer()
    {
        while (!sceneSwitchProgress.IsCompleted)
        {
            yield return null;
        }
        Instantiate<NetworkObject>(player, player.transform.position, Quaternion.identity).SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);

    }
}
