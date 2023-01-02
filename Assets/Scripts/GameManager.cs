using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI debugInfo;
    public bool debugInfoEnabled;
    public static GameManager instance { get; private set; } //Global static instance of game manager
    private void Awake()
    {
        //If there is already an instance of a game manager, remove oneself
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            //Makes sure that this instance cannot be destroyed anymore
            //DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            debugInfoEnabled = !debugInfoEnabled;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (debugInfoEnabled == true)
        {
            debugInfo.enabled = true;
            debugInfo.text =
                "Debug Info (T):" + "\n" +
                "Position:      " + Car.instance.transform.position + "\n" +
                "Rotation:      " + Car.instance.transform.rotation + "\n" +
                "Velocity:      " + Car.instance.Rigidbody.velocity + "\n" +
                "Rpm:           " + Car.instance.axleInfos[0].leftWheel.rpm + "\n" +
                "Motor Torque:  " + Car.instance.axleInfos[0].leftWheel.motorTorque + "\n" +
                "Brake Torque:  " + Car.instance.axleInfos[0].leftWheel.brakeTorque + "\n" +
                "Steer Angle:   " + Car.instance.axleInfos[0].leftWheel.steerAngle + "\n" +
                "Grounded:      " + Car.instance.AllWheelsGrounded;
        }
        else
        {
            debugInfo.enabled = false;
        }
    }

    public void UnloadObject(GameObject gameObject)
    {
        //If the object has a collider, deactivate it
        //Meaning the object cannot be interacted with via collisions anymore
        if (gameObject.GetComponent<Collider>())
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }

        //If the object has a mesh renderer, deactivate it
        //Meaning the object does not get visualized/rendered anymore
        if (gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void PlaySound(GameObject gameObject)
    {
        //If the object has an audiosource on it, play it
        if (gameObject.GetComponent<AudioSource>())
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
