using System.Collections.Generic;
using UnityEngine;
public class NetworkPlayerManager : MonoBehaviour
{
    //TODO check if a code running only on the server should be a mono or network behaviour

    Dictionary<int, Player> players = new Dictionary<int, Player>();
    int playersPerTeam = 1;
    int playersTeam1 = 0;
    int playersTeam2 = 0;

    List<int> chosenSpawnPoints;

    //TODO make it into separate object (SO?)
    readonly Vector3[] spawnPoints = {
        new Vector3(82.5f,14,6.5f),
        new Vector3(82.5f,14,-6.5f),
        new Vector3(97.5f,14,0),
        new Vector3(53,14,44),
        new Vector3(53,14,-44)
    };
    readonly Vector3[] respawnPoints =
    {
        new Vector3(95,12.9f, 56),
        new Vector3(95,12.9f, 48),
        new Vector3(95,12.9f, -56),
        new Vector3(95,12.9f, -48)
    };
    readonly Vector3 team2SpawnOffset = new Vector3(-1, 1, -1);
    readonly Vector3 team2RespawnOffset = new Vector3(-1, 1, 1);

    public void Initialize()
    {
        playersPerTeam = GameParameters.Instance.PlayersPerTeam;
        chosenSpawnPoints = ChooseSpawnPoints();
    }

    private List<int> ChooseSpawnPoints()
    {
        List<int> chosenPoints = new List<int>();
        int _temp;
        while (chosenPoints.Count < playersPerTeam)
        {
            _temp = UnityEngine.Random.Range(0, 5);
            if (!chosenPoints.Contains(_temp)) chosenPoints.Add(_temp);
        }
        return chosenPoints;
    }

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
        if (p.Team == 1)
        {
            p.PlayerObject.transform.position = spawnPoints[chosenSpawnPoints[playersTeam1]];
            p.PlayerObject.transform.eulerAngles = new Vector3(0, -90, 0);
            playersTeam1++;
        }
        else
        {
            p.PlayerObject.transform.position = Vector3.Scale(spawnPoints[chosenSpawnPoints[playersTeam2]], team2SpawnOffset);
            p.PlayerObject.transform.eulerAngles = new Vector3(0, 90, 0);
            playersTeam2++;
        }
    }
}
