using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    private DoorSelector doorSelector;

    public event Action OnPlayerTriggerEnter;

    private void Awake()
    {
        doorSelector = GetComponentInChildren<DoorSelector>();
    }

    public void CloseDoor()
    {
        doorSelector.CloseDoor();
    }

    public void OpenDoor()
    {
        doorSelector.OpenDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        OnPlayerTriggerEnter?.Invoke();
        OnPlayerTriggerEnter = null;
    }
}
