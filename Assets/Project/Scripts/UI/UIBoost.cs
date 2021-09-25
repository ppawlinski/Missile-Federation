using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoost : MonoBehaviour
{
    [SerializeField] MatchManager matchManager; //TODO return to this once i do some more UI stuff and change the way i get a reference to matchmanager
    CarBoost playerBoost;
    private void Start()
    {
        //done in start - not awake - to prevent referencing unassigned PlayerObject
        playerBoost = matchManager.GetComponent<PlayerManager>().PlayerObject.GetComponent<CarBoost>();
    }
    void Update()
    {
        GetComponent<Text>().text = playerBoost.BoostPercentage.ToString();
    }
}
