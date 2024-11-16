
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SwayAndWob : MonoBehaviour
{
    public float swayAmount = 0.02f;    
    public float maxSwayAmount = 0.05f;  
    public float smoothAmount = 10f;   

    public float wobbleAmount = 1.0f;  
    public float wobbleSpeed = 1.5f;    

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 swayOffset;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        UpdateSway();

        UpdateWobble();
    }

    void UpdateSway()
    {

        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        moveX = Mathf.Clamp(moveX, -maxSwayAmount, maxSwayAmount);
        moveY = Mathf.Clamp(moveY, -maxSwayAmount, maxSwayAmount);

    
        Vector3 targetPosition = new Vector3(moveX, moveY, 0f);

        swayOffset = Vector3.Lerp(swayOffset, targetPosition, Time.deltaTime * smoothAmount);

        transform.localPosition = initialPosition + swayOffset;
    }

    void UpdateWobble()
    {
        float wobbleZ = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, wobbleZ);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smoothAmount);
    }
}

