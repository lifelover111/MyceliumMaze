using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSwordsman : Enemy
{
    protected override void Awake()
    {
        name = "SkeletonSwordsman";
        healthCoeff = 10;
        concCoeff = 2.5f;

        stats = new Stats(2, 4, 2, 2);

        soulsPerLevel = 15;

        base.Awake();


        atkHitBoxes.Add(transform.GetChild(0).gameObject);
        atkHitBoxes.Add(transform.GetChild(1).gameObject);
        foreach (var box in atkHitBoxes)
            box.SetActive(false);

        atkHitBoxes[0].GetComponent<DamageEffect>().knockback = true;
        atkHitBoxes[1].GetComponent<DamageEffect>().knockback = false;


        behaviors.Add(new EnemyBehaviors.AttackBehaviors.CommonAttackBehavior(this, "SkeletonSwordsman_Attack_0", atkHitBoxes[0], 0.5f, 1, 1.5f));
        behaviors.Add(new EnemyBehaviors.AttackBehaviors.SerialAttackBehavior(this, "SkeletonSwordsman_Attack_1", atkHitBoxes[1], 0.8f, 0.3f, 3));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.FollowBehavior(this, "SkeletonSwordsman_Move"));
        behaviors.Add(new EnemyBehaviors.DefendBehaviors.BlockBehavior(this, "SkeletonSwordsman_Block"));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.AvoidBehavior(this, "SkeletonSwordsman_Move"));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.SurroundBehavior(this, "SkeletonSwordsman_Move"));

        hitClip = SoundBank.instance?.Hit_Skeleton;
        damageClip = SoundBank.instance?.Damage_Skeleton;
        dieClip = SoundBank.instance?.Die_Skeleton;
        blockClip = SoundBank.instance?.Block;
        parryClip = SoundBank.instance?.Parry;
        stepClip = SoundBank.instance?.Footstep;
    }

    protected override void Update()
    {
        base.Update();
    }



    void StartSerialAttackAnim()
    {
        rigid.velocity = speed*1.5f * (target.transform.position - gameObject.transform.position).normalized;
    }

    void StartSerialHit()
    {
        hit = true;
        facing = CalcFacing(target.transform.position - gameObject.transform.position);
        isAttacking = true;
        currentAtkHitbox.transform.localRotation = Quaternion.Euler(0, 0, 45 * facing);
        currentAtkHitbox.SetActive(true);
    }
    void StopSerialHit()
    {
        rigid.velocity = Vector2.zero;
        isAttacking = false;
        currentAtkHitbox.SetActive(false);
    }

    void JumpBack()
    {
        facing = CalcFacing(target.transform.position - gameObject.transform.position);
        rigid.velocity = -2*new Vector2(Mathf.Cos(facing * Mathf.PI / 4), Mathf.Sin(facing * Mathf.PI / 4));
        agressivity *= 0.25f;
    }

    void EndSerialAttackAnim()
    {
        rigid.velocity = Vector2.zero;
        canChooseBehavior = true;
        isAttacking = false;
    }

}
