using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGeser : MonoBehaviour
{
    public Transform door;
    public Vector3 doorOpenPosition;
    public Vector3 doorClosedPosition;
    public Light doorLight;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public string requiredCardTag = "Card";
    public string keyTag = "Key";
    private AudioSource audioSource;
    private bool isOpen = false;
    private bool isMoving = false;
    public float doorSpeed = 2.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        doorClosedPosition = door.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isMoving && (collision.gameObject.CompareTag(requiredCardTag) || collision.gameObject.CompareTag(keyTag)))
        {
            isOpen = true;
            StartCoroutine(SlideDoor(doorOpenPosition));
            ActivateLight(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!isMoving && (collision.gameObject.CompareTag(requiredCardTag) || collision.gameObject.CompareTag(keyTag)))
        {
            isOpen = false;
            StartCoroutine(SlideDoor(doorClosedPosition));
            ActivateLight(false);
        }
    }
    
    private IEnumerator SlideDoor(Vector3 targetPosition)
    {
        isMoving = true;

        if (audioSource != null)
        {
            if (isOpen && doorOpenSound != null)
            {
                audioSource.PlayOneShot(doorOpenSound, 1.0f);
            }
            else if (!isOpen && doorCloseSound != null)
            {
                audioSource.PlayOneShot(doorCloseSound, 1.0f);
            }
        }

        while (Vector3.Distance(door.position, targetPosition) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPosition, doorSpeed * Time.deltaTime);
            yield return null;
        }

        door.position = targetPosition;
        isMoving = false;
    }

    private void ActivateLight(bool lightState)
    {
        if (doorLight != null)
        {
            doorLight.gameObject.SetActive(lightState);
        }
    }

}
