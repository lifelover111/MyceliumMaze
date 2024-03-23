using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public Animator chestAnimator; 
    public float interactionDistance = 1f; 
    private PlayerManager player;

    void Start()
    {
        chestAnimator.SetBool("isOpen", false);
    }

    private void FixedUpdate()
    {
        var colliders = Physics.OverlapSphere(transform.position, interactionDistance);
        if (colliders is null || colliders.Length == 0)
        {
            return;
        }
        var playerColliders = colliders.Where(c => c.CompareTag("Player"));

        if (playerColliders is null || playerColliders.Count() == 0)
        {
            if (player is not null)
            {
                player.OnInteract -= OpenChest;
                player = null;
            }
            return;
        }

        if (playerColliders.Count() > 0)
        {
            if (player is not null && playerColliders.Select(c => c.gameObject).Contains(player.gameObject))
                return;

            player = playerColliders.First().GetComponent<PlayerManager>();
            player.OnInteract += OpenChest;
        }
    }


    void OpenChest()
    {
        chestAnimator.enabled = true;
        chestAnimator.SetBool("isOpen", true);
    }


}
