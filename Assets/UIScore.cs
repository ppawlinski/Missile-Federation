using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    private void OnEnable()
    {
        NetworkMatchManager.ScoreUpdated += UpdateScore;
    }
    private void OnDisable()
    {
        NetworkMatchManager.ScoreUpdated -= UpdateScore;
    }

    void UpdateScore(int team1, int team2)
    {
        GetComponent<Text>().text = team1 + " - " + team2;
    }
}
