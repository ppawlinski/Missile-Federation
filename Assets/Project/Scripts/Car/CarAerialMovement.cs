using System;
using UnityEngine;

class CarAerialMovement : MonoBehaviour
{
    CarInput input;
    Rigidbody rb;
    CarParametersSO parameters;
    CarWheels wheels;
    float rotationValue = 0;
    bool isColliding;
    public AllowedRotations AllowedRotations { get; set; }
    public bool IsCapsized { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Field")) isColliding = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Field")) isColliding = false;
    }

    private void Awake()
    {
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
        wheels = GetComponent<CarWheels>();
        parameters = GetComponent<CarController>().parameters;
        rotationValue = parameters.defaultRotationValue;
        rb.maxAngularVelocity = 5;
        AllowedRotations = AllowedRotations.All;
    }

    private void FixedUpdate()
    {
        ChangeCenterOfMass();
        if (GetComponent<CarController>().IsMovementBlocked || wheels.WheelsGrounded > 0) return;

        IsCapsized = isColliding && Vector3.Angle(transform.up, Vector3.down) < 5;
        UpdateAllowedRotations();

        rotationValue = isColliding ? parameters.defaultRotationValue / 5 : parameters.defaultRotationValue;
        AirRoll();
        AirFront();
        AirRight();

        DampenAngularVelocity();
    }

    private void UpdateAllowedRotations()
    {
        CarFlip flip = GetComponent<CarFlip>();
        if (AllowedRotations == AllowedRotations.None && input.AirFrontInput != 0)
        {
            bool backFlipCancel = input.AirFrontInput == 1 && flip.CurrentFlipType == CarFlip.FlipType.Back;
            bool frontFlipCancel = input.AirFrontInput == -1 && flip.CurrentFlipType == CarFlip.FlipType.Front;
            if (backFlipCancel || frontFlipCancel) AllowedRotations = AllowedRotations.All;

            bool diagonalBackFlipCancel = input.AirFrontInput == 1 && flip.CurrentFlipType == CarFlip.FlipType.DiagonalBack;
            bool diagonalFrontFlipCancel = input.AirFrontInput == -1 && flip.CurrentFlipType == CarFlip.FlipType.DiagonalFront;
            if (diagonalBackFlipCancel || diagonalFrontFlipCancel) AllowedRotations = AllowedRotations.FrontBackOnly;
        }
    }
    private void ChangeCenterOfMass()
    {
        rb.centerOfMass = wheels.WheelsGrounded > 0 ? parameters.centerOfMass : parameters.aerialCenterOfMass;
    }
    private void AirRoll()
    {
        if (!(wheels.WheelsGrounded == 0 && !IsCapsized && AllowedRotations == AllowedRotations.All)) return;

        rb.AddRelativeTorque(new Vector3(0, 0, -rotationValue * input.AirRollInput), ForceMode.VelocityChange);
    }
    private void AirRight()
    {
        if (!(wheels.WheelsGrounded == 0 && AllowedRotations == AllowedRotations.All)) return;

        if (IsCapsized) rotationValue = parameters.defaultRotationValue;
        rb.AddRelativeTorque(new Vector3(0, rotationValue * input.AirRightInput, 0), ForceMode.VelocityChange);
    }
    private void AirFront()
    {
        if (!(wheels.WheelsGrounded == 0 && !IsCapsized && (AllowedRotations == AllowedRotations.All || AllowedRotations == AllowedRotations.FrontBackOnly))) return;

        rb.AddRelativeTorque(new Vector3(rotationValue * input.AirFrontInput, 0, 0), ForceMode.VelocityChange);
    }
    private void DampenAngularVelocity()
    {
        bool rotating = input.AirFrontInput != 0 || input.AirRightInput != 0 || input.AirRollInput != 0;
        bool dampenAngularVelocity = !GetComponent<CarFlip>().IsFlipping && !rotating && wheels.WheelsGrounded == 0 && !isColliding;

        if (dampenAngularVelocity)
        {
            Vector3 currentVelocity = rb.angularVelocity;
            float currentMagnitude = currentVelocity.magnitude;
            currentVelocity = 0.90f * currentMagnitude * currentVelocity.normalized;
            rb.angularVelocity = currentVelocity;
        }
    }
}

public enum AllowedRotations
{
    None = 0,
    All = 1,
    FrontBackOnly = 2
}