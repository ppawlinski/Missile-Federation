using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class BotMovementAgent : Agent
{
    [SerializeField] Transform ballTransform;
    [SerializeField] Transform opponentGoalTransform;
    [SerializeField] Transform ownGoalTransform;
    [SerializeField] Ball ball;
    [SerializeField] GameObject environmentContainer;

    protected float driveInput = 0;
    protected float steerInput = 0;
    protected float airRightInput = 0;
    protected float airFrontInput = 0;
    protected float airRollInput = 0;
    protected bool boostInput = false;
    protected bool powerSlideInput = false;
    protected bool jumpInput = false;
    private float minDistanceToBall = 0f;
    private float initDistanceToBall = 0f;


    AIControls car;

    private void Start()
    {
        car = GetComponent<AIControls>();
        minDistanceToBall = Vector3.Distance(transform.position, ballTransform.position);
        initDistanceToBall = minDistanceToBall;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ball")
        {
            AddReward(1f);
            EndEpisode();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GoalCheck.GoalScored += GoalScored;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        GoalCheck.GoalScored -= GoalScored;
    }

    private void FixedUpdate()
    {
        AddReward(-1.0f/MaxStep);
        if (transform.InverseTransformVector(GetComponent<Rigidbody>().velocity).z > 0) AddReward(.5f/MaxStep);

        float _distanceToBall = Vector3.Distance(transform.position, ballTransform.position);
        if (_distanceToBall < minDistanceToBall)
        {
            AddReward((minDistanceToBall - _distanceToBall) / initDistanceToBall);
            minDistanceToBall = _distanceToBall;
        }
    }


    public override void OnEpisodeBegin()
    {
        //car.SetRandomPosition();
        //ball.SetRandomPosition();
        minDistanceToBall = Vector3.Distance(transform.position, ballTransform.position);
        initDistanceToBall = minDistanceToBall;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 _dirToTarget = (ballTransform.position - transform.position).normalized;
        sensor.AddObservation(transform.InverseTransformPoint(ballTransform.position));
        sensor.AddObservation(transform.InverseTransformPoint(ownGoalTransform.position));
        sensor.AddObservation(transform.InverseTransformPoint(opponentGoalTransform.position));
        sensor.AddObservation(transform.InverseTransformVector(ball.GetComponent<Rigidbody>().velocity));
        sensor.AddObservation(transform.InverseTransformVector(GetComponent<Rigidbody>().velocity));
        sensor.AddObservation(transform.InverseTransformDirection(_dirToTarget));
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int drive = actions.DiscreteActions[0] - 1;
        int steer = actions.DiscreteActions[1] - 1;
        car.SetDrive(drive);
        car.SetSteer(steer);

        /*car.SetBoost(Convert.ToBoolean(actions.DiscreteActions[2]));
        car.SetJump(Convert.ToBoolean(actions.DiscreteActions[3]));
        car.SetPowerSlide(Convert.ToBoolean(actions.DiscreteActions[4]));
        car.SetAirFront(actions.DiscreteActions[5] - 1);
        car.SetAirRight(actions.DiscreteActions[6] - 1);
        car.SetAirRoll(actions.DiscreteActions[7] - 1);*/
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Convert.ToInt32(driveInput + 1);
        discreteActions[1] = Convert.ToInt32(steerInput + 1);
        /*discreteActions[2] = Convert.ToInt32(boostInput);
        discreteActions[3] = Convert.ToInt32(jumpInput);
        discreteActions[4] = Convert.ToInt32(powerSlideInput);
        discreteActions[5] = Convert.ToInt32(airFrontInput + 1);
        discreteActions[6] = Convert.ToInt32(airRightInput + 1);
        discreteActions[7] = Convert.ToInt32(airRollInput + 1);*/

    }
    private void GoalScored(int goal_id, int env_id)
    {
        if(env_id == environmentContainer.GetInstanceID())
        {
            if (goal_id == 2) AddReward(1f);
            else AddReward(-1f);
            EndEpisode();
        }
    }
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
        jumpInput = Convert.ToBoolean(value.Get<float>());
    }

    void OnBoost(InputValue value)
    {
        boostInput = Convert.ToBoolean(value.Get<float>());
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
