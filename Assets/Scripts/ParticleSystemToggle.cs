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
        collisions = 0;
        particleSystem.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;
        collisionCountText.text = "Collision count: " + collisions;

         if (collisions >= collisionThreshold + 5)
        {
            //Car.instance.CarCanGo = false;
            Car.instance.Rigidbody.AddExplosionForce(Car.instance.Rigidbody.mass * explosionForce, transform.position, explosionRadius, 0f, ForceMode.Impulse);
            if(startedCoroutine == false) //so that the countdown does not get started over and over again
            {
                startedCoroutine = true;
                StartCoroutine(Countdown(explosionDuration));
            }
            
        }
        else if (collisions >= collisionThreshold)
        {
            particleSystem.Play();
            isPlaying = true;
        }
    }

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
    public IEnumerator ParticleEnableWait()
    {
        yield return new WaitForSeconds(2f);
        particleSystem.gameObject.SetActive(true);
    }
}