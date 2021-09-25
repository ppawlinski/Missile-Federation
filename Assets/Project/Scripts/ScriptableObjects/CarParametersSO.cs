using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CarParameters", order = 1)]
public class CarParametersSO : ScriptableObject
{
    public float maxSteeringAngle = 20f;
    public float maxMotorTorque = 5000f;
    public float brakeForce = 100000f;
    public float wheelForwardStiffness = 3f;
    public float jumpForce = 80000f;
    public float maxRPM = 700f;
    public AnimationCurve torqueCurve;
    public AnimationCurve steeringCurve;
    public float downforceDefault = 500f;
    public float defaultRotationValue = 0.5f;
    public float defaultFlipTorque = 100f;
    public float maxBoost = 3f;
    public readonly Vector3 centerOfMass = new Vector3(0,-0.127f,0);
    public readonly Vector3 aerialCenterOfMass = new Vector3(0, 0.176f, -0.268f);
}
