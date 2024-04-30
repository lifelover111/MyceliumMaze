using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;

    private CharacterManager weaponOwner;
    
    private void Awake()
    {
        weaponOwner = GetComponentInParent<CharacterManager>();
    }

    public void Attack()
    {
        var go = Instantiate(projectilePrefab);
        go.transform.position = projectileSpawnPoint.position;
        var projectile = go.GetComponent<Projectile>();
        projectile.target = weaponOwner.combatManager.currentTarget;
        projectile.weaponOwner = weaponOwner;
    }

}
