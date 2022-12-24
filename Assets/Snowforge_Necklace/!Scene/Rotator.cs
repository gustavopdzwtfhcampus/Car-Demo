using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public float speed = 15.00f;

    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
