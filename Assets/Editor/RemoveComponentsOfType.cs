using UnityEngine;
using UnityEditor;
using System;

public class RemoveComponentsOfType : EditorWindow
{
    private GameObject selectedObject;

    [MenuItem("Tools/Remove Components of Type")]
    static void Init()
    {
        RemoveComponentsOfType window = (RemoveComponentsOfType)EditorWindow.GetWindow(typeof(RemoveComponentsOfType));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Remove Components of Type", EditorStyles.boldLabel);

        selectedObject = EditorGUILayout.ObjectField("Selected Object", selectedObject, typeof(GameObject), true) as GameObject;
        //componentTypeName = EditorGUILayout.TextField("Component Type Name", componentTypeName);

        if (GUILayout.Button("Remove Components"))
        {
            if (selectedObject != null)
            {
                Type componentType = typeof(FadingObject);
                if (componentType != null)
                {
                    Component[] components = selectedObject.GetComponentsInChildren(componentType);
                    int i = 0;
                    foreach (Component component in components)
                    {
                        Undo.DestroyObjectImmediate(component);
                        i++;
                    }
                    Debug.Log("Удалено " + i + " компонентов");
                }
                else
                {
                    Debug.LogWarning("Component type not found.");
                }
            }
            else
            {
                Debug.LogWarning("Please select a GameObject.");
            }
        }
    }
}