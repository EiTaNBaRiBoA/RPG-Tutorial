using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshServerInfo : MonoBehaviour
{
    ServerInformation[] serversinfo;
    private void OnEnable()
    {
        serversinfo = FindObjectsOfType<ServerInformation>();
    }
    public void onRefresh()
    {
        if (serversinfo != null)
            foreach (var server in serversinfo)
            {
                server.CheckForServer();
            }
    }
}
