using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    Rigidbody rb;
    public AudioSource engineSource;
    public AudioSource musicSource;

    public float minEnginePitch = 0.2f;
    public float maxEnginePitch = 3.0f;
    public float pitchMultiplier = 5.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            musicSource.mute = !musicSource.mute;
        }
        float speed = rb.velocity.magnitude;
        engineSource.pitch = minEnginePitch + (speed / pitchMultiplier) % maxEnginePitch;
    }
}
