using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Reflection;

public class PurchaseUI : MonoBehaviour
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDescription;

    [SerializeField] TMP_Text[] prices;

    public void SetItemPrice(int index, int price)
    {
        prices[index].text = price.ToString();
    }
    public Image[] itemImages;
    public void SetItemImage(int index, Sprite sprite)
    {
        itemImages[index].sprite = sprite;
    }
    public Button[] buyButtons;
    private ItemDealer itemDealer;

    public void SetItemDealer(ItemDealer itemDealer)
    {
        this.itemDealer = itemDealer;
    }

    public void SelectItem(int index)
    {
        var selectedItem = itemDealer.itemsToPurchase[index];
        
        if (selectedItem is null)
            return;

        itemName.text = selectedItem.itemName;
        itemDescription.text = selectedItem.description;

    }

    public void PurchaseItem(int index)
    {
        if (itemDealer is null)
            return;

        itemDealer.PurchaseItem(index);

        prices[index].transform.parent.gameObject.SetActive(false);
        buyButtons[index].gameObject.SetActive(false);
        itemImages[index].color = Color.black;
        itemName.text = "Выберите предмет";
        itemDescription.text = string.Empty;

    }

    public void CloseWindow()
    {
        itemDealer.EndPurchase();
    }

    public void UpdateItems()
    {
        for (int i = 0; i < itemDealer.itemsToPurchase.Length; i++)
        {
            buyButtons[i].interactable = itemDealer.itemsToPurchase[i] is not null && itemDealer.PLayerHasEnoughSporesForItem(i);
            if (itemDealer.itemsToPurchase[i] is null)
            {
                //Уже проданный предмет (или не инициализированный)
                SetItemPrice(i, 0);
                SetItemImage(i, null);
                continue;
            }

            SetItemPrice(i, itemDealer.itemPrices[itemDealer.itemsToPurchase[i]]);
            SetItemImage(i, itemDealer.itemsToPurchase[i].icon);
        }
    }

    private void OnEnable()
    {
        PlayerInputManager.instance.uiStack.Add(transform);
    }

    private void OnDisable()
    {
        PlayerInputManager.instance.uiStack.Remove(transform);
    }

}
