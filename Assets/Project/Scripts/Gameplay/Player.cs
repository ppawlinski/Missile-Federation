using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    GameObject playerObject;
    PlayerType type;
    int team;
    public string name;
    int score;
    public int goals;
    public int assists;
    public int saves;
    public int touches;

    public int Team { get => team; set => team = value; }
    public GameObject PlayerObject { get => playerObject; }


    public enum PlayerType
    {
        Local = 0,
        LocalAI = 1,
        Remote = 2
    }

    public Player(GameObject obj, PlayerType type, int team, string name = "Marvin")
    {
        playerObject = obj;
        this.type = type;
        this.team = team;
        this.name = name;
        goals = 0;
        assists = 0;
        saves = 0;
    }

    public void PauseAudio(bool value)
    {
        playerObject.GetComponent<CarAudioController>().Pause(value);
    }
}
