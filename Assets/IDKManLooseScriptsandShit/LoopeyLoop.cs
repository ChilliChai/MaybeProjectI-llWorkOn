using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopeyLoop : MonoBehaviour
{
    public string IinniiScene;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            SceneManager.LoadScene(IinniiScene);
        }
    }
}
