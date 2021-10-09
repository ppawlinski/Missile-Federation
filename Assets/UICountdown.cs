using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICountdown : MonoBehaviour
{
    public void SetText(int text)
    {
        GetComponent<Text>().text = text.ToString();
    }

    private void Update()
    {
        SetText(NetworkMatchManager.Instance.Countdown);
    }
}
