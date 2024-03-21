using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsInGameManager : MonoBehaviour
{
    [SerializeField] List<Item> items;

    private void Awake()
    {
        SetIdToAllItems();
    }

    private void SetIdToAllItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].id = i;
        }
    }
}
