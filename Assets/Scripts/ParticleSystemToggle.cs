using UnityEngine;
using UnityEngine.UI;

public class ParticleSystemToggle : MonoBehaviour
{
    public ParticleSystem particleSystem;
    private bool isPlaying;
    public int collisionThreshold;
    private int collisions;
    public Text collisionCountText;
    public Transform carTransform;
    public Vector3 resetPosition;

    private void Start()
    {
        isPlaying = false;
        collisions = 0;
        particleSystem.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;
        collisionCountText.text = "Collision count: " + collisions.ToString();

         if (collisions >= collisionThreshold + 5)
        {
            ResetCar();
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

    public void ResetCar()
    {
        carTransform.position = resetPosition;
        collisions = 0;
        collisionCountText.text = collisions.ToString();
        particleSystem.Stop();
    }
}