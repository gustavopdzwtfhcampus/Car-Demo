using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject start;
    public GameObject settings;
    public GameObject credits;
    public GameObject loading;
    public string sceneName;
    public Button startButton;
    public Slider volumeSlider;
    public Toggle toggleBox1;
    public Toggle toggleBox2;
    public Toggle toggleBox3;
    public Toggle toggleBox4;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;
    public Button backButton;
    public AudioSource buttonSound;
    public AudioSource wowSound;
    public AudioSource music;


    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartScreen")
        {
            Car.instance.CarCanGo = false;
        }
        volumeSlider.value = 1;

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>())
            {
                audio.volume = volumeSlider.value;
            }
        });

        toggleBox1.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
        });

        toggleBox2.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
        });

        toggleBox3.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
        });

        toggleBox4.onValueChanged.AddListener((value) => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
        });

        startButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            Car.instance.Rigidbody.AddForce(Car.instance.transform.forward * Car.instance.Rigidbody.mass * 50, ForceMode.Impulse);
            StartCoroutine(Countdown(3.5f));
            ToggleObject(start);
            ToggleObject(loading);
        });

        settingsButton.onClick.AddListener(() => {
            if(buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(start);
            ToggleObject(settings);
            ToggleObject(backButton.gameObject);
            volumeSlider.Select();
        });

        creditsButton.onClick.AddListener(() => {
            if (wowSound.gameObject.active == true)
            {
                wowSound.Play();
            }
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(start);
            ToggleObject(credits);
            ToggleObject(backButton.gameObject);
            backButton.Select();
        });

        quitButton.onClick.AddListener(()=>{
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            Application.Quit();
                });

        backButton.onClick.AddListener(() => {
            if (buttonSound.gameObject.active == true)
            {
                buttonSound.Play();
            }
            ToggleObject(start);
            settings.SetActive(false);
            credits.SetActive(false);
            ToggleObject(backButton.gameObject);
            startButton.Select();
        });
    }

    // Update is called once per frame
    void Update()
    {
        //music.volume = volumeSlider.value;
        //buttonSound.volume = volumeSlider.value;
        //wowSound.volume = volumeSlider.value;
    }

    void ToggleObject(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.active);
    }

    IEnumerator Countdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
