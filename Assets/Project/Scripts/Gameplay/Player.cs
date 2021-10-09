using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    GameObject playerObject;
    int team;
    public string name;
    public int goals;
    public int assists;
    public int saves;
    public int touches;

    public int Team { get => team; set => team = value; }
    public GameObject PlayerObject { get => playerObject; }

    public Player(GameObject obj, int team, string name = "Marvin")
    {
        playerObject = obj;
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
