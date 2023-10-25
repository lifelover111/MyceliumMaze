using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Item", fileName = "new Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        passive,
        active
    }
    [SerializeField] public string itemName;
    [SerializeField] public ItemType type;
    [SerializeField] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] UnityEvent onPickUp; //� ������ ������� ��� ��������� ��������� - ��������� ����� ������
    [SerializeField] UnityEvent onUse; // ��, ��� ���������� ��� ������������� ��������� ��������
    [SerializeField] UnityEvent onRemove; // �� �� �����, ��� onPickUp, ������ ��������, ����� ������ ������ ���������� ��������, ���� �� ����� �������

    public void PickUp()
    {
        onPickUp.Invoke();
    }
    public void Remove()
    {
        onRemove.Invoke();
    }
    public void Use()
    {
        onUse.Invoke();
    }

    public Item Copy()
    {
        Item copy = new Item();
        copy.itemName = this.itemName;
        copy.type = this.type;
        copy.description = this.description;
        copy.icon = this.icon;
        return copy;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Item objAsItem = obj as Item;
        if (objAsItem == null) return false;
        else
        {
            return this.itemName == objAsItem.itemName;
        }
    }

    public override int GetHashCode()
    {
        return this.itemName.GetHashCode();
    }
}