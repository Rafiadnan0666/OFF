using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RoamRandomAnimal : MonoBehaviour
{
    public NavMeshAgent navAgent;  // NavMesh Agent for the cat
    public GameObject player;      // The player object
    public Animator kucingAnimator; // Animator for the cat

    // Animation states for the cat
    public string walkForwardAnimation = "walk_forward";
    public string jumpAnimation = "jump";
    public string sitAnimation = "sit";
    public string runForwardAnimation = "run_forward";

    // Audio for the cat
    public AudioSource animalAudio;
    public AudioClip meowClip;
    public AudioClip walkClip;

    // Roaming settings
    public float roamRadius = 20f;
    public float approachDistance = 10f;
    public float timeToApproachPlayer = 5f;

    private float distanceToPlayer;
    private bool isJumping = false;
    private bool isApproachingPlayer = false;
    private float approachTimer = 0f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (!navAgent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not placed on a valid NavMesh.");
        }
    }

    void Update()
    {
        DetectPlayer();
        HandleMovement();
    }

    // Detect the player and approach them if within a certain distance
    private void DetectPlayer()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= approachDistance && !isApproachingPlayer)
        {
            isApproachingPlayer = true;
            approachTimer = timeToApproachPlayer;
            animalAudio.PlayOneShot(meowClip);
            navAgent.SetDestination(player.transform.position);
            kucingAnimator.SetTrigger(runForwardAnimation);
        }
    }

    // Handle the movement of the cat, including roaming around and approaching the player
    private void HandleMovement()
    {
        if (isApproachingPlayer)
        {
            approachTimer -= Time.deltaTime;
            if (approachTimer <= 0f)
            {
                isApproachingPlayer = false;
            }
        }
        else
        {
            RoamAround();
        }
    }

    // Roam randomly in the environment
    private void RoamAround()
    {
        if (!isJumping && navAgent.isOnNavMesh) // Check if the agent is on the NavMesh
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
            {
                navAgent.SetDestination(hit.position);
                kucingAnimator.SetBool(walkForwardAnimation, true);
                animalAudio.clip = walkClip;
                if (!animalAudio.isPlaying)
                    animalAudio.Play();

                // Randomly decide to make the cat jump
                if (Random.Range(0, 100) < 10)
                {
                    StartCoroutine(Jump());
                }
            }
        }
    }

    // Simulate a jump action with animation
    private IEnumerator Jump()
    {
        isJumping = true;
        kucingAnimator.SetTrigger(jumpAnimation);
        Rigidbody rb = GetComponent<Rigidbody>();

        // Apply force to simulate jump
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);

        yield return new WaitForSeconds(2f);
        isJumping = false;
    }

    // Draw gizmos to visualize the roam radius and distance to the player
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, roamRadius); // Show roaming radius
        if (player != null)
        {
            Gizmos.DrawLine(transform.position, player.transform.position); // Show line to player
        }
    }
}
