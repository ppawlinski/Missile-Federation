using System;
using UnityEngine.InputSystem;

public class PlayerControls : CarInput
{
    void OnDrive(InputValue value)
    {
        driveInput = value.Get<float>();
    }

    void OnSteer(InputValue value)
    {
        steerInput = value.Get<float>();
    }

    void OnJump(InputValue value)
    {
        jumpHoldInput = Convert.ToBoolean(value.Get<float>());
        jumpStartInput = jumpHoldInput;
    }

    void OnBoost(InputValue value)
    {
        boostInput = Convert.ToBoolean(value.Get<float>());
        boostStartInput = boostInput;
    }

    void OnAirRoll(InputValue value)
    {
        airRollInput = value.Get<float>();
    }

    void OnAirRight(InputValue value)
    {
        airRightInput = value.Get<float>();
    }

    void OnAirFront(InputValue value)
    {
        airFrontInput = value.Get<float>();
    }

    void OnPowerSlide(InputValue value)
    {
        powerSlideInput = Convert.ToBoolean(value.Get<float>());
    }
}
