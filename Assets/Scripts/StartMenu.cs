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
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;
    public Button backButton;

    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        Car.instance.CarCanGo = false;

        startButton.onClick.AddListener(() => {
            Car.instance.Rigidbody.AddForce(Car.instance.transform.forward * Car.instance.Rigidbody.mass * 50, ForceMode.Impulse);
            StartCoroutine(Countdown(3.5f));
            ToggleObject(start);
            ToggleObject(loading);
        });

        settingsButton.onClick.AddListener(() => {
            ToggleObject(start);
            ToggleObject(settings);
            ToggleObject(backButton.gameObject);
            volumeSlider.Select();
        });

        creditsButton.onClick.AddListener(() => {
            ToggleObject(start);
            ToggleObject(credits);
            ToggleObject(backButton.gameObject);
            backButton.Select();
        });

        quitButton.onClick.AddListener(()=>{
            Application.Quit();
                });

        backButton.onClick.AddListener(() => {
            ToggleObject(start);
            settings.SetActive(false);
            credits.SetActive(false);
            ToggleObject(backButton.gameObject);
            startButton.Select();
        });

        volumeSlider.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = volumeSlider.value;
    }

    void ToggleObject(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.active);
    }

    public IEnumerator Countdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
