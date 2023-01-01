using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotat : MonoBehaviour
{
    Rigidbody rb;
    Vector3 angularVelocity;
    public float rot;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        angularVelocity = new Vector3(5, rot, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion addRotation = Quaternion.Euler(angularVelocity * Time.deltaTime);
        rb.MoveRotation(rb.rotation * addRotation);
    }
}