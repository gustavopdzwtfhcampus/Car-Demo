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
    bool carCanGo = true;
    public bool CarCanGo { get { return carCanGo; } set { carCanGo = value; } }
    public static Car instance { get; private set; } //Global static instance of car
    Rigidbody rigidbody; //The cars rigidbody
    public Rigidbody Rigidbody { get { return rigidbody; } } //Getter for the cars rigidbody
    Transform transformAtStart; //The cars starting position, rotation and scale
    public float maxNegativePosition; //The maximum negative y position the car can be positioned in before the game resets
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
    Vector3 startingGravity; //For resetting the cars gravity

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

        transformAtStart = new GameObject().transform; //Creates empty game object which we only use to store the cars initial transform
        //Copies the cars initial transform data
        transformAtStart.position = transform.position;
        transformAtStart.rotation = transform.rotation;
        transformAtStart.localScale = transform.localScale;
        startingGravity = Physics.gravity;
        ResetCar();
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

        if(transform.position.y < Mathf.Abs(maxNegativePosition) * -1)
        {
            ResetCar();
        }
    }

    public void FixedUpdate()
    {
        checkIfGrounded();

        if(carCanGo == true)
        {
            checkBraking();

            checkWheels();

            checkMidAirRotation();
        }

        checkBoosting();
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
    private void ApplyVisualRotation(WheelCollider collider)
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

    //Checks if all four wheels are grounded
    private void checkIfGrounded()
    {
        int groundedCounter = 0;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.leftWheel.isGrounded)
            {
                groundedCounter++;
            }
            if (axleInfo.rightWheel.isGrounded)
            {
                groundedCounter++;
            }
        }
        if (groundedCounter < axleInfos.Count * 2)
        {
            allWheelsGrounded = false;
        }
        else
        {
            allWheelsGrounded = true;
        }
    }

    //Checks whether the car should brake or not and sets values accordingly
    private void checkBraking()
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
                    axleInfo.leftWheel.brakeTorque = maxMotorTorque / 4;
                    axleInfo.rightWheel.brakeTorque = maxMotorTorque / 4;
                }
            }
        }
    }

    //Handles the cars wheels steering, acceleration and rotation
    private void checkWheels()
    {
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
    }

    //Enables the car to adjust its rotation when not on all four wheels (mainly intended for mid air adjustments)
    private void checkMidAirRotation()
    {
        if (allWheelsGrounded == false)
        {
            //Allows for midair rotation adjustments to the car
            Quaternion addRotationHorizontal = Quaternion.Euler(new Vector3(0, 0, midAirRotationSpeed) * -Input.GetAxis("Horizontal"));
            rigidbody.MoveRotation(rigidbody.rotation * addRotationHorizontal);
            Quaternion addRotationVertical = Quaternion.Euler(new Vector3(midAirRotationSpeed, 0, 0) * Input.GetAxis("Vertical"));
            rigidbody.MoveRotation(rigidbody.rotation * addRotationVertical);
        }
    }

    private void checkBoosting()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                WheelHit wheelHitLeft;
                axleInfo.leftWheel.GetGroundHit(out wheelHitLeft);

                WheelHit wheelHitRight;
                axleInfo.leftWheel.GetGroundHit(out wheelHitRight);

                if (wheelHitLeft.collider != null && wheelHitRight.collider != null)
                {
                    if (wheelHitLeft.collider == wheelHitRight.collider)
                    {
                        if(wheelHitLeft.collider.gameObject != null)
                        {
                            GameObject other = wheelHitLeft.collider.gameObject;
                            if (other.GetComponent<BoosterRamp>() != null)
                            {
                                other.GetComponent<BoosterRamp>().Boost(this);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ResetCar()
    {
        transform.position = transformAtStart.position;
        transform.rotation = transformAtStart.rotation;
        transform.localScale = transformAtStart.localScale;
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.useGravity = true;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.motorTorque = 0;
            axleInfo.rightWheel.motorTorque = 0;
            axleInfo.leftWheel.brakeTorque = maxMotorTorque * 100;
            axleInfo.rightWheel.brakeTorque = maxMotorTorque * 100;
        }
        Physics.gravity = startingGravity;
        carCanGo = true;
    }

    public void ChangeDriveMode(int i)
    {
        switch (i)
        {
            case 0:
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    axleInfo.motor = true;
                }
                break;
            case 1:
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    if (axleInfo.steering)
                    {
                        axleInfo.motor = true;
                    }
                    else
                    {
                        axleInfo.motor = false;
                    }
                }
                break;
            case 2:
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    if (axleInfo.steering)
                    {
                        axleInfo.motor = false;
                    }
                    else
                    {
                        axleInfo.motor = true;
                    }
                }
                break;
        }
    }
}