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

        //� ����� � ����� ���������� .PickUp();
    }

    public void RemoveItem(Item item)
    {
        itemsInInventory.Remove(item);
        item.Remove();

        //� ����� � item ���������� .Remove();
    }

}
