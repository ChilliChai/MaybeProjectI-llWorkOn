using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private float gravitationalPull = 300f;

    public float GravitationalPull => gravitationalPull;
}
