using UnityEngine;
using UnityEngine.AI;

public class MobilRand : MonoBehaviour
{
    [Header("Car Settings")]
    public float stopDistance = 5f; // Distance to stop when player is detected
    public float honkVolume = 1f; // Honk sound volume
    public AudioClip honkSound; // Honk sound clip
    public AudioClip engineSound; // Engine sound clip
    public float engineSoundPitch = 1f; // Pitch for engine sound

    [Header("Car Components")]
    public NavMeshAgent navMeshAgent; // NavMeshAgent for the car's movement
    public AudioSource audioSource; // AudioSource for playing sounds
    public Transform player; // Player's transform to track the player

    private bool isPlayerNearby = false;

    private void Start()
    {
        
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                Debug.LogError("NavMeshAgent is missing on the car.");
                return; // Exit the Start method if there's no NavMeshAgent
            }
        }

        // Check if the player transform is assigned, if not, try to find the player by tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player not found or not tagged correctly.");
                return; // Exit the Start method if player is not found
            }
        }

        // Check if AudioSource is assigned, if not, try to get it from the car GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is missing on the car.");
                return; // Exit the Start method if AudioSource is missing
            }
        }

        // Set up the engine sound
        audioSource.clip = engineSound;
        audioSource.loop = true;
        audioSource.pitch = engineSoundPitch;
        audioSource.Play();
    }

    private void Update()
    {
      
        if (!isPlayerNearby)
        {
         
            //navMeshAgent.SetDestination(new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)));
        }

        // Check for player proximity
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopDistance && !isPlayerNearby)
        {
            isPlayerNearby = true;
            StopCarAndHonk();
        }
        else if (distanceToPlayer > stopDistance && isPlayerNearby)
        {
            isPlayerNearby = false;
            StartCar();
        }
    }

    private void StopCarAndHonk()
    {
        // Stop the car
        navMeshAgent.isStopped = true;

        // Play the honk sound
        if (!audioSource.isPlaying || audioSource.clip != honkSound)
        {
            audioSource.clip = honkSound;
            audioSource.loop = false;
            audioSource.PlayOneShot(honkSound, honkVolume);
        }
    }

    private void StartCar()
    {
        // Resume moving the car
        navMeshAgent.isStopped = false;

        // Play engine sound if not playing
        if (audioSource.clip != engineSound || !audioSource.isPlaying)
        {
            audioSource.clip = engineSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
