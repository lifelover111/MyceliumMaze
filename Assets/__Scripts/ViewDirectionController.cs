using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDirectionController : MonoBehaviour
{
    Hero hero;

    private Vector3 lastMousePosition;

    Vector3 _viewDirection;
    public Vector3 viewDirection { get { return _viewDirection; } private set { _viewDirection = value; } }

    void Start()
    {
        hero = GetComponent<Hero>();
        lastMousePosition = Vector3.forward;
        viewDirection = Vector3.forward;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!hero.canRotate)
            return;
        viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection = new Vector3(viewDirection.x, 0, viewDirection.z);


        /*
        Vector3 mouseDelta = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y) - lastMousePosition;
        Quaternion rot = Quaternion.Slerp(Quaternion.identity , Quaternion.FromToRotation(viewDirection, mouseDelta), 4*Time.deltaTime);
        if(viewDirection.normalized != mouseDelta.normalized)
            viewDirection = rot * viewDirection;

        lastMousePosition = Input.mousePosition;
        */
    }
}
