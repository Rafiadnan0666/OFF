using UnityEngine;
using UnityEngine.AI;

public class EnemyRobot : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;
    public float health = 50f;

    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;
    public GameObject projectile;

    public enum Ngambang { False, True }
    public Ngambang ngambangState;

    // States
    public float sightRange = 20f, attackRange = 10f;
    private bool playerInSightRange, playerInAttackRange;

    private Vector3 currentPos;
    private float damageAmount;

    public GameObject Explode;

    public GameObject Flash;

    private void Awake()
    {
        player = GameObject.Find("Player ").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (ngambangState == Ngambang.True)
        {
            currentPos = transform.position;
            float newY = Mathf.Sin(Time.time * 3) * currentPos.y + 0.5f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        playerInSightRange = IsPlayerVisible();
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patrol();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInAttackRange && playerInSightRange)
            AttackPlayer();

        damageAmount= Random.Range(5,10);
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Instantiate(Flash, transform.position + transform.forward,Quaternion.identity);
            Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private bool IsPlayerVisible()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 20f;
            DestroyEnemy();
        }
    }
   

    private void DestroyEnemy()
    {
        Instantiate(Explode, transform.forward, transform.rotation);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
