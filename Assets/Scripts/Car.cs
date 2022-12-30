using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    //Global static instance of car
    public static Car instance { get; private set; }
    //The cars rigidbody
    Rigidbody rigidbody;
    //Getter for the cars rigidbody
    public Rigidbody Rigidbody
    {
        get { return rigidbody; }
    }
    //The rotation vector that gets applied to the car every second when inputing a rotation
    Vector3 rotationVectorPerSecond;
    //Speed/Force that gets added to the cars rigidbody
    public float speed;
    //The rotation value of the y-axis which gets added to the rotation vector
    public float rotation;

    private void Awake()
    {
        //If there is already an instance of a car, remove oneself
        //Now you could globally call the car at anytime with Car.instance
        //Example would be Car.instance.speed = xx; or Car.instance.Rigidbody.AddForce(xx);
        //HOWEVER, TRY TO CALL THE CAR VIA THIS METHOD AS LITTLE AS POSSIBLE
        //IF YOU FOR EXAMPLE WANT TO IMPLEMENT A POWER UP, TRY TO GET THE INSTANCE OF THE CAR
        //VIA THE PARAMETER OF YOUR FUNCTION    
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            //Makes sure that this instance cannot be destroyed anymore
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        rotationVectorPerSecond = new Vector3(0, rotation, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Vertical")!=0)
        {
            rigidbody.AddForce(transform.forward * Input.GetAxis("Vertical") * (speed*10000) * Time.deltaTime);
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            Quaternion addRotation = Quaternion.Euler(rotationVectorPerSecond * Input.GetAxis("Horizontal") * Time.deltaTime);
            rigidbody.MoveRotation(rigidbody.rotation * addRotation);
        }
    }

    //When entering a trigger, activate the object it if its a power up
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PowerUpInterface>() != null)
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
}