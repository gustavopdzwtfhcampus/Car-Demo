using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBoost : MonoBehaviour, PowerUpInterface
{
    public float boostByMassMultiplier;
    public void Activate(Car car)
    {
        //Calls the game manager to play the sound this gameobject should emit
        GameManager.instance.PlaySound(this.gameObject);

        //Inbetween the emission of the sound and the unloading of the object
        //The actual functionality of the power up should occur
        //HERE

        //Adds force to the cars forward direction
        //Using ForceMode.Impulse so that the speed increase is instantaneous
        Car.instance.Rigidbody.AddForce(Car.instance.transform.forward * Car.instance.Rigidbody.mass * boostByMassMultiplier, ForceMode.Impulse);

        //Removes the specified gameobject from the game, visually, meaning it still exists but
        //its colliders and meshrenderers are deactivated
        //More details in game manager method
        GameManager.instance.UnloadObject(this.gameObject);
    }
}
