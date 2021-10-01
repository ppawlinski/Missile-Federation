using Mirror;
using UnityEngine;

public class NetworkManagerMF : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = GetComponent<NetworkPlayerManager>().AddPlayer(playerPrefab, conn.connectionId);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
