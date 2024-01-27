using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions Library", fileName = "ItemActions")]
public class ItemActions : ScriptableObject
{
    public Player player;
    //����� ����� ����� ������ ���������� ������ ���� ���������, ����� ��� ������� ���������� Item ������ ������ ����� ������������ � onPickUp, onUse � onRemove
}
