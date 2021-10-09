using System;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine;

public class PlayerControls : CarInput
{
    void OnDrive(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnDrive(value.Get<float>());
    }

    [Command]
    void CmdOnDrive(float value)
    {
        driveInput = value;
    }

    void OnSteer(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnSteer(value.Get<float>());
    }

    [Command]
    void CmdOnSteer(float value)
    {
        steerInput = value;
    }

    void OnJump(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnJump(Convert.ToBoolean(value.Get<float>()));
    }

    [Command]
    void CmdOnJump(bool value)
    {
        jumpHoldInput = value;
        jumpStartInput = jumpHoldInput;
    }

    void OnBoost(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnBoost(Convert.ToBoolean(value.Get<float>()));
    }

    [Command]
    void CmdOnBoost(bool value)
    {
        boostInput = value;
        boostStartInput = boostInput;
    }

    void OnAirRoll(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnAirRoll(value.Get<float>());
    }

    [Command]
    void CmdOnAirRoll(float value)
    {
        airRollInput = value;
    }

    void OnAirRight(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnAirRight(value.Get<float>());
    }

    [Command]
    void CmdOnAirRight(float value)
    {
        airRightInput = value;
    }

    void OnAirFront(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnAirFront(value.Get<float>());
    }

    [Command]
    void CmdOnAirFront(float value)
    {
        airFrontInput = value;
    }

    void OnPowerSlide(InputValue value)
    {
        if (!isLocalPlayer) return;
        CmdOnPowerSlide(Convert.ToBoolean(value.Get<float>()));
    }

    [Command]
    void CmdOnPowerSlide(bool value)
    {
        powerSlideInput = value;
    }
}
