using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParametersMenu : MonoBehaviour
{
    [SerializeField] Slider players;
    [SerializeField] Slider time;
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ApplyParameters()
    {
        GameParameters.Instance.PlayersPerTeam = Convert.ToInt32(players.value);
        GameParameters.Instance.GameTime = time.value * 30;
    }
}
