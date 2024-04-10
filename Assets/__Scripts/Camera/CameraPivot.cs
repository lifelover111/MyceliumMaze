using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothing;

    void Update()
    {
        var distance = (transform.position - target.position).magnitude;
        if (distance < .3f)
        {
            return;
        }

        else if (distance > 6)
        {
            transform.position = target.position;
            return;
        }    

        transform.position = Vector3.Lerp(transform.position, target.position, smoothing*Time.deltaTime);   
    }
}
