using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tele : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (this.gameObject.CompareTag("PLayer"))
        {
            SceneManager.LoadScene("Game");
        }
    }
}
