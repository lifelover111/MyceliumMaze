using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    protected CharacterManager character;

    public CharacterManager currentTarget;

    private DamageCollider weaponDamageCollider;

    public bool canCombo = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        weaponDamageCollider = character.weapon.GetComponent<DamageCollider>();
    }

    protected virtual void Start()
    {
        DisableWeaponCollider();
    }

    protected virtual void Update()
    {

    }

    public virtual void TryAttack()
    {
        if (character.isPerformingAction)
        {
            if(canCombo)
            {
                character.animatorManager.PlayComboAnimation("Attack", true, true);
                character.canMove = false;
                character.canRotate = true;
            }
            return;
        }
        character.animatorManager.PlayTargetAttackAnimation(character.animationKeys.Attack, true, true);
        character.canMove = false;
        character.canRotate = true;
    }

    public virtual void EnableWeaponCollider()
    {
        weaponDamageCollider.Enable();
    }

    public virtual void DisableWeaponCollider()
    {
        weaponDamageCollider.Disable();
    }
}
