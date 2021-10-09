using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    void Update()
    { 
        float timer = NetworkMatchManager.Instance.Timer;
        float seconds = Mathf.FloorToInt(timer % 60);
        GetComponent<Text>().text = Mathf.FloorToInt(timer / 60).ToString() + ":" + (seconds < 10 ? "0"+ seconds : seconds.ToString());
    }
}
