using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScoreboard : MonoBehaviour
{
    [SerializeField] GameObject matchManagerObject;
    [SerializeField] GameObject scoreboardRowPrefab;

    NetworkMatchStats matchStats;
    Dictionary<int, GameObject> rows = new Dictionary<int, GameObject>();

    int team1Rows = 0;
    int team2Rows = 0;

    private void Awake()
    {
        matchStats = matchManagerObject.GetComponent<NetworkMatchStats>();
    }
    private void OnEnable()
    {
        matchStats.PlayerStatsAdded += AddPlayerRow;
        NetworkPlayerManager.PlayerRemoved += RemovePlayerRow;
        matchStats.StatsUpdated += UpdateScoreboard;
    }
    private void OnDisable()
    {
        matchStats.PlayerStatsAdded -= AddPlayerRow;
        NetworkPlayerManager.PlayerRemoved -= RemovePlayerRow;
        matchStats.StatsUpdated -= UpdateScoreboard;
    }

    void AddPlayerRow(Player player)
    {
        var row = Instantiate(scoreboardRowPrefab, transform.position, Quaternion.identity, transform);
        rows.Add(player.PlayerObject.GetInstanceID(), row);

        if (player.Team == 1)
        {
            row.transform.position = row.transform.position + new Vector3(0, 100 - 50 * team1Rows, 0);
            team1Rows++;
        }
        if (player.Team == 2)
        {
            row.transform.position = row.transform.position + new Vector3(0, -100 - 50 * team2Rows, 0);
            team1Rows++;
        }
        InitializeRowValues(row, player.PlayerObject.GetInstanceID());
    }

    void RemovePlayerRow(int instanceId)
    {
        if (rows.ContainsKey(instanceId))
        {
            rows.TryGetValue(instanceId, out GameObject row);
            Destroy(row);
            rows.Remove(instanceId);
        }
    }

    void InitializeRowValues(GameObject row, int instanceId)
    {
        NetworkMatchStats.PlayerStats stats = matchStats.GetPlayerStats(instanceId);
        row.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = stats.name;
        row.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = stats.Score.ToString();
        row.transform.Find("GoalsText").GetComponent<TextMeshProUGUI>().text = stats.goals.ToString();
        row.transform.Find("AssistsText").GetComponent<TextMeshProUGUI>().text = stats.assists.ToString();
        row.transform.Find("SavesText").GetComponent<TextMeshProUGUI>().text = stats.saves.ToString();
    }

    private void UpdateScoreboard(List<NetworkMatchStats.PlayerStats> stats)
    {
        foreach(NetworkMatchStats.PlayerStats s in stats)
        {
            if (!rows.TryGetValue(s.instanceId, out GameObject row)) continue;
            row.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = s.Score.ToString();
            row.transform.Find("GoalsText").GetComponent<TextMeshProUGUI>().text = s.goals.ToString();
            row.transform.Find("AssistsText").GetComponent<TextMeshProUGUI>().text = s.assists.ToString();
            row.transform.Find("SavesText").GetComponent<TextMeshProUGUI>().text = s.saves.ToString();
        }
    }
}
