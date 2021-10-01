using System.Collections.Generic;
using UnityEngine;
public class NetworkPlayerManager : MonoBehaviour
{
    Dictionary<int, Player> players = new Dictionary<int, Player>();
    float playersTeam1 = 0;
    float playersTeam2 = 0;
    public GameObject AddPlayer(GameObject playerPrefab, int connId)
    {
        GameObject playerObject = Instantiate(playerPrefab);
        int team = playersTeam1 < GameParameters.Instance.PlayersPerTeam ? 1 : 2;
        Player player = new Player(playerObject, Player.PlayerType.Local, team);
        players.Add(connId, player);
        SetPlayerPosition(player);
        return playerObject;
    }

    public void SetPlayerPosition(Player p)
    {
        p.PlayerObject.transform.position = new Vector3(80, 14, 5);
    }
}
