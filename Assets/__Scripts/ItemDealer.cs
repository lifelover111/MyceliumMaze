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

    public Item[] itemsToPurchase = new Item[3];
    private Dictionary<Item, int> itemPrices = new Dictionary<Item, int>();
    
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

        //if (playerInputManager != null)
        //{
        //    UnityEngine.Debug.Log("911");
        //    playerInputManager.enabled = false;
            
        //}
        //Time.timeScale = 0f;
        for (int i = 0; i < itemsToPurchase.Length; i++)
        {
            purchaseWindow.SetItemPrice(i, itemPrices[itemsToPurchase[i]]);
            UnityEngine.Debug.Log(itemPrices[itemsToPurchase[i]]);
            purchaseWindow.SetItemImage(i, itemsToPurchase[i].icon);
            UnityEngine.Debug.Log(itemsToPurchase[i]);
        }

    }
    public void PurchaseItem(int itemIndex)
    {
        var purchaseWindow = player.GetPurchaseUI();
        if (player == null)
            return;
        UnityEngine.Debug.Log("GBPLF");
        Item itemToPurchase = itemsToPurchase[itemIndex];
        int price = itemPrices[itemToPurchase];
        if (player.sporeCount >= price)
        {
            player.sporeCount -= price;

            player.itemManager.AddItem(itemToPurchase);
            purchaseWindow.gameObject.SetActive(false);
            //if (playerInputManager != null)
            //{
            //    playerInputManager.enabled = true;
            //}
        }
        else
        {
            UnityEngine.Debug.Log("Not enough spores to purchase item.");
        }
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
        for(int i = 0; i < itemsToPurchase.Length; i++)
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
