using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Woosh : MonoBehaviour
{
    public string WaterTrip;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            SceneManager.LoadScene(WaterTrip);
        }
    }
}

