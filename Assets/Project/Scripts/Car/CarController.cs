using UnityEngine;
using Mirror;

[RequireComponent(typeof(CarWheels), typeof(CarBoost), typeof(CarJump))]
[RequireComponent(typeof(CarFlip), typeof(CarAerialMovement))]
public class CarController : NetworkBehaviour
{
    //TODO
    //flip cancels
    //ballcam
    //powerslides - adjust forward, fix backward
    //limit max velocity (ball & car)
    //adjust steering
    //FIX downforce when landing on the ball    

    [SerializeField] public CarParametersSO parameters;


 

    Rigidbody rb;   

    public bool IsMovementBlocked { get; set; }

    void OnEnable()
    {
        //GoalCheck.OnGoalExplosion += ApplyGoalExplosionForce;
    }
    private void OnDisable()
    {
        //GoalCheck.OnGoalExplosion -= ApplyGoalExplosionForce;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void SetFreeze(bool value)
    {
        IsMovementBlocked = value;
        rb.angularVelocity = Vector3.zero;

        rb.constraints = value ? 
            RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ : 
            RigidbodyConstraints.None;
    }

    /*

        public void PauseAudio(bool value)
        {
            audioController.Pause(value);
        }


        public void ApplyGoalExplosionForce(Vector3 position)
        {
            car.Rb.AddForce((car.Transform.position - position).normalized * 80, ForceMode.VelocityChange);
        }*/
}

