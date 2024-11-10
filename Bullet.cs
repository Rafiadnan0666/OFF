using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f; // Initial speed of the bullet
    public float acceleration = 5f; // Additional force applied over time
    public enum TypeLuru { Parabola, Lurus } // Enum to choose the bullet type
    public TypeLuru typeLuru; // Variable to set bullet type

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed; // Initial velocity in the forward direction
    }

    void Update()
    {
        // Apply linear acceleration for 'Lurus' type
        if (typeLuru == TypeLuru.Lurus)
        {
            rb.velocity += transform.forward * acceleration * Time.deltaTime;
        }

        // Apply parabolic motion
        if (typeLuru == TypeLuru.Parabola)
        {
            ApplyParabolicMotion();
        }
    }

    private void ApplyParabolicMotion()
    {
        // Parabolic motion involves gravity naturally applied by the physics engine
        // Ensure gravity is being simulated
        rb.useGravity = true;
    }
}
