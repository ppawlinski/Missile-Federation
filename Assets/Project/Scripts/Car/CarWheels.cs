using UnityEngine;
using Mirror;

class CarWheels : NetworkBehaviour
{
    [Tooltip("Front wheels 0-1, back wheels 2-3")]
    [SerializeField] WheelCollider[] wheels = new WheelCollider[4];
    CarInput input;
    CarParametersSO parameters;
    Rigidbody rb;

    float driving = 0;
    int wheelsGrounded = 0;

    public int WheelsGrounded { get => wheelsGrounded; }

    private void Awake()
    {
        parameters = GetComponent<CarController>().parameters;
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
        wheels[0].ConfigureVehicleSubsteps(10, 10, 10);
        rb.centerOfMass = parameters.centerOfMass;
    }

    private void FixedUpdate()
    {
        driving = GetComponent<CarBoost>().IsBoosting ? 1 : GetComponent<CarInput>().DriveInput;
        CountWheelsGrounded();
        SetFrictionAndBrakes();
        SetSteerAngle();
        SetMotorTorque();
        DampenWheels();
        ManageDownforce();
        AddWallClimbingForce();
    }

    private void Update()
    {
        ApplyPositionToVisualWheels();
    }
    private void CountWheelsGrounded()
    {
        wheelsGrounded = 0;
        foreach (WheelCollider w in wheels)
        {
            if (w.isGrounded) wheelsGrounded++;
        }
    }
    private void SetFrictionAndBrakes()
    {
        if (wheelsGrounded == 0) return;

        WheelFrictionCurve x;
        x = wheels[0].forwardFriction;

        if (input.PowerSlideInput)
        {
            x.stiffness = 0.5f;
            wheels[0].forwardFriction = x;
            wheels[1].forwardFriction = x;

            x.stiffness = 1f;
            wheels[0].sidewaysFriction = x;
            wheels[1].sidewaysFriction = x;

            x.stiffness = 0f;
            wheels[2].forwardFriction = x;
            wheels[3].forwardFriction = x;

            x.stiffness = 0.1f;
            wheels[2].sidewaysFriction = x;
            wheels[3].sidewaysFriction = x;
            return;
        }

        bool moving = Mathf.Round(rb.velocity.z * 10) != 0f;
        int movingDirection = 0;
        if (moving) movingDirection = transform.InverseTransformDirection(rb.velocity).z > 0 ? 1 : -1;
        bool isBraking = (movingDirection > 0 && driving < 0) || (movingDirection < 0 && driving > 0);


        if (wheelsGrounded == 4)
        {
            x.stiffness = isBraking ? 15 : parameters.wheelForwardStiffness;
            foreach (WheelCollider wheel in wheels)
            {
                wheel.forwardFriction = x;
                wheel.brakeTorque = isBraking ? parameters.brakeForce : 0;
            }

            x.stiffness = 6;
            wheels[2].sidewaysFriction = x;
            wheels[3].sidewaysFriction = x;

            //x.stiffness = movingDirection < 0 ? 6 : 4;
            wheels[0].sidewaysFriction = x;
            wheels[1].sidewaysFriction = x;
        }
    }
    private void DampenWheels()
    {
        if (driving == 0)
        {
            float value = 1000 / rb.velocity.magnitude;
            foreach (WheelCollider w in wheels)
            {
                w.wheelDampingRate = value;
            }
        }
        else
        {
            foreach (WheelCollider w in wheels)
            {
                w.wheelDampingRate = 0.5f;
            }
        }
    }
    private void SetMotorTorque()
    {
        float motor = driving * parameters.torqueCurve.Evaluate(Mathf.Clamp(Mathf.Abs(wheels[0].rpm) / parameters.maxRPM, 0, 1)) * parameters.maxMotorTorque + 0.001f;
        foreach (WheelCollider w in wheels)
        {
            w.motorTorque = motor;
        }
    }
    private void SetSteerAngle()
    {
        float targetAngle = input.SteerInput * parameters.steeringCurve.Evaluate(Mathf.Abs(wheels[0].steerAngle) / parameters.maxSteeringAngle) * parameters.maxSteeringAngle;
        wheels[0].steerAngle = targetAngle;
        wheels[1].steerAngle = targetAngle;
    }
    private void ApplyPositionToVisualWheels()
    {
        foreach (WheelCollider w in wheels)
        {
            w.GetWorldPose(out Vector3 position, out Quaternion rotation);
            w.transform.GetChild(0).SetPositionAndRotation(position, rotation);
        }
    }
    private void ManageDownforce()
    {

        if (WheelsGrounded != 0 && Vector3.Angle(transform.up, Vector3.down) > 20) //check if on the ceiling
        {
            float downforce = GetComponent<CarJump>().IsJumping ? 0 : parameters.downforceDefault;
            foreach (WheelCollider w in wheels)
            {
                if (!w.isGrounded) rb.AddForceAtPosition(downforce * 20 * -transform.up, w.transform.position);
            }
            rb.AddForce(downforce * 30 * -transform.up);
        }
    }
    private void AddWallClimbingForce()
    {
        float wallClimbAssistValue = 100000;
        if (WheelsGrounded == 4)
        {
            if (Vector3.Angle(transform.forward, Vector3.up) < 85 && rb.velocity.magnitude < 10 && driving == 1)
            {
                rb.AddForce(transform.forward * wallClimbAssistValue, ForceMode.Force);
                return;
            }
            if (Vector3.Angle(transform.forward, Vector3.up) > 95 && rb.velocity.magnitude < 10 && driving == -1)
            {
                rb.AddForce(-transform.forward * wallClimbAssistValue, ForceMode.Force);
                return;
            }
        }
    }
}
