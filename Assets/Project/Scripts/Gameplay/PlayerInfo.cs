using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInfo : NetworkBehaviour
{
    int team;
    public string playerName;
    public int goals = 0;
    public int assists = 0;
    public int saves = 0;
    public int touches = 0;

    public int Team { get => team; set => team = value; }

    public void SetValues(int team, string name)
    {
        this.team = team;
        playerName = name;
    }
}
