using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoostPad : NetworkBehaviour
{
    //TODO enable all boostpads when resetting after a goal
    [SerializeField] public float regenTime = 10f;
    [SerializeField] public int boostValue = 100;

    [ServerCallback]
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var carBoost = other.gameObject.GetComponent<CarBoost>();
            if (!carBoost.IsFullBoost)
            {
                RpcClientBoostGrab(carBoost);
            }
        }

    }

    [ClientRpc]
    private void RpcClientBoostGrab(CarBoost carBoost)
    {
        if (isServer) carBoost.AddBoost(boostValue);
        StartCoroutine(DisableBoost());
    }

    private IEnumerator DisableBoost()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(regenTime);
        GetComponent<Collider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
