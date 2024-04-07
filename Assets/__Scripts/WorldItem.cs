using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Properties")]
    public float interactionDistance = 3f;

    [SerializeField] private Sprite healFlaskIcon;
    [SerializeField] private Item item;
    [SerializeField] private Sprite icon;
    private PlayerManager player;

    private bool _isHealingFlask = false;

    private void Start()
    {
        if(icon is not null)
            GetComponent<SpriteRenderer>().sprite = icon;
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
                player.OnInteract -= GiveItem;
                player = null;
            }
            return;
        }

        if (playerColliders.Count() > 0)
        {
            if (player is not null && playerColliders.Select(c => c.gameObject).Contains(player.gameObject))
                return;

            player = playerColliders.First().GetComponent<PlayerManager>();
            player.OnInteract += GiveItem;
        }

    }

    public void SetHealingFlask()
    {
        icon = healFlaskIcon;
        _isHealingFlask = true;
    }

    public void SetItem(Item item)
    {
        this.item = item;
        icon = item.icon;
    }

    void GiveItem()
    {
        if(_isHealingFlask)
        {
            player.playerStatsManager.IncreaseFlaskCount();
            player.OnInteract -= GiveItem;
            Destroy(gameObject);
            return;
        }

        player.itemManager.AddItem(item);
        player.OnInteract -= GiveItem;
        Destroy(gameObject);
    }

}
