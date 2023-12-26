using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static Hero;

public class Player : MonoBehaviour
{
    [SerializeField] float speedForward;
    [SerializeField] float speedBackward;
    [SerializeField] float speedToSide;

    float speed;
    [SerializeField] Animator anim;
    bool dirHeld = false;
    Vector3 dir;
    public eMode mode = eMode.idle;

    private Rigidbody rigid;
    Vector3 viewDirection;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        viewDirection = Vector3.ProjectOnPlane(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position, Vector3.up).normalized;
        viewDirection = new Vector3(viewDirection.x, 0, viewDirection.z).normalized;

        if(mode != eMode.idle)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.left, viewDirection), 2.2f);
        }


        dirHeld = false;
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        dir = Quaternion.Euler(0, -45, 0) * dir;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dir = transform.rotation * Vector3.left; //viewDirection.normalized;
        }

        if (dir.magnitude > 0) 
        {
            dirHeld = true;
        };
        dir.Normalize();

        if (!dirHeld)
        {
            mode = eMode.idle;
        }
        else
            mode = eMode.move;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), viewDirection);
            anim.SetTrigger("Attack");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), viewDirection);
            anim.SetBool("Block", true);
        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            //transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), viewDirection);
            anim.SetBool("Block", false);
        }


        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.idle:
                anim.SetFloat("x", 0);
                anim.SetFloat("y", 0);

                float p = Mathf.Sin(Mathf.Deg2Rad * Vector3.SignedAngle(transform.rotation * Vector3.left, viewDirection, Vector3.up));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.left, viewDirection), 2.2f);

                anim.SetFloat("turn", p);

                break;
            case eMode.move:
                vel = dir;
                SetSpeed(dir);
                float angle = Vector2.SignedAngle(new Vector2(viewDirection.x, viewDirection.z), Vector2.up);
                Vector3 walkTree = (Quaternion.AngleAxis(angle, Vector3.down) * dir).normalized;
                anim.SetFloat("x", walkTree.x);
                anim.SetFloat("y", walkTree.z);
                break;
        }
        rigid.velocity = vel * speed;
    }

    void SetSpeed(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(viewDirection, direction, Vector3.down);
        if (angle < 0) angle += 360;

        if ((angle >= 0 && angle < 30) || angle > 330)
        {
            speed = speedForward;
        }
        else if (angle > 150 && angle < 210)
        {
            speed = speedBackward;
        }
        else
        {
            speed = speedToSide;
        }
    }
}
