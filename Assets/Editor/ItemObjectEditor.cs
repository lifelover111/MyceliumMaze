using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item), true)]
public class ItemObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        if (GUILayout.Button("Reset", GUILayout.Height(40)))
        {
            item.Reset();
        }
        base.OnInspectorGUI();
    }
}