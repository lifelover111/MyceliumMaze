using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] ItemActions itemActions;
    Player player;
    Inventory inventory = new Inventory();
    private void Awake()
    {
        player = GetComponent<Player>();
        itemActions.player = player;
    }
}
