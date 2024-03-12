using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LocomotionManager : MonoBehaviour
{
    private CharacterManager character;
    [Header("Flags")]
    public bool isDashing = false;
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual Vector3 GetForward()
    {
        return Quaternion.FromToRotation(Vector3.forward, character.forward) * transform.forward;
    }
}
