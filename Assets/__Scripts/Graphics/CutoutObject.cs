using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    [SerializeField]
    private float cutoutSize;

    [SerializeField]
    private float falloffSize;

    [SerializeField]
    private float playerHeight;

    private RaycastHit[] hitObjectsBefore;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        hitObjectsBefore = new RaycastHit[0];
    }

    private void Update()
    {
        Vector3 targetObjectPos = targetObject.position;
        targetObjectPos.y += playerHeight;
        
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObjectPos);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObjectPos - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);


        if (hitObjects.Length == 0 && hitObjectsBefore.Length > 0) 
        {
            for (int i = 0; i < hitObjectsBefore.Length; ++i)
            {
                Material[] materials = hitObjectsBefore[i].transform.GetComponent<Renderer>().materials;
 
                for(int m = 0; m < materials.Length; ++m)
                {
                    materials[m].SetFloat("_CutoutSize", 0.0f);
                    materials[m].SetFloat("_FalloffSize", 0.0f);
                }
            }
            hitObjectsBefore = new RaycastHit[0];
        }

        for (int i = 0; i < hitObjects.Length; ++i)
        {           
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;
            Debug.Log(hitObjects[i].transform.position);
            Debug.Log(hitObjects[i].transform.gameObject);
            for(int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetVector("_CutoutPos", cutoutPos);
                materials[m].SetFloat("_CutoutSize", cutoutSize);
                materials[m].SetFloat("_FalloffSize", falloffSize);
            }
            
        }

        hitObjectsBefore = hitObjects;
    }
}