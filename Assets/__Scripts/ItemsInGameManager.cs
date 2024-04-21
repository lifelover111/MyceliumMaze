using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsInGameManager : MonoBehaviour
{
    [SerializeField] List<Item> items;

    public static ItemsInGameManager instance;

    private List<Item> _itemsGiven;

    private void Awake()
    {
        if (instance is not null)
            return;

        instance = this;

        PlayersInGameManager.instance.playerList.First().OnDead += () => instance = null;

        _itemsGiven = new List<Item>();

        SetIdToAllItems();
        DuplicateItems();
    }

    public Item GetRandomItem()
    {
        var possibleItems = items.Where(i => !_itemsGiven.Contains(i));
        
        if (possibleItems.Count() == 0)
            return null;

        var item = possibleItems.ToArray()[Random.Range(0, possibleItems.Count())];
        _itemsGiven.Add(item);
        return item;
    }

    private void SetIdToAllItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].id = i;
        }
    }

    private void DuplicateItems()
    {
        for(int i = 0;i < items.Count;i++) 
        {
            items[i] = Instantiate(items[i]);
        }
    }

}
