using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMatchStats : NetworkBehaviour
{
    Dictionary<int, PlayerInfo> players = new Dictionary<int, PlayerInfo>();
    Dictionary<int, float> lastTouchTimes = new Dictionary<int, float>();
    NetworkPlayerManager playerManager;

    int lastTouchTeam1;
    int previousTouchTeam1;

    int lastTouchTeam2;
    int previousTouchTeam2;

    public delegate void StatsUpdatedEventHandler(List<PlayerStats> stats);
    public event StatsUpdatedEventHandler StatsUpdated; 
    public delegate void PlayerStatsAddedEventHandler(PlayerInfo player);
    public event PlayerStatsAddedEventHandler PlayerStatsAdded;


    private void OnEnable()
    {
        NetworkPlayerManager.PlayerAdded += AddPlayer;
        NetworkPlayerManager.PlayerRemoved += RemovePlayer;
        BallTouchManager.BallTouched += BallTouchUpdate;
        GoalCheck.GoalScored += UpdateOnGoal;
    }
    public void OnDisable()
    {
        NetworkPlayerManager.PlayerAdded -= AddPlayer;
        NetworkPlayerManager.PlayerRemoved -= RemovePlayer;
        BallTouchManager.BallTouched -= BallTouchUpdate;
        GoalCheck.GoalScored -= UpdateOnGoal;
    }

    private void Awake()
    {
        playerManager = GetComponent<NetworkPlayerManager>();
    }
    private void AddPlayer(int instanceId)
    {
        Debug.Log("PlayerAdded");
        PlayerInfo pInfo = playerManager.GetPlayerFromInstanceId(instanceId);
        if (players.TryGetValue(pInfo.gameObject.GetInstanceID(), out _)) return;
        players.Add(pInfo.gameObject.GetInstanceID(), pInfo);
        lastTouchTimes.Add(pInfo.gameObject.GetInstanceID(), 0f);
        PlayerStatsAdded?.Invoke(pInfo);
    }

    private void RemovePlayer(int instanceId)
    {
        players.Remove(instanceId);
        lastTouchTimes.Remove(instanceId);
    }

    private void BallTouchUpdate(GameObject player, bool save)
    {
        int playerID = player.GetInstanceID();
        if (!players.TryGetValue(playerID, out PlayerInfo p)) return;
        if (p.Team == 1)
        {
            if (lastTouchTeam1 != playerID)
            {
                previousTouchTeam1 = lastTouchTeam1;
                lastTouchTeam1 = playerID;
            }
        }
        else
        {
            if (lastTouchTeam2 != playerID)
            {
                previousTouchTeam2 = lastTouchTeam2;
                lastTouchTeam2 = playerID;
            }
        }
        if (save) p.saves++;
        if(lastTouchTimes.TryGetValue(playerID, out float lastTouchTime))
        {
            if (Time.time - lastTouchTime >= 1)
            {
                p.touches++;
                lastTouchTimes[playerID] = Time.time;
            }
        }            

        StatsUpdated?.Invoke(GetStats());
    }

    void UpdateOnGoal(int goalId, int envId)
    {
        if (goalId == 1)
        {
            if (players.TryGetValue(lastTouchTeam2, out PlayerInfo p))
                p.goals++;
            if (players.TryGetValue(previousTouchTeam2, out p))
                p.assists++;

        }
        if (goalId == 2)
        {
            if (players.TryGetValue(lastTouchTeam1, out PlayerInfo p))
                p.goals++;
            if (players.TryGetValue(previousTouchTeam1, out p))
                p.assists++;
        }
        StatsUpdated?.Invoke(GetStats());
        ResetTouchTracking();
    }

    void ResetTouchTracking()
    {
        lastTouchTeam1 = 0;
        previousTouchTeam1 = 0;
        lastTouchTeam2 = 0;
        previousTouchTeam2 = 0;
    }

    public List<PlayerStats> GetStats()
    {
        PlayerStats stats;
        List<PlayerStats> statsList = new List<PlayerStats>();

        foreach (KeyValuePair<int, PlayerInfo> p in players)
        {
            stats.instanceId = p.Value.gameObject.GetInstanceID();
            stats.team = p.Value.Team;
            stats.name = p.Value.playerName;
            stats.goals = p.Value.goals;
            stats.assists = p.Value.assists;
            stats.saves = p.Value.saves;
            stats.touches = p.Value.touches;
            statsList.Add(stats);
        }
        return statsList;
    }

    public PlayerStats GetPlayerStats(int instanceId)
    {
        if (!players.TryGetValue(instanceId, out PlayerInfo player)) return default;
        PlayerStats stats;
        stats.instanceId = instanceId;
        stats.team = player.Team;
        stats.name = player.playerName;
        stats.goals = player.goals;
        stats.assists = player.assists;
        stats.saves = player.saves;
        stats.touches = player.touches;
        return stats;
    }
}
public struct PlayerStats
{
    public int instanceId;
    public int team;
    public string name;
    public int goals;
    public int assists;
    public int saves;
    public int touches;
    public int Score { get => goals * 100 + (assists + saves) * 50 + (touches * 2); }
    public override string ToString()
    {
        return name + " - " + goals + " - " + assists + " - " + saves + " - " + Score;
    }
}
