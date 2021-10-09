using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoost : MonoBehaviour
{
    //TODO disable it and make some event to wake up when localplayer is assigned
    CarBoost playerBoost;
    public void SetPlayerBoost(GameObject player)
    {
        playerBoost = player.GetComponent<CarBoost>();
    }

    void Update()
    {
        GetComponent<Text>().text = playerBoost.BoostPercentage.ToString();
    }
}
