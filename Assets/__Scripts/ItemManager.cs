using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] ItemActions itemActions;
    Hero hero;
    Inventory inventory = new Inventory();
    private void Awake()
    {
        hero = GetComponent<Hero>();
        itemActions.hero = hero;
    }
}
