using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    protected CharacterManager character;

    public CharacterManager currentTarget;

    private DamageCollider weaponDamageCollider;

    public bool canCombo = false;
    public bool canParry = false;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        weaponDamageCollider = character.weapon.GetComponent<DamageCollider>();
    }

    protected virtual void Start()
    {
        DisableWeaponCollider();
        character.OnDead += DisableWeaponCollider;
    }

    protected virtual void Update()
    {
        HandleBlock();
    }

    public virtual void TryAttack()
    {
        if (character.isPerformingAction)
        {
            if(canCombo)
            {
                character.animatorManager.PlayComboAnimation("Attack", true, true);
                character.isPerformingAction = true;
                character.canMove = false;
                character.canRotate = true;
            }
            return;
        }
        character.animatorManager.PlayTargetAttackAnimation(character.animationKeys.Attack, true, true);
        character.canMove = false;
        character.canRotate = true;
    }

    public void TryBlock()
    {
        if (!character.canBlockAttacks)
            return;

        character.animatorManager.UpdateAnimatorBlockParameters(PlayerInputManager.instance.blockInput);
    }

    protected virtual void HandleBlock()
    {
        ///TODO: переопределить
        ///
        //if (character.isPerformingAction) return;

        //if (PlayerInputManager.instance.blockInput)
        //{
        //    character.isPerformingAction = true;
        //    character.canMove = false;
        //    character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Block, true);
        //}
    }

    public virtual void EnableWeaponCollider()
    {
        weaponDamageCollider.Enable();
    }

    public virtual void DisableWeaponCollider()
    {
        weaponDamageCollider.Disable();
    }

    public virtual void EnableIsBlocking()
    {
        character.isBlocking = true;
    }

    public virtual void DisableIsBlocking()
    {
        character.isBlocking = false;
    }

    public virtual void EnableCanParry()
    {
        canParry = true;
    }

    public virtual void DisableCanParry() 
    {
        canParry = false;
    }

}
