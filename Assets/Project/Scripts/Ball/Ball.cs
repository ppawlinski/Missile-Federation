using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float hitForce = 4000f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 2 * Mathf.PI;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector3 forceDirection = (transform.position - collision.gameObject.transform.position).normalized;
            rb.AddForce(hitForce * collision.relativeVelocity.magnitude * forceDirection, ForceMode.Force);
        }
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0, 14.2f, 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    /*public void SetRandomPosition()
    {
        //100, 80, 15
        //float _randomX = UnityEngine.Random.Range(-100f,-80f);
        //float _randomZ = UnityEngine.Random.Range(-15f, 15f);
        //transform.position = new Vector3(_randomX, 14.2f, _randomZ);
        Vector3 _temp = Vector3.zero;
        _temp.y = 12.94f;
        while (Mathf.Abs(_temp.x) < 2)
            _temp.x = UnityEngine.Random.Range(-77f, 77f);

        while (Mathf.Abs(_temp.z) < 2)
            _temp.z = UnityEngine.Random.Range(-57f, 57f);

        transform.position = _temp + containerObject.position;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }*/
}