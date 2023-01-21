using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Functions nearly the same as class PowerUpBoost, but without the interface and the unloading of the object
public class BoosterRamp : MonoBehaviour
{
    public float boostByMassMultiplier;
    public void Boost(Car car)
    {
        //Calls the game manager to play the sound this gameobject should emit
        if(GameManager.instance != null)
        {
            GameManager.instance.PlaySound(this.gameObject);
        }

        //Inbetween the emission of the sound and the unloading of the object
        //The actual functionality of the power up should occur
        //HERE

        //Adds force to the cars forward direction
        //Using ForceMode.Impulse so that the speed increase is instantaneous
        Car.instance.Rigidbody.AddForce(Car.instance.transform.forward * Car.instance.Rigidbody.mass * (boostByMassMultiplier/2), ForceMode.Impulse);
    }
}
