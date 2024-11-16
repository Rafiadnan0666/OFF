using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Kucing : MonoBehaviour
{
    public NavMeshAgent kucing;
    public GameObject player;
    public Animator kucingAnimator;
    public AudioSource kucingAudio;
    public AudioClip meowClip;
    public AudioClip walkClip;
    public float roamRadius = 20f;
    public float timeToApproachPlayer = 5f;
    public float health = 100f;

    private float distanceToPlayer;
    private bool isJumping = false;
    private bool isApproachingPlayer = false;
    private float approachTimer = 0f;

    void Start()
    {
        kucing = GetComponent<NavMeshAgent>();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }

    void Update()
    {
        DetectPlayer();
        HandleMovement();

        
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
        
        if (collision.gameObject.CompareTag("bullet"))
        {
          
            health -= 20f; 

         
            Destroy(collision.gameObject);
        }
    }

    private void HandleMovement()
    {
        if (isApproachingPlayer)
        {
            approachTimer -= Time.deltaTime;
            kucing.destination = player.transform.position;
            kucingAnimator.SetBool("isWalking", true);
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
            kucingAnimator.SetBool("isWalking", true);
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
        kucingAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(2f);
        isJumping = false;
    }

    private void Die()
    {
        // klo mati ngapain
        kucingAnimator.SetTrigger("Die");
        kucing.enabled = false;
        kucingAudio.Stop(); 
        Debug.Log("The cat has died.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, player.transform.position);
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }
}
