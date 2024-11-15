using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public Transform target;
    public TargetType currentTarget;

    public enum TargetType { Player, Enemy }

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (currentTarget == TargetType.Enemy)
        {
            if (target == null)
            {
                // Launch the projectile forward if no target is specified
                if (rb != null)
                {
                    rb.velocity = transform.forward * speed;
                }
            }
        }
        else if (currentTarget == TargetType.Player)
        {
            if (target == null)
            {
                // Find the player by tag if no specific target is assigned
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    target = playerObject.transform;
                }
            }
        }

        // Move the projectile towards the target if available
        if (target != null && rb != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
    }

    private void Update()
    {
        Destroy(this.gameObject,1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // Apply damage to the player
                //player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Uncomment and customize this if you have an Enemy script with a TakeDamage method
            // Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage((int)damage);
            // }
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
