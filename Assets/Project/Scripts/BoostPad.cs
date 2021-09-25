using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    //TODO enable all boostpads when resetting after a goal
    [SerializeField] public float regenTime = 10f;
    [SerializeField] public int boostValue = 100;

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var carBoost = other.gameObject.GetComponent<CarBoost>();
            if (!carBoost.IsFullBoost)
            {
                carBoost.AddBoost(boostValue);
                StartCoroutine(DisableBoost());
            }
        }

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
