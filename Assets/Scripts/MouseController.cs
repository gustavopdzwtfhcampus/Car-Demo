using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    //The object which opposes the cars rotation
    public GameObject antiRotationObject;
    //The object which gets rotated
    public GameObject rotationObject;
    //The main camera
    Camera camera;
    public float rotationSpeed;
    public float viewAngleLimitHorizontal;
    public float viewAngleLimitVerticalUpper;
    public float viewAngleLimitVerticalLower;
    //If true, the camera does not move
    public bool lockCamera;
    //The rotation applied to the x axis/The vertical rotation of the mouse
    float rotationXAxis = 0;
    //The rotation applied to the y axis/The horizontal rotation of the mouse
    float rotationYAxis = 0;
    public bool invertXAxis;
    int invertXAxisModifier;
    public bool invertYAxis;
    int invertYAxisModifier;
    //Temporary variable to store and modify new field of view value
    float fieldOfViewModifier;
    //The starting value of the field of view
    float fieldOfViewStartingValue;
    //Lower limit of the field of view
    public float fieldOfViewLimitLow;
    //Upper limit of the field of view
    public float fieldOfViewLimitUpper;

    void Awake()
    {
        //Locks the mouse cursor into the middle of the screen and hides it, ESC will unlock it during play
        //IMPORTANT: Should this not work during testing in unity, just click into the game view and the mouse should lock
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Assigns the camera of the component this script is located on
        camera = this.GetComponent<Camera>();

        fieldOfViewStartingValue = camera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        //The anti rotation object always has a rotation of zero on all its axis,
        //meaning its children objects will not get affected by the cars rotation
        antiRotationObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (!lockCamera)
        {
            //Inverts X Axis
            if (invertXAxis) { invertXAxisModifier = -1; }
            else { invertXAxisModifier = 1; }

            //Inverts Y Axis
            if (invertYAxis) { invertYAxisModifier = -1; }
            else { invertYAxisModifier = 1; }

            //Reads the input of the vertical mouse input, multiplies it with the rotation speed and adds it to the rotation variable of the x axis
            rotationXAxis += -Input.GetAxis("Mouse Y") * rotationSpeed;
            //Limits the rotation of the x axis to the specified view angle
            rotationXAxis = Mathf.Clamp(rotationXAxis, -viewAngleLimitVerticalLower, viewAngleLimitVerticalUpper);

            //Reads the input of the horizontal mouse input, multiplies it with the rotation speed and adds it to the rotation variable of the y axis
            rotationYAxis += Input.GetAxis("Mouse X") * rotationSpeed;
            //Limits the rotation of the y axis to the specified view angle
            rotationYAxis = Mathf.Clamp(rotationYAxis, -viewAngleLimitHorizontal, viewAngleLimitHorizontal);

            //Changes the rotation of the camera by applying the previously read rotation of the x axis to a parent gameobject
            rotationObject.transform.localRotation = Quaternion.Euler(-rotationXAxis * invertXAxisModifier, -rotationYAxis * invertYAxisModifier, 0);
            //???
            //transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);

            //Reads the input of the mousewheel and subtracts/adds it from/to the cameras current field of view,
            //however it is not yet actually assigned to the camera, instead in a temporary variable
            fieldOfViewModifier = camera.fieldOfView - 2 * Input.mouseScrollDelta.y;
            //Clamps the new field of view between the assigned lower and upper limit the field of view should be able to have
            fieldOfViewModifier = Mathf.Clamp(fieldOfViewModifier, fieldOfViewLimitLow, fieldOfViewLimitUpper);
            //Assigns the new field of view to the cameras actual field of view
            camera.fieldOfView = fieldOfViewModifier;

            //Resets the field of view when the mouse wheel is clicked
            if (Input.GetMouseButtonDown(2))
            {
                camera.fieldOfView = fieldOfViewStartingValue;
            }
        }
    }
}