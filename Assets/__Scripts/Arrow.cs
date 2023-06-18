using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    private void OnTriggerEnter(Collider other)
    {
        if(enemyLayer.value != (1 << other.gameObject.layer))
            Destroy(gameObject);
    }
    
}
