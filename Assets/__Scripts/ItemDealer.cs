using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Media;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemDealer : MonoBehaviour
{
    [Header("Properties")]
    public float minPriceFactor = 0.85f;
    public float maxPriceFactor = 1.15f;
    public float interactionDistance = 3f;
    private PlayerInputManager playerInputManager;

    public Item[] itemsToPurchase = new Item[6];

    public Dictionary<Item, int> itemPrices = new Dictionary<Item, int>();

    private PlayerManager player;
    // public PurchaseUI purchaseUI;


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
        //if (playerInputManager != null)
        //{
        //    UnityEngine.Debug.Log("911");
        //    playerInputManager.enabled = false;

        //}
        //Time.timeScale = 0f;

    }

    public void PurchaseItem(int itemIndex)
    {
        if (player == null)
            return;

        Item itemToPurchase = itemsToPurchase[itemIndex];

        if (itemToPurchase is null)
            return;

        var purchaseWindow = player.GetPurchaseUI();
        int price = itemPrices[itemToPurchase];
        if (player.TryTakeSpores(price))
        {
            player.itemManager.AddItem(itemToPurchase);
            itemsToPurchase[System.Array.IndexOf(itemsToPurchase, itemToPurchase)] = null;
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
        foreach (var item in itemsToPurchase)
        {
            int price = Random.Range(Mathf.RoundToInt(minPriceFactor * item.meanPrice), Mathf.RoundToInt(maxPriceFactor * item.meanPrice));
            itemPrices.Add(item, price);
        }

    }


    private void GetItems()
    {
        for (int i = 0; i < itemsToPurchase.Length; i++)
        {
            itemsToPurchase[i] = ItemsInGameManager.instance.GetRandomItem();
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
