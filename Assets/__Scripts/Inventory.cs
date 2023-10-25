using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    List<Item> itemsInInventory = new List<Item>();
    public void AddItem(Item item)
    {
        Item copy = item.Copy();
        itemsInInventory.Add(copy);

        copy.PickUp();
    }

    public void RemoveItem(Item item)
    {
        itemsInInventory.Remove(item);

        item.Remove();
    }

}
