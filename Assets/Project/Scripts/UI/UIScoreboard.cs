using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScoreboard : MonoBehaviour
{
    [SerializeField] GameObject matchManagerObject;
    [SerializeField] GameObject scoreboardRowPrefab;

    MatchStats statsManager;
    List<GameObject> team1ScoreboardRows = new List<GameObject>();
    List<GameObject> team2ScoreboardRows = new List<GameObject>();

    private void Awake()
    {
        statsManager = matchManagerObject.GetComponent<MatchStats>();
    }
    private void OnEnable()
    {
        statsManager.OnStatsUpdate += UpdateScoreboard;
    }
    private void OnDisable()
    {
        statsManager.OnStatsUpdate -= UpdateScoreboard;
    }

    private void Start()
    {
        InitScoreboard();
    }

    void InitScoreboard()
    {
        List<MatchStats.PlayerStats> statsList = statsManager.GetStats();
        int team1Count = 0;
        int team2Count = 0;

        foreach (MatchStats.PlayerStats stat in statsList)
        {
            if (stat.team == 1)
            {
                var row = Instantiate(scoreboardRowPrefab, transform.position, Quaternion.identity, transform);
                row.transform.position = row.transform.position + new Vector3(0, 100 - 50 * team1Count, 0);
                team1ScoreboardRows.Add(row);
                team1Count++;
            }
            if (stat.team == 2)
            {
                var row = Instantiate(scoreboardRowPrefab, transform.position, Quaternion.identity, transform);
                row.transform.position = row.transform.position + new Vector3(0, -100 - 50 * team2Count, 0);
                team2ScoreboardRows.Add(row);
                team2Count++;
            }
        }
        GetComponent<Canvas>().enabled = false;
        UpdateScoreboard(statsList);
    }

    private void UpdateScoreboard(List<MatchStats.PlayerStats> stats)
    {
        /*TODO change the way i loop through the stats. Now it depends on the number of
        rows per team being the same as the number of players per team which can possibly change in the future*/
        int i = 0;
        foreach (GameObject row in team1ScoreboardRows)
        {
            if (stats[i].team == 1)
            {
                row.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = stats[i].name;
                row.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = stats[i].Score.ToString();
                row.transform.Find("GoalsText").GetComponent<TextMeshProUGUI>().text = stats[i].goals.ToString();
                row.transform.Find("AssistsText").GetComponent<TextMeshProUGUI>().text = stats[i].assists.ToString();
                row.transform.Find("SavesText").GetComponent<TextMeshProUGUI>().text = stats[i].saves.ToString();
                i++;
            }
        }

        foreach (GameObject row in team2ScoreboardRows)
        {
            if (stats[i].team == 2)
            {
                row.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = stats[i].name;
                row.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = stats[i].Score.ToString();
                row.transform.Find("GoalsText").GetComponent<TextMeshProUGUI>().text = stats[i].goals.ToString();
                row.transform.Find("AssistsText").GetComponent<TextMeshProUGUI>().text = stats[i].assists.ToString();
                row.transform.Find("SavesText").GetComponent<TextMeshProUGUI>().text = stats[i].saves.ToString();
                i++;
            }
        }
    }
}
