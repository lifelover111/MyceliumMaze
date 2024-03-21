using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : ScriptableObject
{
    public enum ItemType
    {
        passive,
        active
    }
    [HideInInspector] public int id;

    [Header("Item properties")]
    [SerializeField] public string itemName;
    [SerializeField] public ItemType type;
    [SerializeField] public string description;
    [SerializeField] public Sprite icon;

    public virtual void PickUp(PlayerManager player)
    {
    }
    public virtual void Remove(PlayerManager player)
    {
    }
    public virtual void Use(PlayerManager player)
    {
    }
}