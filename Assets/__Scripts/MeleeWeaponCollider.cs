using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponCollider : DamageCollider
{
    private CharacterManager weaponOwner;

    protected override void Awake()
    {
        base.Awake();
        weaponOwner = GetComponentInParent<CharacterManager>();
    }
    protected override void DamageTarget(CharacterManager target, bool withConcentrationDamage = true)
    {
        if (target == weaponOwner) return;
        base.DamageTarget(target, withConcentrationDamage);
    }

}
