using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float cutoutSize;
    [SerializeField] private float sphereColliderRadius;
    [SerializeField] private float falloffSize;
    [SerializeField] private float playerHeight;
    private List<Collider> hitColliders = new List<Collider>();
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        targetObject = PlayersInGameManager.instance.playerList[0].transform;
    }

    private void Update()
    {
        Vector3 targetPointPos = targetObject.position;
        targetPointPos.y += playerHeight;

        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetPointPos);
        cutoutPos.y /= Screen.width / Screen.height;

        foreach (Collider collider in hitColliders)
        {
            Renderer[] renderers = collider.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                foreach (Material material in materials)
                {
                    material.SetFloat("_CutoutSize", 0.0f);
                    material.SetFloat("_FalloffSize", 0.0f);
                }
            }
        }
        hitColliders.Clear();

        Collider[] overlappedColliders = Physics.OverlapSphere(targetPointPos, sphereColliderRadius, wallMask);

        foreach (Collider collider in overlappedColliders)
        {
            Renderer[] renderers = collider.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                foreach (Material material in materials)
                {
                    material.SetVector("_CutoutPos", cutoutPos);
                    material.SetFloat("_CutoutSize", cutoutSize);
                    material.SetFloat("_FalloffSize", falloffSize);
                }
            }
        }
        hitColliders.AddRange(overlappedColliders.Where(c => !hitColliders.Contains(c)));
    }
}