using System;
using System.Collections;
using UnityEngine;

public class CarFlip : MonoBehaviour
{
    Rigidbody rb;
    CarInput input;
    CarParametersSO parameters;

    Vector3 flipTorque;
    Vector3 flipForce;
    private FlipType currentFlipType;

    public bool IsFlipping { get; private set; }
    public FlipType CurrentFlipType { get => currentFlipType; set => currentFlipType = value; }

    public enum FlipType
    {
        Front = 0,
        Back = 1,
        Side = 2,
        DiagonalFront = 3,
        DiagonalBack = 4,
        None = 5
    }

    private void Awake()
    {
        parameters = GetComponent<CarController>().parameters;
        rb = GetComponent<Rigidbody>();
        input = GetComponent<CarInput>();
    }

    private void OnEnable()
    {
        GetComponent<CarJump>().Flip += StartFlip;
    }

    private void OnDisable()
    {
        GetComponent<CarJump>().Flip -= StartFlip;
    }

    private void StartFlip(object sender, EventArgs e)
    {
        SetFlipValues();
        StartCoroutine(Flip());
    }
    private void SetFlipValues()
    {
        flipForce = Vector3.zero;
        flipTorque = Vector3.zero;
        Vector3 _tempFlipForce = Quaternion.Euler(0, -90, 0) * Vector3.Cross(transform.forward, Vector3.down).normalized;
        currentFlipType = FlipType.None;

        if (input.AirFrontInput > 0)
        {
            currentFlipType = FlipType.Front;
            flipForce = _tempFlipForce;
            flipTorque.x = 1;
        }
        else if (input.AirFrontInput < 0)
        {
            currentFlipType = FlipType.Back;
            flipForce = -_tempFlipForce;
            flipTorque.x = -1;
        }

        if (input.AirRollInput + input.AirRightInput != 0)
        {
            if (currentFlipType == FlipType.Front)
                currentFlipType = FlipType.DiagonalFront;
            else if (currentFlipType == FlipType.Back)
                currentFlipType = FlipType.DiagonalBack;
            else
                currentFlipType = FlipType.Side;
        }

        if (input.AirRollInput + input.AirRightInput > 0)
        {
            flipForce = (flipForce + (Quaternion.Euler(0, 90, 0) * _tempFlipForce)).normalized;
            flipTorque.z = -1;
        }
        else if (input.AirRollInput + input.AirRightInput < 0)
        {
            flipForce = (flipForce + (Quaternion.Euler(0, -90, 0) * _tempFlipForce)).normalized;
            flipTorque.z = 1;
        }

        flipForce = flipForce.normalized;
    }

    private IEnumerator Flip()
    {
        IsFlipping = true;
        GetComponent<CarAerialMovement>().AllowedRotations = AllowedRotations.None;
        float time = 0;
        rb.maxAngularVelocity = 10;
        Vector3 velocity;
        rb.AddForce(flipForce.normalized * 10, ForceMode.VelocityChange);
        while (time < 0.65f)
        {
            if (GetComponent<CarController>().IsMovementBlocked) break;
            if (time < 0.3f)
                rb.AddRelativeTorque(flipTorque.normalized * parameters.defaultFlipTorque, ForceMode.VelocityChange);
            if (time > 0.15f)
            {
                velocity = rb.velocity;
                velocity.y *= 0.65f;
                rb.velocity = velocity;
            }
            if (time > 0.2f)
            {
                rb.maxAngularVelocity = 8;
            }
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        rb.maxAngularVelocity = 5;
        GetComponent<CarAerialMovement>().AllowedRotations = AllowedRotations.All;
        IsFlipping = false;
    }
}