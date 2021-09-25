using System;
using UnityEngine;

public class CarAudioController : MonoBehaviour
{
    AudioManager audioManager;
    CarParametersSO parameters;

    private void Awake()
    {
        parameters = GetComponent<CarController>().parameters;
        audioManager = GetComponent<AudioManager>();
    }

    private void OnEnable()
    {
        GetComponent<CarJump>().JumpEvent += PlayJump;
        GetComponent<CarJump>().Flip += PlayFlip;
        GetComponent<CarBoost>().OnFailedBoostInit += PlayFailedBoostInit;
        GetComponent<CarJump>().DoubleJumpEvent += PlayDoubleJump;
    }

    private void OnDisable()
    {
        GetComponent<CarJump>().JumpEvent -= PlayJump;
        GetComponent<CarJump>().Flip -= PlayFlip; ;
        GetComponent<CarBoost>().OnFailedBoostInit -= PlayFailedBoostInit;
        GetComponent<CarJump>().DoubleJumpEvent -= PlayDoubleJump;
    }

    public void Start()
    {
        audioManager.Play("Engine", false);
    }
    public void Update()
    {
        if (GetComponent<CarBoost>().IsBoosting) audioManager.Play("Boost", false);
        else audioManager.Stop("Boost");

        if (GetComponent<CarWheels>().WheelsGrounded == 4)
            audioManager.ChangePitch("Engine", 0.4f + GetComponent<Rigidbody>().velocity.magnitude / 50 * 0.8f);
        else
            audioManager.ChangePitch("Engine", 0.6f);
        /*TODO if (carState.WheelsGrounded > wheelsGrounded && !carState.IsJumping && !carState.IsFlipping) audioManager.Play("WheelImpact", true);
        wheelsGrounded = carState.WheelsGrounded;*/
    }

    private void PlayJump(object sender, EventArgs e)
    {
        audioManager.Play("Jump", true);
    }
    private void PlayFlip(object sender, EventArgs e)
    {
        audioManager.Play("Flip", true);
    }
    private void PlayFailedBoostInit(object sender, EventArgs e)
    {
        audioManager.Play("BoostInit", true);
    }
    private void PlayDoubleJump(object sender, EventArgs e)
    {
        audioManager.Play("DoubleJump", true);
    }

    public void Pause(bool value)
    {
        if (value)
        {
            audioManager.Stop("Boost");
            audioManager.Stop("Engine");
        }
        else
        {
            audioManager.Play("Engine", false);
        }
    }
}
