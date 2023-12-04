using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TargetDetector : Detector
{
    [SerializeField]
    private float targetDetectionRange = 15;

    [SerializeField]
    private LayerMask obstaclesLayerMask, playerLayerMask;

    [SerializeField]
    private bool showGizmos = false;

    private List<Transform> colliders;

    public override void Detect(AIData aiData)
    {
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, targetDetectionRange, playerLayerMask);
        
        Collider playerCollider = playerColliders.Length == 0 ? null : playerColliders[0];
        if (playerCollider != null)
        {
            Vector3 direction = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, targetDetectionRange, obstaclesLayerMask | playerLayerMask);
            
            if (hits.Length > 0)
            {
                RaycastHit hit = hits.First(x => x.distance == hits.Min(x => x.distance));
                if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.magenta);
                    colliders = new List<Transform>() { playerCollider.transform };
                }
                else
                {
                    colliders = null;
                }
            }
            else
                colliders = null;
        }
        else
        {
            colliders = null;
        }
        aiData.targets = colliders;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (colliders == null)
            return;
        Gizmos.color = Color.magenta;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}
