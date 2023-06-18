using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonousFog : MonoBehaviour
{
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
