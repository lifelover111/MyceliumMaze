using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [System.Serializable]
    public record AnimationKeys
    {
        public string Attack = "Attack";
        public string Dash = "Dash";
        public string Dead = "Dead";
        public string HitForward = "HitFromForward";
        public string HitBack = "HitFromBack";
        public string StunForward = "StunForward";
        public string StunBack = "StunBack";
        public string Block = "BlockUp";
        public string BlockHit = "BlockHit";
        [Header("Player")]
        public string Heal = "Heal";
    }

    protected CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, float dampTime = 0.1f)
    {
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
        character.combatManager.EnableWeaponCollider();
    }

    public virtual void DisableAttackCollider()
    {
        character.combatManager.DisableWeaponCollider();
    }

    public virtual void EnableCanRotate()
    {
        character.canRotate = true;
    }

    public virtual void DisableCanRotate()
    {
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

    #endregion
}
