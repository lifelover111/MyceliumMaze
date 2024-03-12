using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    Animator fogAnimator;
    Collider fogCollider;
    public void Awake()
    {
        fogAnimator = GetComponent<Animator>();
        fogCollider = GetComponent<Collider>();

        
    }

    public void CloseDoor()
    {
        if (fogAnimator is null)
        {
            fogAnimator = GetComponent<Animator>();
        }

        if (fogCollider is null)
        {
            fogCollider = GetComponent<Collider>();
        }
        fogCollider.enabled = true;
        fogAnimator.enabled = true;
    }
    public void OpenDoor()
    {
        fogCollider.enabled = false;
        fogAnimator.SetTrigger("Open");
    }

    public void DisabledAnimation()
    {
        fogAnimator.enabled = false;
        gameObject.SetActive(false);
    }
}
