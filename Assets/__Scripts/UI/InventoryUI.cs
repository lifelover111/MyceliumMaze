using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TMP_Text selectedItemTitle;
    [SerializeField] private TMP_Text selectedItemDescription;
    [SerializeField] private Transform activeItemsAnchor;
    [SerializeField] private Transform passiveItemsAnchor;
    [SerializeField] private GameObject uiItemPrefab;

    public event System.Action OnEnabled = delegate { };

    private Item SelectedItem { 
        set {
            selectedItemIcon.sprite = value.icon;
            selectedItemTitle.text = value.itemName;
            selectedItemDescription.text = value.description;
        }
    }

    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }

    public void UpdateItems(ItemManager itemManager)
    {
        for (int i = 0; i < activeItemsAnchor.childCount; i++)
        {
            Destroy(activeItemsAnchor.GetChild(i).gameObject);
        }

        for (int i = 0; i < passiveItemsAnchor.childCount; i++)
        {
            Destroy(passiveItemsAnchor.GetChild(i).gameObject);
        }

        foreach (var item in itemManager.itemsInInventory)
        {
            var uiItem = Instantiate(uiItemPrefab);
            uiItem.GetComponent<Image>().sprite = item.icon;
            uiItem.GetComponent<Button>().onClick.AddListener(() => { SelectedItem = item; });
            uiItem.transform.SetParent(passiveItemsAnchor, false);
        }

        if (itemManager.activeItem != null)
        {
            var activeUiItem = Instantiate(uiItemPrefab);
            activeUiItem.GetComponent<Image>().sprite = itemManager.activeItem.icon;
            activeUiItem.GetComponent<Button>().onClick.AddListener(() => { SelectedItem = itemManager.activeItem; });
            activeUiItem.transform.SetParent(activeItemsAnchor, false);
            SelectedItem = itemManager.activeItem;
        }
        else if(itemManager.itemsInInventory.Count > 0)
        {
            SelectedItem = itemManager.itemsInInventory.First();
        }
        else
        {
            selectedItemTitle.text = "Нет предметов";
            selectedItemIcon.gameObject.SetActive(false);
            selectedItemDescription.text = string.Empty;
            return;
        }
        selectedItemIcon.gameObject.SetActive(true);
    }
}
