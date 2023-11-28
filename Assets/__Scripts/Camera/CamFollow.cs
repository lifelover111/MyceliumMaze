using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    Vector3 delta;
    [SerializeField] Transform player;
    private void Awake()
    {
        delta = transform.position;
    }

    private void Update()
    {
        transform.position = player.position + delta;
    }
}
