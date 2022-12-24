using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform orbitAround;
    public float distance;
    public float speed;
    float dt = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = new Vector3(Mathf.Cos(dt)*distance + orbitAround.position.x, orbitAround.position.y, Mathf.Sin(dt)*distance + orbitAround.position.z);
        dt += Time.deltaTime * speed;
    }
}
