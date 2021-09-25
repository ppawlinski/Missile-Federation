using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameters : MonoBehaviour
{
    int playersPerTeam=2;
    float gameTime=300;
    private static GameParameters instance;

    public static GameParameters Instance { get => instance; }
    public int PlayersPerTeam { get => playersPerTeam; set => playersPerTeam = value; }
    public float GameTime { get => gameTime; set => gameTime = value; }



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
