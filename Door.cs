using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false; 
    public float rotationSpeed = 2f;  
    public float openAngle = 90f;  

    public GameObject doorModel; 
    public GameObject pivot; 

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
       
        if (pivot == null || doorModel == null)
        {
            Debug.LogError("Pivot or DoorModel not set in the inspector.");
            return;
        }

      
        closedRotation = pivot.transform.rotation;
        openRotation = Quaternion.Euler(pivot.transform.eulerAngles.x, pivot.transform.eulerAngles.y + openAngle, pivot.transform.eulerAngles.z);
    }

    public void ToggleDoor()
    {
        StopAllCoroutines();
        if (isOpen)
        {
       
            StartCoroutine(RotateDoor(closedRotation));
            isOpen = false;
        }
        else
        {
           
            StartCoroutine(RotateDoor(openRotation));
            isOpen = true;
        }
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(pivot.transform.rotation, targetRotation) > 0.1f)
        {
            pivot.transform.rotation = Quaternion.Slerp(pivot.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        pivot.transform.rotation = targetRotation;  
    }
}
