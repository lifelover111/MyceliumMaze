using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public Animator chestAnimator; 
    public float interactionDistance = 1f; 
    private bool isOpen = false; 
    private GameObject player;

    void Start()
    {
        chestAnimator.SetBool("isOpen", false);
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerNearby())
        {
            if (!isOpen)
            {
                OpenChest();
                Debug.Log("Открыто");
            }
            else
            {
                //CloseChest();
            }
        }
    }


    void OpenChest()
    {
        chestAnimator.enabled = true;
        chestAnimator.SetBool("isOpen", true);
        isOpen = true; 
    }

    //void CloseChest()
    //{
    //    chestAnimator.SetBool("isOpen", false);
    //    isOpen = false; 
    //}

    bool IsPlayerNearby()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= interactionDistance;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }
}
