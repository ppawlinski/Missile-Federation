using System;
using System.Collections;
using UnityEngine;
using Mirror;

class CarJump : NetworkBehaviour
{
    CarInput input;
    Rigidbody rb;
    CarParametersSO parameters;

    bool isDoubleJumpAvailible = false;
    bool noJumpAerial = false;

    public bool IsJumping { get; private set; } = false;

    public event EventHandler Flip;
    public event EventHandler JumpEvent;
    public event EventHandler DoubleJumpEvent;

    private void Awake()
    {
        parameters = GetComponent<CarController>().parameters;
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (GetComponent<CarController>().IsMovementBlocked) return;
        CheckIfDoubleJumpAvailble();
        if (input.JumpStartInput) TryJump();
    }

    private void TryJump()
    {
        input.JumpStartInput = false;
        if (GetComponent<CarWheels>().WheelsGrounded == 4 && !IsJumping)
        {
            StartCoroutine(Jump());
            JumpEvent?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (isDoubleJumpAvailible)
        {
            isDoubleJumpAvailible = false;
            if (input.AirFrontInput == 0 && input.AirRollInput + input.AirRightInput == 0)
            {
                DoubleJump();
                return;
            }
            Flip?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (GetComponent<CarAerialMovement>().IsCapsized)
        {
            StartCoroutine(TurnBackUp());
        }
    }
    private IEnumerator Jump()
    {
        isDoubleJumpAvailible = true;
        IsJumping = true;
        float time = 0;

        rb.AddForce(10 * parameters.jumpForce * transform.up, ForceMode.Force);
        while (time < 0.2f && (input.JumpHoldInput || time < 0.12f))
        {
            if (GetComponent<CarController>().IsMovementBlocked) break;
            if (!GetComponent<CarFlip>().IsFlipping) rb.angularVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
            if (time == 0) rb.AddForce(6 * parameters.jumpForce * -transform.up, ForceMode.Force);
            rb.AddForce(transform.up * parameters.jumpForce, ForceMode.Force);
            time += Time.fixedDeltaTime;
        }
        input.JumpHoldInput = false;
        IsJumping = false;
        StartCoroutine(FlipTimer());
    }
    private IEnumerator FlipTimer()
    {
        yield return new WaitForSecondsRealtime(1.25f);
        isDoubleJumpAvailible = false;
    }

    private void DoubleJump()
    {
        rb.AddForce(5 * parameters.jumpForce * transform.up, ForceMode.Force);
        DoubleJumpEvent?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator TurnBackUp()
    {
        float time = 0;
        while (time < 0.4f)
        {
            if (GetComponent<CarController>().IsMovementBlocked) break;
            rb.AddRelativeTorque(new Vector3(0, 0, 100), ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
    }
    public void CheckIfDoubleJumpAvailble()
    {
        if (GetComponent<CarWheels>().WheelsGrounded > 2 && !IsJumping)
        {
            isDoubleJumpAvailible = false;
            noJumpAerial = false;
        }
        else
        {
            if (!noJumpAerial)
            {
                noJumpAerial = true;
                isDoubleJumpAvailible = true;
            }
        }
    }
}
