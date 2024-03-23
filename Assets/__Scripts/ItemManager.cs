using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] public Transform OffHand;
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

    public void AddToHand(GameObject item)
    {
        item.transform.SetParent(OffHand, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.rotation = Quaternion.FromToRotation(player.playerLocomotionManager.GetForward(), player.transform.forward);
    }

    public void ClearOffHand()
    {
        for (int i = 6; i < OffHand.childCount; i++)
        {
            Destroy(OffHand.GetChild(i));
        }
    }
}
