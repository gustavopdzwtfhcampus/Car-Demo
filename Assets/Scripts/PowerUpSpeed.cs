using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : MonoBehaviour, PowerUpInterface
{
    public float duration;
    public void Activate(Car car)
    {
        //Calls the game manager to play the sound this gameobject should emit
        GameManager.instance.PlaySound(this.gameObject);

        //Inbetween the emission of the sound and the unloading of the object
        //The actual functionality of the power up should occur
        //HERE
        StartCoroutine(Countdown(duration, car));
        //Removes the specified gameobject from the game, visually, meaning it still exists but
        //its colliders and meshrenderers are deactivated
        //More details in game manager method
        GameManager.instance.UnloadObject(this.gameObject);
    }

    //An IEnumerator can be used to call functionality after a certain time
    //with yield return new WaitForSeconds(duration);
    //Parameters can be added as needed
    public IEnumerator Countdown(float duration, Car car)
    {
        float oldMaxMotorTorque = Car.instance.maxMotorTorque;
        Car.instance.maxMotorTorque *= 3;
        yield return new WaitForSeconds(duration);
        Car.instance.maxMotorTorque = oldMaxMotorTorque;
    }
}