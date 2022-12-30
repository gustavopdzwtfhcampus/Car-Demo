using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public float speedMax =50.0f;
    public bool kinematicVelocity = false;

    public Light light;

    private bool inside = false;
    private Collider vehicle;
    private Vector3 lastPosition;

    private int etape = 0;
 

    public void Update()
    {
        if (inside)
        {
            if (etape == 1)
            {
                Vector3 position = vehicle.transform.position;
                float distance = Vector3.Distance(position, lastPosition);
                float vitesse = distance / Time.deltaTime;
                float vitessekmh = vitesse * 3.6f;
                Debug.Log(" vitesse : " + vitessekmh + " km/h");
               
                if (vitessekmh > speedMax)
                    flasher();

                etape = 2;
                inside = false;


            }
            if (etape == 0)
            {
                lastPosition = vehicle.transform.position;
                etape = 1;
            }
        }
                    
    }


    private void OnTriggerEnter(Collider other)
    { 
        if (kinematicVelocity)
        {
            float vitesse = other.attachedRigidbody.velocity.magnitude;
           
            if (vitesse > speedMax)
                flasher();
        }
        else
        {
            etape = 0;
            vehicle = other;
            inside = true;
       }
    }

    public void flasher()
    {
      
        StartCoroutine("flash");
    }

    IEnumerator flash()
    {
        yield return null;
        light.enabled = true;
        yield return new WaitForSeconds(0.15f);
        light.enabled = false;
        yield return null;
    }
}
