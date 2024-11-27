using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Kucing : MonoBehaviour
{
    public NavMeshAgent kucing; // The NavMeshAgent component
    public GameObject player; // Reference to the player
    public Animator kucingAnimator; // Animator component
    public AudioSource kucingAudio; // AudioSource for sound effects
    public AudioClip meowClip; // Audio when the cat meows
    public AudioClip walkClip; // Audio when the cat walks
    public float roamRadius = 20f; // Roaming radius for random movement
    public float timeToApproachPlayer = 5f; // Time to approach the player
    public float health = 100f; // Cat's health

    private float distanceToPlayer; // Distance between the cat and the player
    private bool isJumping = false; // Flag to determine if the cat is jumping
    private bool isApproachingPlayer = false; // Flag for approaching the player
    private float approachTimer = 0f; // Timer for how long the cat follows the player

    void Start()
    {
        kucing = GetComponent<NavMeshAgent>();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }

    void Update()
    {
        DetectPlayer();
        HandleMovement();

        // If the cat's health is 0 or less, it dies
        if (health <= 0)
        {
            Die();
        }
    }

    private void DetectPlayer()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= 10f && !isApproachingPlayer)
        {
            isApproachingPlayer = true;
            approachTimer = timeToApproachPlayer;
            kucingAudio.PlayOneShot(meowClip);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 20f; // Reduce health when hit by a bullet
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }

    private void HandleMovement()
    {
        if (isApproachingPlayer)
        {
            approachTimer -= Time.deltaTime;
            kucing.destination = player.transform.position;

            // Play the "crawl_fast" animation when approaching the player
            kucingAnimator.SetBool("crawl", false);
            kucingAnimator.SetBool("crawl_fast", true);

            kucingAudio.clip = walkClip;
            if (!kucingAudio.isPlaying)
                kucingAudio.Play();

            if (approachTimer <= 0f)
            {
                isApproachingPlayer = false;
                RoamAround();
            }
        }
        else
        {
            RoamAround();
        }
    }

    private void RoamAround()
    {
        if (!isJumping)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
            Vector3 finalPosition = hit.position;

            kucing.destination = finalPosition;

            // Play the "crawl" animation when roaming
            kucingAnimator.SetBool("crawl", true);
            kucingAnimator.SetBool("crawl_fast", false);

            kucingAudio.clip = walkClip;
            if (!kucingAudio.isPlaying)
                kucingAudio.Play();

            if (Random.Range(0, 100) < 10)
            {
                StartCoroutine(Jump());
            }
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;

        // Trigger the "pounce" animation when jumping
        kucingAnimator.SetTrigger("pounce");

        yield return new WaitForSeconds(2f); // Duration of the jump animation
        isJumping = false;
    }

    private void Die()
    {
        // Play the "die" animation when the cat dies
        kucingAnimator.SetTrigger("die");

        kucing.enabled = false; // Disable the NavMeshAgent
        kucingAudio.Stop(); // Stop audio playback
        Debug.Log("The cat has died.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, player.transform.position);
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }
}
