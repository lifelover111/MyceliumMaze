using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class ItemDealer : MonoBehaviour
{
    [Header("Properties")]
    public const int shopItemSize = 3;
    public float minPriceFactor = 0.85f;
    public float maxPriceFactor = 1.15f;
    public float interactionDistance = 3f;
    private PlayerInputManager playerInputManager;
    private Item[] _itemsToPurchase = new Item[shopItemSize];
    public Dictionary<Item, int> itemPrices = new Dictionary<Item, int>();
    
    private PlayerManager player;

    public Item[] itemsToPurchase => _itemsToPurchase;

    private void Start()
    {
        GetItems();
        SetPrices();
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void FixedUpdate()
    {
        FindPlayers();
    }




    private void StartPurchase()
    {
        var purchaseWindow = player.GetPurchaseUI();
        purchaseWindow.gameObject.SetActive(true);
        purchaseWindow.SetItemDealer(this);
        purchaseWindow.UpdateItems();

    }

    public void EndPurchase()
    {
        var purchaseWindow = player.GetPurchaseUI();
        purchaseWindow.gameObject.SetActive(false);
    }

    public void PurchaseItem(int itemIndex)
    {
        if (player == null)
            return;

        Item itemToPurchase = _itemsToPurchase[itemIndex];

        if (itemToPurchase is null)
            return;

        var purchaseWindow = player.GetPurchaseUI();
        int price = itemPrices[itemToPurchase];
        if (player.TryTakeSpores(price))
        {
            player.itemManager.AddItem(itemToPurchase);
            _itemsToPurchase[System.Array.IndexOf(_itemsToPurchase, itemToPurchase)] = null;
        }
        else
        {
            UnityEngine.Debug.Log("Not enough spores to purchase item.");
        }
        purchaseWindow.UpdateItems();
    }

    public bool PLayerHasEnoughSporesForItem(int i)
    {
        if (itemsToPurchase[i] is null)
            return false;

        return player.sporeCount >= itemPrices[itemsToPurchase[i]];
    }

    private void SetPrices()
    {
        foreach (var item in _itemsToPurchase)
        {
            int price = Random.Range(Mathf.RoundToInt(minPriceFactor * item.meanPrice), Mathf.RoundToInt(maxPriceFactor * item.meanPrice));
            itemPrices.Add(item, price);
        }

    }


    private void GetItems()
    {
        for(int i = 0; i < shopItemSize; i++)
        {
            _itemsToPurchase[i] = ItemsInGameManager.instance.GetRandomItem();
            //TODO: выдавать хилки с каким-то шансом
        }
    }


    private void FindPlayers()
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
                player.OnInteract -= StartPurchase;
                player = null;
            }
            return;
        }

        if (playerColliders.Count() > 0)
        {
            if (player is not null && playerColliders.Select(c => c.gameObject).Contains(player.gameObject))
                return;

            player = playerColliders.First().GetComponent<PlayerManager>();
            player.OnInteract += StartPurchase;
        }
    }

}
