using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGeser : MonoBehaviour
{
    public GameObject pintu;
    public Vector3 openPosition; 
    public Vector3 closedPosition; 
    public float speed = 2.0f; 
    private bool isOpening = false; 

    void Start()
    {

        if (closedPosition == Vector3.zero)
        {
            closedPosition = pintu.transform.position;
        }
    }


    void Update()
    {
        if (isOpening)
        {

            pintu.transform.position = Vector3.Lerp(pintu.transform.position, openPosition, speed * Time.deltaTime);

      
            if (Vector3.Distance(pintu.transform.position, openPosition) < 0.01f)
            {
                pintu.transform.position = openPosition;
                isOpening = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
     
        if (other.CompareTag("Card"))
        {
            isOpening = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOpening)
        {
               
        }
    }
}
