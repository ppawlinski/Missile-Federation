using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderValueDisplayTime : MonoBehaviour
{    public void OnValueChanged(float value)
    {
        int minutes = Mathf.FloorToInt(value / 2);
        string seconds = value % 2 == 1 ? "30" : "00";
        GetComponent<TextMeshProUGUI>().text = minutes + ":" + seconds;
    }
}
