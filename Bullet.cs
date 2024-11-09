using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f; 
    public float acceleration = 5f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed; 
    }

    void Update()
    {
      
        rb.velocity += transform.forward * acceleration * Time.deltaTime;
    }
}
