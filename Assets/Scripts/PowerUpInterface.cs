using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PowerUpInterface
{
    //This method gets called in the OnTriggerEnter or OnCollisionEnter method of the car script
    void Activate(Car car);
}