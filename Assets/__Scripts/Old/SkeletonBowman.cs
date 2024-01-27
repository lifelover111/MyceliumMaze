/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldProject
{
    public class SkeletonBowman : OldProject.Enemy
    {
        [SerializeField] LayerMask shotLayerMask;
        Bow bow;
        Vector3 perspectiveError = 0.55f * Vector3.up;

        AudioClip shotClip;

        protected override void Awake()
        {
            name = "SkeletonBowman";
            healthCoeff = 8;
            concCoeff = 2;

            stats = new Stats(1, 1, 3, 3);

            soulsPerLevel = 45;

            base.Awake();


            atkHitBoxes.Add(transform.GetChild(0).gameObject);
            atkHitBoxes.Add(transform.GetChild(1).gameObject);
            foreach (var box in atkHitBoxes)
                box.SetActive(false);

            atkHitBoxes[0].GetComponent<DamageEffect>().knockback = true;
            atkHitBoxes[1].GetComponent<DamageEffect>().knockback = false;


            bow = atkHitBoxes[0].GetComponent<Bow>();

            float atkDistance = 7;

            behaviors.Add(new EnemyBehaviors.AttackBehaviors.ShotBehavior(this, "SkeletonBowman_Attack_0", atkHitBoxes[0], atkDistance, 0.8f, 0, 1.2f, shotLayerMask));
            behaviors.Add(new EnemyBehaviors.AttackBehaviors.CommonAttackBehavior(this, "SkeletonBowman_Attack_1", atkHitBoxes[1], 0.8f, 0.4f, 3));
            behaviors.Add(new EnemyBehaviors.MoveBehaviors.KeepDistanceBehavior(this, "SkeletonBowman_Move", atkDistance));

            behaviors.Add(new EnemyBehaviors.MoveBehaviors.SurroundBehavior(this, "SkeletonBowman_Move"));

            hitClip = SoundBank.instance?.Hit_SkeletonBow;
            damageClip = SoundBank.instance?.Damage_Skeleton;
            dieClip = SoundBank.instance?.Die_Skeleton;
            blockClip = SoundBank.instance?.Block;
            parryClip = SoundBank.instance?.Parry;
            stepClip = SoundBank.instance?.Footstep;
            shotClip = SoundBank.instance?.Shot_Bow;
        }

        protected override void Update()
        {
            base.Update();
        }

        void StartMeleeHit()
        {
            hit = true;
            currentAtkHitbox.transform.localRotation = Quaternion.Euler(0, 0, 45 * facing);
            currentAtkHitbox.SetActive(true);

            agressivity *= 0.1f;

        }
        void StopMeleeHit()
        {
            canChooseBehavior = true;
            currentAtkHitbox.SetActive(false);
            isAttacking = false;
        }

        void StartShotAnim()
        {
            anim.speed = 1;
        }

        void StartToAim()
        {
            anim.speed = 0.25f;
        }

        void MakeShot()
        {
            audioSource.clip = shotClip;
            audioSource.Play();

            if (Random.value > 0.5f)
                agressivity *= 0.8f;
            anim.speed = 1;
            bow.direction = (target.transform.position + perspectiveError) - bow.gameObject.transform.position;
            currentAtkHitbox.SetActive(true);
        }
        void StopShotAnim()
        {
            canChooseBehavior = true;
            currentAtkHitbox.SetActive(false);
            isAttacking = false;
        }


    }
}
*/