using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

public class NetworkPlayerManager : NetworkBehaviour
{
    SyncDictionary<int, PlayerInfo> players = new SyncDictionary<int, PlayerInfo>();
    Dictionary<int, int> connectionToInstanceId = new Dictionary<int, int>();
    Dictionary<uint, int> netIdToInstanceId = new Dictionary<uint, int>();

    public delegate void PlayerAddedEventHandler(int connectionId);
    public static event PlayerAddedEventHandler PlayerAdded;
    public delegate void PlayerRemovedEventHandler(int instanceId);
    public static event PlayerRemovedEventHandler PlayerRemoved;

    int playersPerTeam = 2;

    [SyncVar (hook = nameof(PlayerCountChanged))]
    int playersTeam1 = 0;
    [SyncVar(hook = nameof(PlayerCountChanged))]
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

    public SyncDictionary<int, PlayerInfo> Players { get => players; private set => players = value; }

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

        //teams should be chosen in the lobby
        int team = playersTeam1 < GameParameters.Instance.PlayersPerTeam ? 1 : 2;
        if (team == 1) playersTeam1++;
        else if (team == 2) playersTeam2++;

        playerObject.GetComponent<PlayerInfo>().SetValues(team, $"{ playerPrefab.name}[connId={ connId}]");

        players.Add(playerObject.GetInstanceID(), playerObject.GetComponent<PlayerInfo>());
        connectionToInstanceId.Add(connId, playerObject.GetInstanceID());
        RpcInvokePlayerAdded(playerObject.GetInstanceID());

        SetPlayerPosition(playerObject);
        return playerObject;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isClientOnly)
        {
            foreach(PlayerInfo p in FindObjectsOfType<PlayerInfo>())
            {
                players.Add(p.gameObject.GetInstanceID(),p);
            }
        }
        foreach(KeyValuePair<int, PlayerInfo> p in players)
        {
            PlayerAdded?.Invoke(p.Value.GetInstanceID());
        }
    }

    [ClientRpc]
    private void RpcInvokePlayerAdded(int instanceId)
    {
        PlayerAdded?.Invoke(instanceId);
    }

    public void RemovePlayer(int connId)
    {
        if(!connectionToInstanceId.TryGetValue(connId, out int instanceId)) return;
        players.Remove(instanceId);
        connectionToInstanceId.Remove(connId);
        PlayerRemoved?.Invoke(instanceId);
        RpcInvokePlayerRemoved(instanceId);
    }

    [ClientRpc]
    private void RpcInvokePlayerRemoved(int instanceId)
    {
        PlayerRemoved?.Invoke(instanceId);
    }

    public void SetPlayerPosition(GameObject p)
    {
        Debug.Log(p);
        if (p.GetComponent<PlayerInfo>().Team == 1)
        {
            p.transform.position = spawnPoints[chosenSpawnPoints[playersSetTeam1]];
            p.transform.eulerAngles = new Vector3(0, -90, 0);
            playersSetTeam1++;
        }
        else
        {
            p.transform.position = Vector3.Scale(spawnPoints[chosenSpawnPoints[playersSetTeam2]], team2SpawnOffset);
            p.transform.eulerAngles = new Vector3(0, 90, 0);
            playersSetTeam2++;
        }
    }

    public void ResetPositions()
    {
        playersSetTeam1 = playersSetTeam2 = 0;
        chosenSpawnPoints = ChooseSpawnPoints();
        foreach (KeyValuePair<int, PlayerInfo> p in players)
        {
            SetPlayerPosition(p.Value.gameObject);
        }
    }

    public void SetMovementBlock(bool value)
    {
        foreach (KeyValuePair<int, PlayerInfo> p in players)
        {
            p.Value.GetComponent<CarController>().SetFreeze(value);
        }
    }

    public PlayerInfo GetPlayerFromInstanceId(int instanceId)
    {
        players.TryGetValue(instanceId, out PlayerInfo p);
        return p;
    }

    public PlayerInfo GetPlayerFromNetId(uint netId)
    {
        netIdToInstanceId.TryGetValue(netId, out int instanceId);
        players.TryGetValue(instanceId, out PlayerInfo p);
        return p;
    }

    public void PlayerCountChanged(int oldValue, int newVlaue)
    {
        Debug.Log($"Player count changed from {oldValue} to {newVlaue}");
    }
}
