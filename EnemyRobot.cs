using UnityEngine;
using UnityEngine.AI;

public class EnemyRobot : MonoBehaviour
{
    public Transform player;
    public float sightRange = 15f;
    public float detectionRange = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float inaccuracy = 5f; // Adjust for shooting inaccuracy
    public Transform hidingSpot; // Assign the corner hiding spot in the inspector

    private bool playerDetected = false;
    private float floatSpeed = 2f;
    private float floatHeight = 0.5f;
    private float lastShotTime;
    private bool isHiding = true; // For the hiding variant
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Floating effect
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Check player sight detection
        if (Vector3.Distance(transform.position, player.position) <= sightRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, sightRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    playerDetected = true;
                    isHiding = false; // Robot stops hiding when it detects the player
                }
            }
        }

        // Check player sound detection
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            playerDetected = true;
            isHiding = false;
        }

        // Move to hiding spot if in hiding state
        if (isHiding)
        {
            agent.SetDestination(hidingSpot.position);
            if (Vector3.Distance(transform.position, hidingSpot.position) < 1f) // Check if it reached the hiding spot
            {
                agent.isStopped = true; // Stop the agent from moving further
            }
        }

        // Shooting logic
        if (playerDetected && Time.time - lastShotTime >= shootInterval)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    void Shoot()
    {
        // Introduce inaccuracy by modifying the direction
        Vector3 shootingDirection = (player.position );
        //shootingDirection.x += Random.Range(-inaccuracy, inaccuracy) * 0.01f;
        //shootingDirection.y += Random.Range(-inaccuracy, inaccuracy) * 0.01f;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = shootingDirection * 20f; // Adjust speed as needed
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, player.transform.position);
        Gizmos.DrawWireSphere(transform.position, sightRange);
       
    }
}

//-firePoint.position
