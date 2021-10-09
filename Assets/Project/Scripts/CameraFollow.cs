using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    //TODO add wall camera mode
    //lookatdirection can be done with extension method Flat()
    [SerializeField] GameObject carObject;
    [SerializeField] Transform ball;
    [SerializeField] float camDistance = 9.5f;
    [SerializeField] float camHeight = 3;
    [SerializeField] bool ballCam = false;

    PlayerManager playerManager;
    bool playerSet = false;

    //normal camera offset and alternative used while in air with ballCam off
    private Vector3 cameraOffset;
    private Vector3 alternativeCameraOffset;

    private float distanceBasedOnVelocity;

    //camera velocity used for smooth transitions
    private Vector3 currentCamSmoothVelocity;
    private Vector3 currentAlternativeCamSmoothVelocity;

    private void OnEnable()
    {
        CarController.OnLocalPlayerStart += SetFollowedPlayer;
    }
    private void OnDisable()
    {
        CarController.OnLocalPlayerStart -= SetFollowedPlayer;
    }
    void Start()
    {
        /*playerManager = FindObjectOfType<PlayerManager>();
        carObject = playerManager.PlayerObject;
        Vector3 _lookingDirection = Quaternion.Euler(0, -90, 0) * Vector3.Cross(Vector3.down, carObject.transform.forward);
        Debug.DrawRay(carObject.transform.position, _lookingDirection * 20, Color.black);
        Debug.DrawRay(carObject.transform.position, carObject.transform.forward.Flat() * 20, Color.yellow);

        cameraOffset = _lookingDirection.normalized * camDistance;
        cameraOffset.y += camHeight;
        transform.position = carObject.transform.position + cameraOffset;
        Vector3 templookattarget = carObject.transform.position;
        templookattarget.y = transform.position.y;
        transform.LookAt(templookattarget);
        distanceBasedOnVelocity = camDistance;*/
    }

    public void SetFollowedPlayer(GameObject player)
    {
        carObject = player;
        playerSet = true;
    }

    void LateUpdate()
    {
        if (!playerSet) return;
        //TODO increase camera distance with speed
        //add animationcurve for that and disable it when going backwards
        //distanceBasedOnVelocity = camDistance + 4* carObject.Velocity.magnitude / carObject.MaxVelocity;

        distanceBasedOnVelocity = camDistance;
        Vector3 newCameraOffset;
        CalculateNonBallcamAerial();
        if (!ballCam)
        {
            //TODO stabilize the camera when dodging

            //if on the ground
            if (carObject.transform.position.y < 13f || carObject.GetComponent<CarWheels>().WheelsGrounded >= 3)
            {
                Vector3 _lookingDirection = Quaternion.Euler(0, -90, 0) * Vector3.Cross(Vector3.down, carObject.transform.forward);
                newCameraOffset = _lookingDirection.normalized * distanceBasedOnVelocity;
                newCameraOffset.y += camHeight;
                alternativeCameraOffset = newCameraOffset;
            }
            else
            {
                newCameraOffset = alternativeCameraOffset;
            }

            cameraOffset = Vector3.SmoothDamp(cameraOffset, newCameraOffset, ref currentCamSmoothVelocity, 0.05f, 100f);
            transform.position = carObject.transform.position + cameraOffset;
            Vector3 _tempLookAtTarget = carObject.transform.position;
            _tempLookAtTarget.y = transform.position.y;
            transform.LookAt(_tempLookAtTarget);
        }
        else
        {
            Vector3 _ballPosition = ball.position;
            Vector3 _targetPosition = carObject.transform.position;
            Vector3 _cameraPivot = _targetPosition + Vector3.up * camHeight;

            Vector3 _cameraPivotToBallVector = _ballPosition - _cameraPivot;
            Vector3 _carGroundVector = new Vector3(_ballPosition.x, _targetPosition.y, _ballPosition.z) - _targetPosition;

            //TODO come up with a good equation covering all possibilities or divide it into smaller cases
            float _maxSpeed = Mathf.Clamp(Vector3.Angle(transform.eulerAngles, _cameraPivotToBallVector) - 20, 5, 100);

            Vector3 _lookAtDirection;



            if (Vector3.Angle(_cameraPivotToBallVector, _carGroundVector) < 20)
            {
                Vector3 _newCameraAngle = Quaternion.Euler(0, -90, 0) * Vector3.Cross(Vector3.down, _carGroundVector);
                newCameraOffset = _newCameraAngle.normalized * distanceBasedOnVelocity;
                newCameraOffset.y += camHeight;
            }
            else
            {
                newCameraOffset = -_cameraPivotToBallVector.normalized * distanceBasedOnVelocity;
                newCameraOffset.y += camHeight;
            }

            cameraOffset = Vector3.SmoothDamp(cameraOffset, newCameraOffset, ref currentCamSmoothVelocity, 0.05f, _maxSpeed);
            Vector3 newCameraPosition = _targetPosition + cameraOffset;
            newCameraPosition.y = newCameraPosition.y < 12.8f ? 12.8f : newCameraPosition.y;
            _lookAtDirection = _cameraPivot;
            transform.position = newCameraPosition;
            transform.LookAt(_lookAtDirection);
        }
    }

    private void CalculateNonBallcamAerial()
    {
        float _maxSpeed = ballCam ? 100 : 5;
        Vector3 _newCameraOffset;

        //instead of turning the camera towards the target I turn it towards the half way point between current and target rotation
        //this prevents the camera to go over the car when transitioning front to back (instead it turns front-side-back)
        Vector3 _lookingDirection = Quaternion.Euler(0, -90, 0) * Vector3.Cross(Vector3.down, carObject.GetComponent<Rigidbody>().velocity.normalized + transform.forward);
        _newCameraOffset = _lookingDirection.normalized * distanceBasedOnVelocity;
        _newCameraOffset.y += camHeight;
        alternativeCameraOffset = Vector3.SmoothDamp(alternativeCameraOffset, _newCameraOffset, ref currentAlternativeCamSmoothVelocity, 0.05f, _maxSpeed);
        alternativeCameraOffset = alternativeCameraOffset.normalized * distanceBasedOnVelocity;
    }

    public void OnToggleView()
    {
        ballCam = !ballCam;
    }
}
