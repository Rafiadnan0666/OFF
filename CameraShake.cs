using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public float HowShake;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        originalPosition = cameraTransform.localPosition;
        originalRotation = cameraTransform.localRotation;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-HowShake, HowShake) * magnitude;
            float y = Random.Range(-HowShake, HowShake) * magnitude;
            float z = Random.Range(-HowShake, HowShake) * magnitude;

            cameraTransform.localPosition = new Vector3(x, y, z) + originalPosition;
            cameraTransform.localRotation = new Quaternion(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                1f) * originalRotation;

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPosition;
        cameraTransform.localRotation = originalRotation;
    }
}
