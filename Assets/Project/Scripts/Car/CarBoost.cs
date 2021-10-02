using System;
using UnityEngine;
using Mirror;

class CarBoost : NetworkBehaviour
{
    CarInput input;
    CarParametersSO parameters;

    float boostAmount = 0;
    bool isBoosting = false;

    public float BoostAmount { get => boostAmount; }
    public bool IsBoosting { get => isBoosting; }
    public bool IsFullBoost { get => boostAmount == parameters.maxBoost; }
    public int BoostPercentage { get => Mathf.FloorToInt(boostAmount / parameters.maxBoost *100); }

    public event EventHandler OnFailedBoostInit;

    void Awake()
    {
        input = GetComponent<CarInput>();
        parameters = GetComponent<CarController>().parameters;
    }

    private void OnEnable()
    {
        InGameUIController.OnGamePause += Pause;
    }

    private void OnDisable()
    {
        InGameUIController.OnGamePause -= Pause;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        isBoosting = input.BoostInput && boostAmount > 0 && !GetComponent<CarController>().IsMovementBlocked;
        if (isBoosting) Boost();

        if (input.BoostStartInput && BoostAmount == 0) OnFailedBoostInit?.Invoke(this, EventArgs.Empty);
        input.BoostStartInput = false;
    }
    private void Boost()
    {
        float boostValue = GetComponent<CarWheels>().WheelsGrounded == 0 ? 35000 : 30000;
        GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, boostValue), ForceMode.Force);

        boostAmount -= Time.fixedDeltaTime;
        if (boostAmount < 0) boostAmount = 0;
    }
    public void AddBoost(int percent)
    {
        boostAmount += parameters.maxBoost * percent / 100;
        if (boostAmount > parameters.maxBoost) boostAmount = parameters.maxBoost;
    }
    public void SetBoostAmount(int percent)
    {
        boostAmount = parameters.maxBoost * percent / 100;
        if (boostAmount > parameters.maxBoost) boostAmount = parameters.maxBoost;
    }

    private void Pause(bool value)
    {
        if (value) isBoosting = false;
    }
}
