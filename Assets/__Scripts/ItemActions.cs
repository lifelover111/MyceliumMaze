using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions Library", fileName = "ItemActions")]
public class ItemActions : ScriptableObject
{
    [SerializeField] Hero hero;
    //����� ����� ����� ������ ���������� ������ ���� ���������, ����� ��� ������� ���������� Item ������ ������ ����� ������������ � onPickUp, onUse � onRemove
}
