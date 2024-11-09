using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampuKedip : MonoBehaviour
{
    public Light lampu;

    float interval = 1;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            lampu.enabled = !lampu.enabled;
            interval = Random.Range(0f, 1f);
            timer = 0;
        }
    }
}
