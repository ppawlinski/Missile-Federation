using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShadow : MonoBehaviour
{
    [SerializeField] Transform ball;

    //TODO remove shadow when ball is gone
    void Update()
    {
        Vector3 position = ball.transform.position;
        position.y = transform.position.y;
        transform.position = position;
    }
}
