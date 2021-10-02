using Mirror;
using UnityEngine;

//TODO make some kind of interface to integrate single with multiplayer without making separate class for each functionality
public class NetworkManagerMF : NetworkManager
{
    NetworkPlayerManager playerManager;

    public override void Awake()
    {
        base.Awake();
        playerManager = GetComponent<NetworkPlayerManager>();
    }
    public override void OnStartServer()
    {
        playerManager.Initialize();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = playerManager.AddPlayer(playerPrefab, conn.connectionId);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

}
