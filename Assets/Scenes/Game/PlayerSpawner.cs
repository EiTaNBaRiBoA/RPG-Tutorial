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

        Debug.Log("Owner");
        if (!hasSpawned)
        {
            SpawnPlayerServerRpc(NetworkManager.LocalClientId);
        }
    }


    // Start is called before the first frame update
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong id)
    {
        if (IsServer)
        {
            Debug.Log("Completed");
            hasSpawned = true;
            Instantiate<NetworkObject>(player, transform.position, Quaternion.identity).SpawnWithOwnership(id);
        }

    }
}
