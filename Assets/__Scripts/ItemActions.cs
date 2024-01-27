using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions Library", fileName = "ItemActions")]
public class ItemActions : ScriptableObject
{
    public Player player;
    //здесь нужно будет писать конкретные методы всех предметов, затем дл€ каждого экземпл€ра Item нужные методы будут выставл€тьс€ в onPickUp, onUse и onRemove
}
