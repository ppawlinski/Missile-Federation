using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    [SerializeField] GameObject boostCounter;
    [SerializeField] GameObject countdownText;

    private void OnEnable()
    {
        CarController.OnLocalPlayerStart += EnableBoostCounter;
        NetworkMatchManager.CountDownStarted += EnableCountdown;
        NetworkMatchManager.CountDownEnded += DisableCountdown;
    }

    private void OnDisable()
    {
        CarController.OnLocalPlayerStart -= EnableBoostCounter;
        NetworkMatchManager.CountDownStarted -= EnableCountdown;
        NetworkMatchManager.CountDownEnded -= DisableCountdown;
    }

    void EnableBoostCounter(GameObject player)
    {
        boostCounter.GetComponent<UIBoost>().SetPlayerBoost(player);
        boostCounter.SetActive(true);
    }

    void EnableCountdown(int time)
    {
        countdownText.GetComponent<UICountdown>().SetText(time);
        countdownText.SetActive(true);
    }
    void DisableCountdown()
    {
        countdownText.SetActive(false);
    }
}
