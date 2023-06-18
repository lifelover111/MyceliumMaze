using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDirectionController : MonoBehaviour
{
    Hero hero;

    private Vector2 lastMousePosition;

    Vector2 _viewDirection;
    public Vector2 viewDirection { get { return _viewDirection; } private set { _viewDirection = value; } }

    void Start()
    {
        hero = GetComponent<Hero>();
        lastMousePosition = Vector2.up;
        viewDirection = Vector2.up;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!hero.canRotate)
            return;

        Vector2 mouseDelta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastMousePosition;
        Quaternion rot = Quaternion.Slerp(Quaternion.identity , Quaternion.FromToRotation(viewDirection, mouseDelta), 4*Time.deltaTime);
        if(viewDirection.normalized != mouseDelta.normalized)
            viewDirection = rot * viewDirection;

        lastMousePosition = Input.mousePosition;
    }
}
