using System;
using UnityEngine.InputSystem;

public class PlayerControls : CarInput
{
    void OnDrive(InputValue value)
    {
        if (!isLocalPlayer) return;
        driveInput = value.Get<float>();
    }

    void OnSteer(InputValue value)
    {
        if (!isLocalPlayer) return;
        steerInput = value.Get<float>();
    }

    void OnJump(InputValue value)
    {
        if (!isLocalPlayer) return;
        jumpHoldInput = Convert.ToBoolean(value.Get<float>());
        jumpStartInput = jumpHoldInput;
    }

    void OnBoost(InputValue value)
    {
        if (!isLocalPlayer) return;
        boostInput = Convert.ToBoolean(value.Get<float>());
        boostStartInput = boostInput;
    }

    void OnAirRoll(InputValue value)
    {
        if (!isLocalPlayer) return;
        airRollInput = value.Get<float>();
    }

    void OnAirRight(InputValue value)
    {
        if (!isLocalPlayer) return;
        airRightInput = value.Get<float>();
    }

    void OnAirFront(InputValue value)
    {
        if (!isLocalPlayer) return;
        airFrontInput = value.Get<float>();
    }

    void OnPowerSlide(InputValue value)
    {
        if (!isLocalPlayer) return;
        powerSlideInput = Convert.ToBoolean(value.Get<float>());
    }
}
