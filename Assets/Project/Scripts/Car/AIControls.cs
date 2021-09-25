
public class AIControls : CarInput
{
    public void SetDrive(float value)
    {
        driveInput = value;
    }

    public void SetSteer(float value)
    {
        steerInput = value;
    }

    public void SetJump(bool value)
    {
        jumpStartInput = !jumpHoldInput && value;
        jumpHoldInput = value;
    }

    public void SetBoost(bool value)
    {
        boostInput = value;
        boostStartInput = value;
    }

    public void SetAirRoll(float value)
    {
        airRollInput = value;
    }

    public void SetAirRight(float value)
    {
        airRightInput = value;
    }

    public void SetAirFront(float value)
    {
        airFrontInput = value;
    }

    public void SetPowerSlide(bool value)
    {
        powerSlideInput = value;
    }
}
