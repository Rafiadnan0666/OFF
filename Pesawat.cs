using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pesawat : MonoBehaviour
{
    public Transform landingStartPoint; 
    public Transform landingEndPoint;   
    public float landingDuration = 5.0f; 
    public AudioSource planeSound;     

    private float elapsedTime = 0.0f;
    private bool isLanding = true;

    void Update()
    {
        if (isLanding)
        {
            elapsedTime += Time.deltaTime;

          
            float progress = Mathf.Clamp01(elapsedTime / landingDuration);
            transform.position = Vector3.Lerp(landingStartPoint.position, landingEndPoint.position, progress);

    
            if (progress >= 1.0f)
            {
                isLanding = false;
                if (planeSound.isPlaying)
                {
                    planeSound.Stop(); 
                }
            }
        }
    }


    public void StartLanding()
    {
        if (planeSound != null)
        {
            planeSound.Play(); 
        }

        elapsedTime = 0.0f;
        isLanding = true;
    }
}
