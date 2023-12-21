using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static Hero;

public class Player : MonoBehaviour
{
    public float speed = 2;
    [SerializeField] Animator anim;
    bool dirHeld = false;
    Vector3 dir;
    public eMode mode = eMode.idle;
    const string goForwardKey = "GoForward";
    private ViewDirectionController viewDirectionController;
    private Rigidbody rigid;
    Vector3 viewDirection;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        viewDirectionController = GetComponent<ViewDirectionController>();
    }

    void Update()
    {
        viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection = new Vector3(viewDirection.x, 0, viewDirection.z).normalized;

        if (mode != eMode.attack && mode != eMode.healing && mode != eMode.knockback && mode != eMode.rest)
            transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), viewDirection);

        dirHeld = false;
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        dir = Quaternion.Euler(0, -45, 0) * dir;
        if (dir.magnitude > 0) dirHeld = true;
        dir.Normalize();

        if (!dirHeld)
        {
            mode = eMode.idle;
        }
        else
            mode = eMode.move;


        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.idle:
                //anim.transform.localPosition = Vector3.zero;
                //anim.transform.localRotation = Quaternion.identity;
                anim.CrossFade("Idle", 0);
                anim.speed = 1;
                break;
            case eMode.move:
                //anim.transform.localPosition = Vector3.zero;
                //anim.transform.localRotation = Quaternion.identity;
                vel = dir;
                string a = CalcBodyMoveDirection(dir);
                anim.CrossFade(a, 0);
                anim.speed = 1;
                break;
        }
        rigid.velocity = vel * speed;
    }

    string CalcBodyMoveDirection(Vector3 direction)
    {
        string[] cd = { "MoveForward", "MoveLeft", "MoveBackward", "MoveRight" };
        float angle = Vector3.SignedAngle(viewDirection, direction, Vector3.down);
        if (angle < 0) angle += 360;

        int i = Mathf.RoundToInt((angle) / 90);
        if (i == 4) i = 0;
        return cd[i];
    }
}
