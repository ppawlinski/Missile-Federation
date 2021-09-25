using System.Collections;
using UnityEngine;

public class CarDemoliition : MonoBehaviour
{
    Vector3 velocity;
    public Vector3 Velocity { get => velocity; }

    public delegate void Demo(GameObject sender); 
    public static event Demo OnDemo;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var other = collision.gameObject.GetComponent<CarDemoliition>();
            if (other.Velocity.z > 50)
            {
                StartCoroutine(GetComponent<CarParticleManager>().Explode());
                StartCoroutine(MoveAndDisable());
            }
        }
    }

    private void FixedUpdate()
    {
        velocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
    }

    IEnumerator MoveAndDisable()
    {
        OnDemo?.Invoke(gameObject);
        transform.position = Vector3.down * 100;
        yield return new WaitForFixedUpdate();
        gameObject.SetActive(false);
    }
}
