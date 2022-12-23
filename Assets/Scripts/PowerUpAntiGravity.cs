using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAntiGravity : MonoBehaviour, PowerUp
{
    public void Activate()
    {
        GameManager.instance.PlaySound(this.gameObject);
        //Removes the specified gameobject from the game
        GameManager.instance.UnloadObject(this.gameObject);

    }
}
