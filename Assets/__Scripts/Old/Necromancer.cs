using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace OldProject
{
    public class Necromancer : OldProject.Enemy
    {
        [SerializeField] LayerMask shotLayerMask;
        [SerializeField] GameObject projectile;
        DamageEffect projectileDmgEf;
        AudioClip shotClip;
        protected override void Awake()
        {
            name = "Necromancer";
            healthCoeff = 10;
            concCoeff = 1;

            stats = new Stats(4, 1, 1, 2);

            soulsPerLevel = 35;

            base.Awake();


            atkHitBoxes.Add(transform.GetChild(0).gameObject);
            foreach (var box in atkHitBoxes)
                box.SetActive(false);

            projectileDmgEf = atkHitBoxes[0].GetComponent<DamageEffect>();
            projectileDmgEf.knockback = false;

            float atkDistance = 3.5f;

            behaviors.Add(new EnemyBehaviors.AttackBehaviors.ShotBehavior(this, "Necromancer_Attack", atkHitBoxes[0], atkDistance, 0, 0.5f, 0, shotLayerMask));
            behaviors.Add(new EnemyBehaviors.MoveBehaviors.KeepDistanceBehavior(this, "Necromancer_Move", atkDistance, true));

            behaviors.Add(new EnemyBehaviors.MoveBehaviors.SurroundBehavior(this, "Necromancer_Move"));

            hitClip = SoundBank.instance?.Hit_Skeleton;
            damageClip = SoundBank.instance?.Damage_Necromancer;
            dieClip = SoundBank.instance?.Die_Necromancer;
            blockClip = SoundBank.instance?.Block;
            parryClip = SoundBank.instance?.Parry;
            stepClip = SoundBank.instance?.Footstep;
            shotClip = SoundBank.instance?.Shot_Necromancer;
        }

        protected override void Update()
        {
            base.Update();
        }


        void MakeShot()
        {
            currentAtkHitbox.SetActive(true);
            currentAtkHitbox.transform.localPosition = Quaternion.Euler(0, 0, 45 * facing) * Vector2.right;
            GameObject go = Instantiate(projectile);
            go.GetComponent<DamageEffect>().sanityDamage = projectileDmgEf.sanityDamage;
            go.transform.position = currentAtkHitbox.transform.position + 0.3f * Vector3.up;
            go.GetComponent<Rigidbody>().velocity = (target.transform.position - transform.position).normalized;
            agressivity *= 0.7f;

            audioSource.clip = shotClip;
            audioSource.Play();
        }

        void StopShotAnim()
        {
            currentAtkHitbox.SetActive(false);
            isAttacking = false;
            canChooseBehavior = true;
        }

    }
}
*/