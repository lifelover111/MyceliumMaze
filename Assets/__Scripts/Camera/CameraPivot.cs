using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public static CameraPivot instance;

    public Transform target;
    [SerializeField] float smoothing;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        var distance = (transform.position - target.position).magnitude;
        if (distance < .3f)
        {
            return;
        }

        else if (distance > 50)
        {
            transform.position = target.position;
            return;
        }    

        transform.position = Vector3.Lerp(transform.position, target.position, smoothing*Time.deltaTime);   
    }
}
