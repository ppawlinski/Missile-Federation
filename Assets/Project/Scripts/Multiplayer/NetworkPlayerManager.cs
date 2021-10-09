using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerManager : NetworkBehaviour
{
    Dictionary<int, Player> players = new Dictionary<int, Player>();
    Dictionary<int, int> connectionToInstanceId = new Dictionary<int, int>();

    public delegate void PlayerAddedEventHandler(Player player);
    public static event PlayerAddedEventHandler PlayerAdded;
    public delegate void PlayerRemovedEventHandler(int instanceId);
    public static event PlayerRemovedEventHandler PlayerRemoved;

    int playersPerTeam = 1;
    int playersTeam1 = 0;
    int playersTeam2 = 0;
    int playersSetTeam1 = 0;
    int playersSetTeam2 = 0;

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

    public Dictionary<int, Player> Players { get => players; private set => players = value; }

    public void Initialize()
    {
        //don't have to check if isserver because only executed in OnStartServer
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

        // teams should be chosen in the lobby
        int team = playersTeam1 < GameParameters.Instance.PlayersPerTeam ? 1 : 2;
        if (team == 1) playersTeam1++;
        else if (team == 2) playersTeam2++;

        Player player = new Player(playerObject, team);
        players.Add(playerObject.GetInstanceID(), player);
        connectionToInstanceId.Add(connId, playerObject.GetInstanceID());
        SetPlayerPosition(player);
        PlayerAdded?.Invoke(player);
        return playerObject;
    }

    public void RemovePlayer(int connId)
    {
        if(!connectionToInstanceId.TryGetValue(connId, out int instanceId)) return;
        players.Remove(instanceId);
        connectionToInstanceId.Remove(connId);
        PlayerRemoved?.Invoke(instanceId);
    }

    public void SetPlayerPosition(Player p)
    {
        if (p.Team == 1)
        {
            p.PlayerObject.transform.position = spawnPoints[chosenSpawnPoints[playersSetTeam1]];
            p.PlayerObject.transform.eulerAngles = new Vector3(0, -90, 0);
            playersSetTeam1++;
        }
        else
        {
            p.PlayerObject.transform.position = Vector3.Scale(spawnPoints[chosenSpawnPoints[playersSetTeam2]], team2SpawnOffset);
            p.PlayerObject.transform.eulerAngles = new Vector3(0, 90, 0);
            playersSetTeam2++;
        }
    }

    public void ResetPositions()
    {
        playersSetTeam1 = playersSetTeam2 = 0;
        chosenSpawnPoints = ChooseSpawnPoints();
        foreach (KeyValuePair<int, Player> p in players)
        {
            SetPlayerPosition(p.Value);
        }
    }

    public void SetMovementBlock(bool value)
    {
        foreach (KeyValuePair<int, Player> p in players)
        {
            p.Value.PlayerObject.GetComponentInChildren<CarController>().SetFreeze(value);
        }
    }

    public Player GetPlayerFromInstanceId(int instanceId)
    {
        players.TryGetValue(instanceId, out Player p);
        return p;
    }
}
