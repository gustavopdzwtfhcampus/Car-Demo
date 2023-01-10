using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI debugInfo; //The text box where the debug information gets displayed
    public bool debugInfoEnabled; //Whether the debug information should be displayed, can also be modfied ingame with the T key
    //Enum for the action when pressing the R key
    public enum RActivity 
    {
        RestartsScene,
        ResetsCar
    }

    [Tooltip("Whether pressing the 'R' button should restart the whole scene or only reset the cars position")]
    public RActivity rActivity;
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
        //Toggles debug information
        if (Input.GetKeyDown(KeyCode.T))
        {
            debugInfoEnabled = !debugInfoEnabled;
        }

        //The content of the debug information
        if (debugInfoEnabled == true)
        {
            debugInfo.enabled = true;
            debugInfo.text =
                "Debug Info (T): " + "\n" + "\n" +
                "Position: " + Car.instance.transform.position + "\n" +
                //"Rotation: " + UnityEditor.TransformUtils.GetInspectorRotation(Car.instance.transform) + "\n" +
                "Rotation:" + Car.instance.Rigidbody.transform.eulerAngles + "\n" +
                "Velocity: " + Car.instance.Rigidbody.velocity + "\n" +
                "Rpm: " + Car.instance.axleInfos[0].leftWheel.rpm + "\n" +
                "Motor Torque: " + Car.instance.axleInfos[0].leftWheel.motorTorque + "\n" +
                "Brake Torque: " + Car.instance.axleInfos[0].leftWheel.brakeTorque + "\n" +
                "Steer Angle: " + Car.instance.axleInfos[0].leftWheel.steerAngle + "\n" +
                "Grounded: " + Car.instance.AllWheelsGrounded;
        }
        else
        {
            debugInfo.enabled = false;
        }

        //Restarts scene or resets car
        if (Input.GetKeyDown(KeyCode.R))
        {
            switch (rActivity)
            {
                case RActivity.RestartsScene:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                case RActivity.ResetsCar:
                    Car.instance.ResetCar();
                    break;
            }
        }
    }

    //Unloads an object from the game, by deactivating its collider and meshrenderer
    //making the object invisible and uninteractable
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

    //Checks if the specified object even has an audio sorce on it and plays it if given
    public void PlaySound(GameObject gameObject)
    {
        //If the object has an audiosource on it, play it
        if (gameObject.GetComponent<AudioSource>())
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
