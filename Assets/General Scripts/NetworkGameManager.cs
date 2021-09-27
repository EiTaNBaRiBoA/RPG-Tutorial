using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkManager
{
    #region Serverside
    [Header("Server")]
    public NetworkObject player;
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (IsHost)
        {
            if (FindObjectsOfType<PlayerOnline>().Length == 1)
                Instantiate<NetworkObject>(player, player.transform.position, Quaternion.identity).SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }
}
