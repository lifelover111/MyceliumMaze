using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    List<Item> itemsInInventory;
    public void AddItem(Item item)
    {
        Item copy = item.Copy();
        itemsInInventory.Add(copy);
        copy.PickUp();

        //в конце у копии вызывается .PickUp();
    }

    public void RemoveItem(Item item)
    {
        itemsInInventory.Remove(item);
        item.Remove();

        //в конце у item вызывается .Remove();
    }

}
