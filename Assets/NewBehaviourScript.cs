using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{   
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    public float accerlation = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currentaccerlation = 0f;
    private float currentbreakforce = 0f;
    private float currentTurnAngle = 0f;

    private void FixedUpdate()
    {
        currentaccerlation = accerlation * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
            currentbreakforce = breakingForce;
        else
        {
            currentbreakforce = 0f;
        }

        frontRight.motorTorque = currentaccerlation;
        frontLeft.motorTorque = currentaccerlation;

        frontRight.brakeTorque = currentbreakforce;
        frontLeft.brakeTorque = currentbreakforce;
        backRight.brakeTorque = currentbreakforce;
        backLeft.brakeTorque = currentbreakforce;

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");

        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;
        backRight.steerAngle = currentTurnAngle;
        backLeft.steerAngle = currentTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);

    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }
}
