using UnityEngine;

public class CollisionGravity : MonoBehaviour
{
    private Rigidbody rb; //physics object

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.useGravity = true; //if colliding turn on the gravity
    }
}