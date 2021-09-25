using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderValueDisplay : MonoBehaviour
{
    public void OnValueChanged(float value)
    {
        GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
}
