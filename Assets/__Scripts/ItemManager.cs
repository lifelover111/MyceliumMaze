using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    [SerializeField] public Transform OffHand;
    PlayerManager player;
    public List<PassiveItem> itemsInInventory = new List<PassiveItem>();
    public ActiveItem activeItem;
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        activeItem = Instantiate(activeItem);
        activeItem.PickUp(player);
    }

    private void Update()
    {
        ProcessPassiveEffects();
    }

    public void AddItem(Item item)
    {
        if (item is PassiveItem passiveItem)
        {
            PassiveItem copy = Instantiate(passiveItem);
            itemsInInventory.Add(copy);
            copy.PickUp(player);
        }
        else if(item is ActiveItem activeItem)
        {
            ActiveItem copy = Instantiate(activeItem);
            //TODO: дроп предыдущего предмета

            this.activeItem = copy;
            copy.PickUp(player);
        }
        else
        {
            throw new System.Exception(item + " is not passive nor active");
        }
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

    private void ProcessPassiveEffects()
    {
        foreach (PassiveItem item in itemsInInventory)
        {
            item.ProcessPassiveEffect(player);
        }
    }
}
