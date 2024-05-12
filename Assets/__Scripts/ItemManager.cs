using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] GameObject worldItemPrefab;
    [SerializeField] public Transform OffHand;
    PlayerManager player;
    public List<PassiveItem> itemsInInventory = new List<PassiveItem>();
    public ActiveItem activeItem;
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        ProcessPassiveEffects();
    }

    public void AddItem(Item item)
    {
        player.soundManager.PlaySound(SoundBank.instance.itemPickup);

        player.playerUIController.NotifyPlayerItemAdded(item);

        if (item is PassiveItem passiveItem)
        {
            PassiveItem copy = Instantiate(passiveItem);
            itemsInInventory.Add(copy);
            copy.PickUp(player);
        }
        else if(item is ActiveItem activeItem)
        {
            if (this.activeItem != null)
            {
                GameObject go = Instantiate(worldItemPrefab);
                WorldItem worldItem = go.GetComponent<WorldItem>();
                worldItem.SetItem(this.activeItem);
                go.transform.position = transform.position + Vector3.up;
            }

            ActiveItem copy = Instantiate(activeItem);
            this.activeItem = copy;
            copy.PickUp(player);
            player.playerUIController.activeItemIconRenderer.sprite = copy.icon;
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
        item.transform.localPosition = Vector3.zero + 0.005f*Vector3.up + 0.01f*Vector3.forward;
        item.transform.localRotation = Quaternion.Euler(-90, 0, 0); //Quaternion.FromToRotation(player.playerLocomotionManager.GetForward(), player.transform.forward);
    }

    public void ClearOffHand()
    {
        for (int i = 6; i < OffHand.childCount; i++)
        {
            Destroy(OffHand.GetChild(i).gameObject);
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
