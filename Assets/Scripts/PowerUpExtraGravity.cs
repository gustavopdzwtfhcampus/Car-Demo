using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExtraGravity : MonoBehaviour, PowerUpInterface
{
    public float duration;
    public float gravityMultiplier;
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
        Vector3 originalGravity = Physics.gravity;
        Physics.gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        yield return new WaitForSeconds(duration);
        Physics.gravity = originalGravity;
    }
}