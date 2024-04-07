using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothing;

    void Update()
    {
        if ((transform.position - target.position).magnitude < .3f)
            return;

        transform.position = Vector3.Lerp(transform.position, target.position, smoothing*Time.deltaTime);   
    }
}
