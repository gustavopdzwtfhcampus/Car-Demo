using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ParticleSystemToggle : MonoBehaviour
{
    public ParticleSystem particleSystem;
    private bool isPlaying;
    public int collisionThreshold;
    private int collisions;
    public TextMeshProUGUI collisionCountText;
    //public Transform carTransform;
    //public Vector3 resetPosition;
    public float explosionDuration;
    public float explosionForce;
    public float explosionRadius;
    bool startedCoroutine = false;

    private void Start()
    {
        isPlaying = false; 
        collisions = 0; //zero collisions
        particleSystem.Stop(); //no effects are playing
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++; // if we collide add collisions
        collisionCountText.text = "Collision count: " + collisions; //update counter

         if (collisions >= collisionThreshold + 5) //if collisions over the threshold+5 reset the car and let explode
        {
            //Car.instance.CarCanGo = false;
            Car.instance.Rigidbody.AddExplosionForce(Car.instance.Rigidbody.mass * explosionForce, transform.position, explosionRadius, 0f, ForceMode.Impulse); //explode
            if(startedCoroutine == false) //so that the countdown does not get started over and over again
            {
                startedCoroutine = true; //coroutine is running now
                StartCoroutine(Countdown(explosionDuration)); //start the explosion
            }
            
        }
        else if (collisions >= collisionThreshold) //if collisions over the threshold play particles
        {
            particleSystem.Play();
            isPlaying = true;
        }
    }


    /* On/Off of the particle system */
    public void ToggleParticleSystem()
    {
        if (isPlaying)
        {
            particleSystem.Stop();
            isPlaying = false;
        }
        else
        {
            if (collisions >= collisionThreshold)
            {
                particleSystem.Play();
                isPlaying = true;
            }
        }
    }

    public IEnumerator Countdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResetCar();
    }

    public void ResetCar()
    {
        //carTransform.position = resetPosition;
        particleSystem.Stop();
        particleSystem.gameObject.SetActive(false);
        StartCoroutine(ParticleEnableWait());//So that while the last particles draw out, they do not get shown when the car resets after an explosion
        collisions = 0;
        collisionCountText.text = "Collision Count: " + collisions;
        //Car.instance.CarCanGo = true;
        Car.instance.ResetCar(); //call Car.instance instead
        startedCoroutine = false;
    }

    //By using an IEnumerator, the script can run multiple tasks in parallel
    //such as waiting for a certain amount of time to pass, without blocking the main thread
    public IEnumerator ParticleEnableWait()
    {
        yield return new WaitForSeconds(2f); //wait
        particleSystem.gameObject.SetActive(true);
    }
}