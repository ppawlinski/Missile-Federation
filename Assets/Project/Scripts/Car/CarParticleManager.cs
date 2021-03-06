using System.Collections;
using UnityEngine;
using Mirror;

public class CarParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem boostParticles;
    [SerializeField] ParticleSystem boostParticles2;
    [SerializeField] ParticleSystem sparkParticles;
    [SerializeField] ParticleSystem explosionParticles;

    private bool isPlayingBoost = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) return;
        sparkParticles.Play();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) return;
        var contact = collision.GetContact(0);
        sparkParticles.transform.position = contact.point;
        sparkParticles.transform.eulerAngles = contact.normal;
    }
    private void OnCollisionExit(Collision collision)
    {
        sparkParticles.Stop();
    }

    private void Start()
    {
        boostParticles.Play();
        boostParticles2.Play();
    }

    private void Update()
    {
        /*ParticleSystem.EmissionModule em = boostParticles.emission;
        em.enabled = GetComponent<CarBoost>().IsBoosting; 
        em = boostParticles2.emission;
        em.enabled = GetComponent<CarBoost>().IsBoosting;*/
        if (GetComponent<CarBoost>().IsBoosting)
        {
            if (isPlayingBoost) return;
            boostParticles.Play();
            boostParticles2.Play();
            isPlayingBoost = true;
        }
        else
        {
            if (!isPlayingBoost) return;
            boostParticles.Stop();
            boostParticles2.Stop();
            isPlayingBoost = false;
        }
    }

    public IEnumerator Explode()
    {
        explosionParticles.Play();
        yield return new WaitForSeconds(4);
        explosionParticles.Stop();
        explosionParticles.Clear();
    }
}
