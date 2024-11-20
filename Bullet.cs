using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;                   // Speed of the bullet
    public float damage = 10f;                 // Damage dealt by the bullet
    public Transform target;                   // Target for the bullet
    public TargetType currentTarget;           // Type of target (Player or Enemy)
    public GameObject Explode;                 // Explosion effect prefab
    public Meledak dak;                        // Explosion type

    public enum TargetType { Player, Enemy }
    public enum Meledak { ya, ga }

    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        if (rb == null)
        {
            Debug.LogError("Bullet requires a Rigidbody component!");
            return;
        }

        if (currentTarget == TargetType.Enemy)
        {
            if (target == null && mainCamera != null)
            {
                // Perform a raycast from the center of the screen
                Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // If a hit is detected, aim the bullet at the hit point
                    target = new GameObject("TemporaryTarget").transform;
                    target.position = hit.point;
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

        // Direct the bullet towards the target if one exists
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
        else
        {
            // Launch the bullet forward if no target is available
            rb.velocity = transform.forward * speed;
        }
    }

    private void Update()
    {
        // Destroy the temporary target if it was created
        if (target != null && target.name == "TemporaryTarget" && Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Destroy(target.gameObject);
        }

        // Destroy the bullet after 1 second
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // Apply damage to the player
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
            // Uncomment and customize this if you have an Enemy script with a TakeDamage method
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
    }
}
