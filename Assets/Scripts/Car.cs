using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; //Identifies if the wheels are at the motor of the car
    public bool steering; //Identifies if the wheels apply steering
}
public class Car : MonoBehaviour
{
    public static Car instance { get; private set; } //Global static instance of car
    Rigidbody rigidbody; //The cars rigidbody
    public Rigidbody Rigidbody { get { return rigidbody; } } //Getter for the cars rigidbody 
    public List<AxleInfo> axleInfos; //List of all the cars axles, which include the wheels
    public float maxMotorTorque; //The maximum speed the car can "generate"
    public float maxSteeringAngle; //The maximum angle the front wheels can steer to
    float motor; //The current torque/force applied to the cars motor
    float steering; //The current steering applied to the car
    //For checking if the car/motor should be braking or not
    bool braking;
    //For checking if all wheels are grounded
    bool allWheelsGrounded;
    public bool AllWheelsGrounded { get { return allWheelsGrounded;  } }
    //How fast the car can rotate when mid air
    public float midAirRotationSpeed;

    private void Awake()
    {
        //If there is already an instance of a car, remove this one
        //Now you could globally call the car at anytime with Car.instance
        //Example would be Car.instance.Rigidbody.AddForce(xx);
        if (instance != null && instance != this) { Destroy(this); }
        else {
            instance = this;
            //DontDestroyOnLoad(this); //Makes sure that this instance cannot be destroyed anymore
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks input of spacebar to control braking of the car
        if (Input.GetKey(KeyCode.Space))
        {
            braking = true;
        }
        else
        {
            braking = false;
        }
    }

    public void FixedUpdate()
    {
        if (braking == true)
        {
            motor = 0; //Sets the cars applied motor torque/force to zero, meaning no additional speed gets added to the car
            //Sets the cars braking torque to the max motor torque, meaning it comes to a halt almost immediately
            foreach (AxleInfo axleInfo in axleInfos)
            {
                axleInfo.leftWheel.brakeTorque = maxMotorTorque;
                axleInfo.rightWheel.brakeTorque = maxMotorTorque;
            }
        }
        else
        {
            motor = maxMotorTorque * Input.GetAxis("Vertical"); //Reads vertical input, multiplies it with the max motor torque and adds it to the current motor torque
            //Sets the cars braking torque to zero again...
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (Input.GetAxis("Vertical") != 0)
                {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                }
                //...unless the input is zero, then the car should stop on its own, but gradually
                else
                {
                    axleInfo.leftWheel.brakeTorque = maxMotorTorque/4;
                    axleInfo.rightWheel.brakeTorque = maxMotorTorque/4;
                }
            }
        }
        steering = maxSteeringAngle * Input.GetAxis("Horizontal"); //Reads horizontal input and sets the steering accordingly

        //Checks all the axles wheels and applies the correct steering, motor and rotation
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyVisualRotation(axleInfo.leftWheel);
            ApplyVisualRotation(axleInfo.rightWheel);
        }

        //checks if all four wheels are grounded
        int isGroundedCounter = 0;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.leftWheel.isGrounded)
            {
                isGroundedCounter++;
            }
            if (axleInfo.rightWheel.isGrounded)
            {
                isGroundedCounter++;
            }
        }
        if(isGroundedCounter < axleInfos.Count * 2)
        {
            allWheelsGrounded = false;
        }
        else
        {
            allWheelsGrounded = true;
        }

        //allows for midair rotation adjustments to the car
        Quaternion addRotationHorizontal = Quaternion.Euler(new Vector3(0, 0, midAirRotationSpeed) * -Input.GetAxis("Horizontal"));
        rigidbody.MoveRotation(rigidbody.rotation * addRotationHorizontal);
        Quaternion addRotationVertical = Quaternion.Euler(new Vector3(midAirRotationSpeed, 0, 0) * Input.GetAxis("Vertical"));
        rigidbody.MoveRotation(rigidbody.rotation * addRotationVertical);
    }

    //When entering a trigger, activate the object it if its a power up
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PowerUpInterface>() != null)
        {
            other.GetComponent<PowerUpInterface>().Activate(this);
        }
    }

    //When colliding, activate the object it if its a power up
    //This exists just in case someone creates a power up which is not a trigger but a solid collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PowerUpInterface>() != null)
        {
            collision.gameObject.GetComponent<PowerUpInterface>().Activate(this);
        }
    }

    //Finds the corresponding visual wheel in the child object and correctly applies the rotation
    public void ApplyVisualRotation(WheelCollider collider)
    {
        //Checks if there are even any child objects
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0); //Transform of the object with the wheel mesh

        Vector3 position; //Will not get used, however the collider always gives back the position as well
        Quaternion rotation; //Stores the rotation of the collider
        collider.GetWorldPose(out position, out rotation); //Gets position data from the collider
        visualWheel.transform.rotation = rotation; //Applies the colliders rotation to the wheel mesh
    }
}