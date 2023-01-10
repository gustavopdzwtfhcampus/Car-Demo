using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBoost : MonoBehaviour, PowerUpInterface
{
    public float boostForce;
    public bool unloadObjectOnCollision;
    public void Activate(Car car)
    {
        //Calls the game manager to play the sound this gameobject should emit
        GameManager.instance.PlaySound(this.gameObject);

        //Inbetween the emission of the sound and the unloading of the object
        //The actual functionality of the power up should occur
        //HERE
        Car.instance.Rigidbody.AddForce(Car.instance.transform.forward * 15000 * boostForce, ForceMode.Impulse);

        //Removes the specified gameobject from the game, visually, meaning it still exists but
        //its colliders and meshrenderers are deactivated
        //More details in game manager method
        if(unloadObjectOnCollision == true)
        {
            GameManager.instance.UnloadObject(this.gameObject);
        }
    }
}
