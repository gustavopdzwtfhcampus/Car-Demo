using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject pause;
    public GameObject settings;
    public GameObject loading;
    public string sceneName;
    public Button restartButton;
    public Slider volumeSlider;
    public TMPro.TMP_Dropdown driveModeDropdown;
    public TMPro.TMP_InputField maxMotorTorqueInput;
    public TMPro.TMP_InputField maxSteeringAngleInput;
    public TMPro.TMP_InputField midAirRotationSpeedInput;
    public TMPro.TMP_InputField sidewayFrictionInput;
    public TMPro.TMP_InputField stiffnessInput;
    public Toggle toggleBox1;
    public Toggle toggleBox2;
    public Toggle toggleBox3;
    public Toggle toggleBox4;
    public Button settingsButton;
    public Button menuButton;
    public Button returnButton;
    public Button backButton;
    public AudioSource buttonSound;
    public MouseController mouseController;
    class AudioObject
    {
        public float startingVolume;
        public AudioSource audioSource;

        public AudioObject(float startingVolume, AudioSource audioSource)
        {
            this.startingVolume = startingVolume;
            this.audioSource = audioSource;
        }
    }
    List<AudioObject> audioObjects;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = 1;

        audioObjects = new List<AudioObject>();
        foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>())
        {
            audioObjects.Add(new AudioObject(audio.volume, audio));
        }

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            foreach (AudioObject audioObject in audioObjects)
            {
                audioObject.audioSource.volume = audioObject.startingVolume*(1-(1-volumeSlider.value));
            }
        });
        //-------------------------------------------------------------------
        driveModeDropdown.onValueChanged.AddListener((value) => {
            Car.instance.ChangeDriveMode(value);
        });
        Car.instance.ChangeDriveMode(driveModeDropdown.value);
        //-------------------------------------------------------------------
        maxMotorTorqueInput.onValueChanged.AddListener((value) => {
            try
            {
                Car.instance.maxMotorTorque = float.Parse(value);
            }
            catch{}
        });
        try
        {
            Car.instance.maxMotorTorque = float.Parse(maxMotorTorqueInput.text);
        }
        catch{}
        //-------------------------------------------------------------------
        maxSteeringAngleInput.onValueChanged.AddListener((value) => {
            try
            {
                Car.instance.maxSteeringAngle = float.Parse(value);
            }
            catch { }
        });
        try
        {
            Car.instance.maxSteeringAngle = float.Parse(maxSteeringAngleInput.text);
        }
        catch { }
        //-------------------------------------------------------------------
        midAirRotationSpeedInput.onValueChanged.AddListener((value) => {
            try
            {
                Car.instance.midAirRotationSpeed = float.Parse(value);
            }
            catch { }
        });
        try
        {
            Car.instance.midAirRotationSpeed = float.Parse(midAirRotationSpeedInput.text);
        }
        catch { }
        //-------------------------------------------------------------------
        sidewayFrictionInput.onValueChanged.AddListener((value) => {
            try
            {
                foreach (AxleInfo axleInfo in Car.instance.axleInfos)
                {
                    WheelFrictionCurve tempWFC;
                    tempWFC = axleInfo.leftWheel.sidewaysFriction;
                    tempWFC.extremumSlip = float.Parse(value);
                    axleInfo.leftWheel.sidewaysFriction = tempWFC;

                    WheelFrictionCurve tempWFC_;
                    tempWFC_ = axleInfo.rightWheel.sidewaysFriction;
                    tempWFC_.extremumSlip = float.Parse(value);
                    axleInfo.rightWheel.sidewaysFriction = tempWFC_;
                }
            }
            catch { }
        });
        try
        {
            foreach (AxleInfo axleInfo in Car.instance.axleInfos)
            {
                WheelFrictionCurve tempWFC;
                tempWFC = axleInfo.leftWheel.sidewaysFriction;
                tempWFC.extremumSlip = float.Parse(sidewayFrictionInput.text);
                axleInfo.leftWheel.sidewaysFriction = tempWFC;

                WheelFrictionCurve tempWFC_;
                tempWFC_ = axleInfo.rightWheel.sidewaysFriction;
                tempWFC_.extremumSlip = float.Parse(sidewayFrictionInput.text);
                axleInfo.rightWheel.sidewaysFriction = tempWFC_;
            }
        }
        catch { }
        //-------------------------------------------------------------------
        stiffnessInput.onValueChanged.AddListener((value) => {
            try
            {
                foreach (AxleInfo axleInfo in Car.instance.axleInfos)
                {
                    WheelFrictionCurve tempWFC;
                    tempWFC = axleInfo.leftWheel.sidewaysFriction;
                    tempWFC.stiffness = float.Parse(value);
                    axleInfo.leftWheel.sidewaysFriction = tempWFC;

                    WheelFrictionCurve tempWFC_;
                    tempWFC_ = axleInfo.rightWheel.sidewaysFriction;
                    tempWFC_.stiffness = float.Parse(value);
                    axleInfo.rightWheel.sidewaysFriction = tempWFC_;
                }
            }
            catch { }
        });
        try
        {
            foreach (AxleInfo axleInfo in Car.instance.axleInfos)
            {
                WheelFrictionCurve tempWFC;
                tempWFC = axleInfo.leftWheel.sidewaysFriction;
                tempWFC.stiffness = float.Parse(stiffnessInput.text);
                axleInfo.leftWheel.sidewaysFriction = tempWFC;

                WheelFrictionCurve tempWFC_;
                tempWFC_ = axleInfo.rightWheel.sidewaysFriction;
                tempWFC_.stiffness = float.Parse(stiffnessInput.text);
                axleInfo.rightWheel.sidewaysFriction = tempWFC_;
            }
        }
        catch { }
        //-------------------------------------------------------------------
        toggleBox1.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            mouseController.matchCarRotation = toggleBox1.isOn;
        });

        toggleBox2.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            mouseController.invertXAxis = toggleBox2.isOn;
        });

        toggleBox3.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            mouseController.invertYAxis = toggleBox2.isOn;
        });

        toggleBox4.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            GameManager.instance.debugInfoEnabled = toggleBox4.isOn;
        });

        restartButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(pause);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            ToggleObject(loading);
        });

        settingsButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(pause);
            ToggleObject(settings);
            volumeSlider.Select();
        });

        menuButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(pause);
            SceneManager.LoadSceneAsync(sceneName);
            ToggleObject(loading);
        });

        returnButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(pauseUI);
            Cursor.lockState = CursorLockMode.Locked;
        });

        backButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(pause);
            ToggleObject(settings);
            returnButton.Select();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(pauseUI);
            if(pauseUI.active == true)
            {
                Cursor.lockState = CursorLockMode.None;
                if(returnButton.IsActive() == true)
                {
                    returnButton.Select();
                }
                else
                {
                    backButton.Select();
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void ToggleObject(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.active);
    }
}
