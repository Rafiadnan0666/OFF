using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;                   // Bullet speed
    public float damage = 10f;                 // Damage dealt by the bullet
    public Transform target;                   // Target to aim at
    public TargetType currentTarget;           // Type of target
    public GameObject Explode;                 // Explosion prefab
    public Meledak dak;                        // Whether the bullet explodes on impact

    public enum TargetType { Player, Enemy }
    public enum Meledak { ya, ga }

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Bullet requires a Rigidbody component!");
            return;
        }

        // Determine bullet behavior based on the target type
        if (currentTarget == TargetType.Enemy)
        {
            // If targeting an enemy, simply shoot forward
            rb.velocity = transform.forward * speed;
        }
        else if (currentTarget == TargetType.Player)
        {
            // If targeting the player, find and aim at the player
            if (target == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    target = playerObject.transform;
                }
            }

            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                rb.velocity = direction * speed;
            }
            else
            {
                rb.velocity = transform.forward * speed;
            }
        }
    }

    private void Update()
    {
        // Destroy bullet after 1 second if no collision occurs
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // Handle player damage
                // player.TakeDamage(damage);
            }
            if (dak == Meledak.ya)
            {
                Instantiate(Explode, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Handle enemy damage
            // Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage((int)damage);
            // }
            if (dak == Meledak.ya)
            {
                Instantiate(Explode, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        else
        {
            // Handle collision with other objects
            Instantiate(Explode, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
