using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    PlayerManager player;
    List<Item> itemsInInventory = new List<Item>();
    public Item activeItem;
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void AddItem(Item item)
    {
        Item copy = Instantiate(item);
        itemsInInventory.Add(copy);

        copy.PickUp(player);
    }

    public void RemoveItem(Item item)
    {
        var removingItem = itemsInInventory.First(i => i.id == item.id);
        removingItem.Remove(player);
        itemsInInventory.Remove(removingItem);
    }

    public void TryUseItem()
    {
        if (player.isPerformingAction)
            return;

        if (activeItem is null)
            return;

        activeItem.Use(player);
    }
}
