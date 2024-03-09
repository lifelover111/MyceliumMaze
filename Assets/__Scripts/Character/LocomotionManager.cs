using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    [Header("Flags")]
    public bool isDashing = false;
    protected virtual void Awake()
    {

    }

    public virtual Vector3 GetForward()
    {
        return transform.forward;
    }
}
