using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireCamera : MonoBehaviour
{
    public static BonfireCamera instance;
    [SerializeField] BonfireMenu menu;
    private void Awake()
    {
        instance = this;
        Cursor.SetCursor(GameManager.cursor, Vector2.zero, CursorMode.Auto);
    }

    public void ShowMenu()
    {
        Cursor.visible = true;
        menu.gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        Cursor.visible = false;
        menu.gameObject.SetActive(false);
    }
}
