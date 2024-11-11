using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 50f;
    public float lifetime = 5f;
    public float destroyDelayAfterHit = 0.5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * bulletSpeed;
        }
        StartCoroutine(DestroyAfterLifetime());
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optional: Add logic for what happens on impact, like dealing damage
        Debug.Log($"Bullet hit: {collision.collider.name}");

        // Destroy bullet after a short delay
        StartCoroutine(DestroyAfterHit());
    }

    IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    IEnumerator DestroyAfterHit()
    {
        yield return new WaitForSeconds(destroyDelayAfterHit);
        Destroy(gameObject);
    }
}
