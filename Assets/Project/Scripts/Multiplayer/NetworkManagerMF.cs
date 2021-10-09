using Mirror;
using UnityEngine;

public class NetworkManagerMF : NetworkManager
{
    [SerializeField] GameObject matchManagerObject;
    NetworkMatchManager matchManager;
    NetworkPlayerManager playerManager;

    public override void Awake()
    {
        base.Awake();
        matchManager = matchManagerObject.GetComponent<NetworkMatchManager>();
        playerManager = matchManagerObject.GetComponent<NetworkPlayerManager>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        playerManager.Initialize();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = playerManager.AddPlayer(playerPrefab, conn.connectionId);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Player disconnected");
        playerManager.RemovePlayer(conn.connectionId);
    }
}
