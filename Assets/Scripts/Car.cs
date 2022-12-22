using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    Rigidbody rb;
    Vector3 angularVelocity;
    public float speed;
    public float rotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        angularVelocity = new Vector3(0, rotation, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Vertical")!=0)
        {
            rb.AddForce(transform.forward * Input.GetAxis("Vertical") * (speed*10) * Time.deltaTime);
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            Quaternion addRotation = Quaternion.Euler(angularVelocity * Input.GetAxis("Horizontal") * Time.deltaTime);
            rb.MoveRotation(rb.rotation * addRotation);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            this.GetComponent<AudioSource>().Play();
        }
    }
}
