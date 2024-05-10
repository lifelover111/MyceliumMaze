using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField] GameObject worldItemPrefab;
    public Animator chestAnimator; 
    public float interactionDistance = 1f; 
    private PlayerManager player;
    private bool isOpen = false;

    void Start()
    {
        chestAnimator.SetBool("isOpen", false);
    }

    private void FixedUpdate()
    {
        if (isOpen)
            return;

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
        isOpen = true;
        chestAnimator.enabled = true;
        chestAnimator.SetBool("isOpen", true);
        player.soundManager.PlaySound(SoundBank.instance.openingChestSound);
        DropItem();
        player.OnInteract -= OpenChest;
    }

    void DropItem()
    {
        Item item = ItemsInGameManager.instance.GetRandomItem();
        if (item is null)
        {
            GameObject healFlaskGo = Instantiate(worldItemPrefab);
            WorldItem healFlask = healFlaskGo.GetComponent<WorldItem>();
            healFlask.SetHealingFlask();
            healFlaskGo.transform.position = transform.position + Vector3.up;
            return;
        }

        GameObject go = Instantiate(worldItemPrefab);
        WorldItem worldItem = go.GetComponent<WorldItem>();
        worldItem.SetItem(item);
        go.transform.position = transform.position + Vector3.up;
    }

}
