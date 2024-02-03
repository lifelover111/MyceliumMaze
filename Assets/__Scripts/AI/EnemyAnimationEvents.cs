using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    void DecreaseAggressivivity()
    {
        if (Random.value > 0.5)
            enemy.agressivity *= 0.5f;
    }
    void EnableWeaponHitbox()
    {
        enemy.weapon.enabled = true;
    }

    void DisableWeaponHitbox()
    {
        enemy.weapon.enabled = false;
    }
}
