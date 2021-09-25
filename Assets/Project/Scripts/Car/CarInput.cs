using System;
using UnityEngine;

public abstract class CarInput : MonoBehaviour
{
    protected float driveInput = 0;
    protected float steerInput = 0;
    protected float airRightInput = 0;
    protected float airFrontInput = 0;
    protected float airRollInput = 0;
    protected bool boostInput = false;
    protected bool boostStartInput = false;
    protected bool powerSlideInput = false;
    protected bool jumpStartInput = false;
    protected bool jumpHoldInput = false;

    public float DriveInput { get => driveInput; }
    public float SteerInput { get => steerInput; }
    public float AirRightInput { get => airRightInput; }
    public float AirFrontInput { get => airFrontInput; }
    public float AirRollInput { get => airRollInput; }
    public bool BoostInput { get => boostInput; }
    public bool PowerSlideInput { get => powerSlideInput; }
    public bool JumpHoldInput { get => jumpHoldInput; set => jumpHoldInput = value; }
    public bool BoostStartInput { get => boostStartInput; set => boostStartInput = value; }
    public bool JumpStartInput { get => jumpStartInput; set => jumpStartInput = value; }
}
