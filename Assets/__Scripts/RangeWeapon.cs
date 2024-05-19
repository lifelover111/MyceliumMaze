using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected GameObject projectilePrefab;

    protected CharacterManager weaponOwner;
    
    protected virtual void Awake()
    {
        weaponOwner = GetComponentInParent<CharacterManager>();
    }

    public virtual void Attack()
    {
        var go = Instantiate(projectilePrefab);
        go.transform.position = projectileSpawnPoint.position;
        var projectile = go.GetComponent<Projectile>();
        projectile.target = weaponOwner.combatManager.currentTarget;
        projectile.weaponOwner = weaponOwner;
    }

}
