using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [System.Serializable]
    public record AnimationKeys
    {
        public string Attack = "Attack";
        public string Dead = "Dead";
        public string HitForward = "HitFromForward";
        public string HitBack = "HitFromBack";
        public string StunForward = "StunForward";
        public string StunBack = "StunBack";
        public string StunLeft = "StunLeft";
        public string StunRight = "StunRight";
        public string Block = "BlockUp";
        public string BlockHit = "BlockHit";
        public string BreakBlock = "BreakBlock";
        public string ParriedToStun = "ParriedStun";
        public string Parry = "Parry";

        [Header("Player")]
        public string Heal = "Heal";
        public string Dash = "Dash";
        public string Cast = "CastSpell";
    }

    protected CharacterManager character;

    [Header("Attack cancelation flags")]
    protected bool _enableColliderCanceled = false;
    protected bool _disableCanRotateCanceled = false;
    protected bool _enableWeaponSlashCanceled = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void ResetAnimationFlags()
    {
        _enableColliderCanceled = false;
        _disableCanRotateCanceled = false;
        _enableWeaponSlashCanceled = false;
    }

    public void CancelAttack()
    {
        CancelAttackEvents();
    }

    private void CancelAttackEvents()
    {
        _enableColliderCanceled = true;
        _disableCanRotateCanceled = true;
        _enableWeaponSlashCanceled = true;
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool applyRootMotion = true, float dampTime = 0.1f)
    {
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.SetFloat("x", horizontalValue, dampTime ,Time.deltaTime);
        character.animator.SetFloat("y", verticalValue, dampTime, Time.deltaTime);
    }
    public void SetAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        character.animator.SetFloat("x", horizontalValue);
        character.animator.SetFloat("y", verticalValue);
    }

    public void UpdateAnimatorRotationParameters(float turnValue, float dampTime = 0.1f)
    {
        character.animator.SetFloat("turn", turnValue, dampTime, Time.deltaTime);
    }

    public void SetAnimatorRotationParameters(float turnValue)
    {
        character.animator.SetFloat("turn", turnValue);
    }

    public void UpdateAnimatorBlockParameters(bool block)
    {
        character.animator.SetBool("Block", block);
    }

    public void PlayTargetActionAnimation(string targetAnimation, bool isPerformongAction, bool applyRootMotion = true)
    {
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformongAction;
    }

    public void PlayTargetHitAnimation(string targetAnimation, bool isPerformongAction, bool applyRootMotion = true)
    {
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.05f);
        character.isPerformingAction = isPerformongAction;
    }

    public void PlayTargetAttackAnimation(string targetAttackAnimation, bool isPerformongAction, bool applyRootMotion = true)
    {
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAttackAnimation, 0.2f);
        character.isPerformingAction = isPerformongAction;
    }

    public void PlayComboAnimation(string key, bool isPerformongAction, bool applyRootMotion = true)
    {
        DisableCanDoCombo();
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.SetTrigger(key);
        character.isPerformingAction = isPerformongAction;
    }



    #region Animation Events
    public virtual void EnableCanDoCombo()
    {
        character.combatManager.canCombo = true;
    }

    public virtual void DisableCanDoCombo()
    {
        character.combatManager.canCombo = false;
    }

    public virtual void EnableAttackCollider()
    {
        if(_enableColliderCanceled)
        {
            _enableColliderCanceled = false;
            return;
        }

        character.combatManager.EnableWeaponCollider();
    }

    public virtual void DisableAttackCollider()
    {
        character.combatManager.DisableWeaponCollider();
    }

    public virtual void EnableWeaponSlash()
    {
        if(_enableWeaponSlashCanceled)
        {
            _enableWeaponSlashCanceled = false;
            return;
        }
        character.weapon.transform.GetChild(0).gameObject.SetActive(true);
    }

    public virtual void DisableWeaponSlash()
    {
        if (character.weapon.transform.childCount == 0)
            return;
        character.weapon.transform.GetChild(0).gameObject.SetActive(false);
    }

    public virtual void EnableCanRotate()
    {
        character.canRotate = true;
    }

    public virtual void DisableCanRotate()
    {
        if (_disableCanRotateCanceled)
        {
            _disableCanRotateCanceled = false;
            return;
        }
        
        character.canRotate = false;
    }
    public virtual void EnableIsBlocking()
    {
        character.combatManager.EnableIsBlocking();
    }

    public virtual void DisableIsBlocking()
    {
        character.combatManager.DisableIsBlocking();
    }

    public virtual void EnableCanParry()
    {
        character.combatManager.EnableCanParry();
    }

    public virtual void DisableCanParry()
    {
        character.combatManager.DisableCanParry();
    }

    public virtual void EnableInvulnerability()
    {
        character.combatManager.EnableInvulnerability();
    }

    public virtual void DisableInvulnerability()
    {
        character.combatManager.DisableInvulnerability();
    }

    public virtual void DoRangeAttack()
    {
        character.combatManager.DoRangeAttack();
    }

    #endregion
}
