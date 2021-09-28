using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    bool hasSpawned = false;
    public NetworkObject player;


    private void Start()
    {

        if (!hasSpawned)
        {
            SpawnPlayerServerRpc(NetworkManager.LocalClientId);
        }
    }


    // I don't want to spawn the player when he is connected but when he switches to the game map
    /// <summary>
    /// This method means that i don't require only the owner/HOST of the server to call serverRPC but instead everyone
    /// can call it but only the server can execute it
    /// </summary>
    /// <param name="id">the client id that will own the gameobject</param>
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong id)
    {
        if (IsServer)
        {
            hasSpawned = true;
            Instantiate<NetworkObject>(player, transform.position, Quaternion.identity).SpawnAsPlayerObject(id);
        }

    }
}
