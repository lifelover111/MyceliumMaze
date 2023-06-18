using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBehaviors
{
    namespace AttackBehaviors
    {
        using System.Linq;
        public abstract class AttackBehaviorBase
        {
            protected Enemy enemy;
            protected Rigidbody body;
            protected GameObject target;
            protected float speed;
            protected Animator anim;
            protected string animationName;
            protected GameObject atkHitbox;
            protected float atkRange;
            private float dmgCoefficient;
            private float concDmgCoefficient;

            public AttackBehaviorBase(Enemy enemy, string animName, GameObject atkHitbox, float atkRange, float dmgCoefficient, float concDmgCoefficient)
            {
                this.dmgCoefficient = dmgCoefficient;
                this.concDmgCoefficient = concDmgCoefficient;
                this.enemy = enemy;
                body = enemy.rigid;
                this.anim = enemy.anim;
                animationName = animName;
                this.atkHitbox = atkHitbox;
                this.atkRange = atkRange;
                DamageEffect dmgEf = this.atkHitbox.GetComponent<DamageEffect>();
                dmgEf.damage = dmgCoefficient * enemy.stats.Dexterity;
                dmgEf.concentrationDamage = concDmgCoefficient * enemy.stats.Strength;
            }

            public void InitNewStats()
            {
                this.atkHitbox.GetComponent<DamageEffect>().damage = dmgCoefficient * enemy.stats.Dexterity;
                this.atkHitbox.GetComponent<DamageEffect>().concentrationDamage = concDmgCoefficient * enemy.stats.Strength;
            }
        }

        public class CommonAttackBehavior : AttackBehaviorBase, IEnemyBehavior
        {
            public CommonAttackBehavior(Enemy enemy, string animName, GameObject atkHitbox, float atkRange, float dmgCoefficient, float concDmgCoefficient) :base(enemy, animName, atkHitbox, atkRange, dmgCoefficient, concDmgCoefficient) 
            {
                DamageEffect dmgEf = atkHitbox.GetComponent<DamageEffect>();
                dmgEf.damage = enemy.stats.Dexterity * dmgCoefficient;
                dmgEf.concentrationDamage = enemy.stats.Strength * concDmgCoefficient;
            }
            public void Update()
            {
                enemy.facing = enemy.CalcFacing(target.transform.position - enemy.gameObject.transform.position);

                if (!enemy.isAttacking)
                    return;
                anim.CrossFade(animationName + '_' + enemy.facing, 0);
                anim.speed = 1;
            }

            public float Analyze()
            {
                
                target = enemy.target;
                float result;
                
                result = atkRange * enemy.baseAgressivity * Mathf.Sqrt(enemy.agressivity) / (enemy.gameObject.transform.position - target.transform.position).magnitude;

                return result;
            }

            public void PrepareBehavior()
            {
                enemy.canChooseBehavior = false;
                body.velocity = Vector3.zero;
                enemy.currentAtkHitbox = atkHitbox;
                enemy.atkStepVel = Vector2.zero;
                enemy.isBlockUp = false;
                enemy.isAttacking = true;
            }
        }


        public class SerialAttackBehavior : AttackBehaviorBase, IEnemyBehavior
        {
            public SerialAttackBehavior(Enemy enemy, string animName, GameObject atkHitbox, float atkRange, float dmgCoefficient, float concDmgCoefficient) : base(enemy, animName, atkHitbox, atkRange, dmgCoefficient, concDmgCoefficient)
            {
                DamageEffect dmgEf = atkHitbox.GetComponent<DamageEffect>();
                dmgEf.damage = enemy.stats.Dexterity * dmgCoefficient;
                dmgEf.concentrationDamage = enemy.stats.Strength * concDmgCoefficient;
            }
            public void Update()
            {
                if (!enemy.isAttacking)
                    return;
                anim.CrossFade(animationName + '_' + enemy.facing, 0);
                anim.speed = 1;
            }

            public float Analyze()
            {

                target = enemy.target;
                float result;

                result = Random.value * atkRange * enemy.baseAgressivity * Mathf.Sqrt(enemy.agressivity) / ((enemy.gameObject.transform.position - target.transform.position).magnitude + atkRange);

                return result;
            }

            public void PrepareBehavior()
            {
                enemy.canChooseBehavior = false;
                body.velocity = Vector3.zero;
                enemy.currentAtkHitbox = atkHitbox;
                enemy.atkStepVel = Vector2.zero;
                enemy.isBlockUp = false;
                enemy.isAttacking = true;
            }
        }


        public class ShotBehavior : AttackBehaviorBase, IEnemyBehavior
        {
            LayerMask shotLayerMask;
            int onView = 0;
            public ShotBehavior(Enemy enemy, string animName, GameObject atkHitbox, float atkRange, float dmgCoefficient, float sanityDmgCoefficient, float concDmgCoefficient, LayerMask shotLayerMask) : base(enemy, animName, atkHitbox, atkRange, dmgCoefficient, concDmgCoefficient)
            {
                DamageEffect dmgEf = atkHitbox.GetComponent<DamageEffect>();
                dmgEf.damage = enemy.stats.Dexterity * dmgCoefficient;
                dmgEf.sanityDamage = enemy.stats.Dexterity * sanityDmgCoefficient;
                dmgEf.concentrationDamage = enemy.stats.Strength * concDmgCoefficient;
                this.shotLayerMask = shotLayerMask;
            }
            public void Update()
            {
                enemy.facing = enemy.CalcFacing(target.transform.position - enemy.gameObject.transform.position);
                body.velocity = Vector2.zero;
                if (!enemy.isAttacking)
                    return;
                anim.CrossFade(animationName + '_' + enemy.facing, 0);
            }

            public float Analyze()
            {
                target = enemy.target;
                Vector2 direction = target.transform.position - enemy.gameObject.transform.position;
                RaycastHit[] hits = Physics.RaycastAll(enemy.gameObject.transform.position, direction, atkRange, shotLayerMask);
                
                if (hits.Length == 0)
                    return 0;
                RaycastHit hit = hits.First(x => x.distance == hits.Min(x => x.distance));
                onView = (hit.collider != null && (target.layer == hit.collider.gameObject.layer))? 1 : 0;
                if (onView == 0)
                    return 0;

                float result;

                result = onView*(enemy.baseAgressivity * Mathf.Sqrt(enemy.agressivity) * direction.magnitude/atkRange);

                return result;
            }

            public void PrepareBehavior()
            {
                enemy.canChooseBehavior = false;
                body.velocity = Vector3.zero;
                enemy.currentAtkHitbox = atkHitbox;
                enemy.atkStepVel = Vector2.zero;
                enemy.isBlockUp = false;
                enemy.isAttacking = true;
            }
        }

    }
    namespace MoveBehaviors
    {
        public abstract class MoveBehaviorBase
        {
            protected Enemy enemy;
            protected MovingAI ai;
            protected AIData aiData;
            protected List<SteeringBehaviour> steeringBehaviors;
            protected Rigidbody body;
            protected GameObject target;
            protected float speed;
            protected Animator anim;
            protected string animationName;
            protected float avoidDecisionTime = 5;

            public MoveBehaviorBase(Enemy enemy, string animName)
            {
                this.enemy = enemy;
                body = enemy.rigid;
                speed = enemy.speed;
                this.anim = enemy.anim;
                animationName = animName;
                ai = enemy.gameObject.GetComponent<MovingAI>();
                steeringBehaviors = ai.steeringBehaviours;
                aiData = enemy.gameObject.GetComponent<AIData>();
            }

            public virtual void Update()
            {
                if ((enemy.gameObject.transform.position - enemy.avoidDecisionPosition).magnitude >= body.velocity.magnitude * Time.deltaTime)
                {
                    enemy.lastMoveTime = Time.time;//Mathf.Lerp(enemy.lastMoveTime, Time.time, (enemy.lastMoveTime+Time.deltaTime)/Time.time);
                }
            }

            protected char CalcBodyDirection(Vector3 direction)
            {
                char[] cd = { 'T', 'L', 'B', 'R' };
                Vector3 relDir = new Vector3(Mathf.Cos(enemy.facing * Mathf.PI / 4), Mathf.Sin(enemy.facing * Mathf.PI / 4), 0);
                float angle = Vector3.SignedAngle(relDir, direction, Vector3.forward);
                if (angle < 0) angle += 360;
                int i = (int)Mathf.Floor((angle + 45) / 90);
                if (i == 4) i = 0;
                return cd[i];
            }

            public void InitNewStats(){}
        }

        public class FollowBehavior : MoveBehaviorBase, IEnemyBehavior
        {
            public FollowBehavior(Enemy enemy, string animName) : base(enemy, animName){}
            public override void Update()
            {
                base.Update();

                Vector2 movementDirection = ai.movementDirectionSolver.GetDirectionToMove(steeringBehaviors, aiData);
                
                body.velocity = speed * movementDirection;
                if (body.velocity.magnitude <= 0.01f)
                {
                    anim.CrossFade(animationName.Split('_')[0] + "_Idle_" + enemy.facing, 0);
                    anim.speed = 1;
                }
                else
                {
                    anim.CrossFade(animationName + CalcBodyDirection(movementDirection) + '_' + enemy.facing, 0);
                    anim.speed = 1;
                    enemy.facing = enemy.CalcFacing(movementDirection/*target.transform.position - enemy.gameObject.transform.position*/);

                }
                enemy.agressivity += Time.deltaTime;
            }

            public float Analyze()
            {
                target = enemy.target;
                return 2*(enemy.gameObject.transform.position - target.transform.position).magnitude * Mathf.Sqrt(enemy.agressivity) * Mathf.Clamp((avoidDecisionTime + enemy.lastMoveTime - Time.time)/avoidDecisionTime,0,1);
            }
            public void PrepareBehavior()
            {
                enemy.isBlockUp = false;
                enemy.isAttacking = false;
            }
        }
        public class AvoidBehavior : MoveBehaviorBase, IEnemyBehavior
        {
            public AvoidBehavior(Enemy enemy, string animName) : base(enemy, animName) { }
            public override void Update()
            {
                base.Update();

                enemy.facing = enemy.CalcFacing(target.transform.position - enemy.gameObject.transform.position);

                Vector2 movementDirection = ai.movementDirectionSolver.GetDirectionToMove(steeringBehaviors, aiData, 180);

                body.velocity = 0.5f * speed * movementDirection;
                anim.CrossFade(animationName + CalcBodyDirection(movementDirection) + '_' + enemy.facing, 0);
                anim.speed = 1;

                enemy.agressivity += Time.deltaTime;
            }

            public float Analyze()
            {
                target = enemy.target;
                return (enemy.baseAgressivity * 20/ (enemy.agressivity * (enemy.gameObject.transform.position - target.transform.position).magnitude))*(1 + Mathf.Exp((Time.time - enemy.lastMoveTime)/avoidDecisionTime));
            }
            public void PrepareBehavior()
            {
                enemy.isBlockUp = false;
                enemy.isAttacking = false;
            }
        }
        public class SurroundBehavior : MoveBehaviorBase, IEnemyBehavior
        {
            private float timeDirChanged;
            private float dirChangeDelay = 3f;
            int dirCoeff;
            public SurroundBehavior(Enemy enemy, string animName) : base(enemy, animName) { }
            public override void Update()
            {
                base.Update();

                if(Time.time > timeDirChanged + dirChangeDelay)
                {
                    timeDirChanged = Time.time;
                    dirCoeff = Random.value > 0.5f ? 1 : -1;
                }

                enemy.facing = enemy.CalcFacing(target.transform.position - enemy.gameObject.transform.position);

                Vector2 movementDirection = ai.movementDirectionSolver.GetDirectionToMove(steeringBehaviors, aiData, dirCoeff*90);
                body.velocity = 0.8f * speed * movementDirection;
                anim.CrossFade(animationName + CalcBodyDirection(movementDirection) + '_' + enemy.facing, 0);
                anim.speed = 1;
                enemy.agressivity += 2*Time.deltaTime;
            }

            public float Analyze()
            {
                target = enemy.target;
                return (-10 * enemy.agressivity / (enemy.gameObject.transform.position - target.transform.position).magnitude + 2.5f * enemy.baseAgressivity)* (1 + Mathf.Exp((Time.time - enemy.lastMoveTime) / avoidDecisionTime));
            }
            public void PrepareBehavior()
            {
                enemy.isBlockUp = false;
                enemy.isAttacking = false;
            }
        }

        public class KeepDistanceBehavior : MoveBehaviorBase, IEnemyBehavior
        {
            float distance;
            bool jib;
            public KeepDistanceBehavior(Enemy enemy, string animName, float distance, bool jib = false) : base(enemy, animName) 
            {
                this.distance = distance;
                this.jib = jib;
            }
            public override void Update()
            {
                base.Update();

                int angle = ((enemy.gameObject.transform.position - target.transform.position).magnitude > distance) ? 0 : 180;

                Vector2 movementDirection = ai.movementDirectionSolver.GetDirectionToMove(steeringBehaviors, aiData, angle);

                body.velocity = speed * movementDirection;
                if (body.velocity.magnitude <= 0.01f)
                {
                    anim.CrossFade(animationName.Split('_')[0] + "_Idle_" + enemy.facing, 0);
                    anim.speed = 1;
                }
                else
                {
                    anim.CrossFade(animationName + CalcBodyDirection(movementDirection) + '_' + enemy.facing, 0);
                    anim.speed = 1;
                    if(jib)
                        enemy.facing = enemy.CalcFacing(target.transform.position - enemy.transform.position);
                    else
                        enemy.facing = enemy.CalcFacing(movementDirection);

                }
                enemy.agressivity += Time.deltaTime;
            }

            public float Analyze()
            {
                target = enemy.target;
                float k = (enemy.transform.position-enemy.avoidDecisionPosition).magnitude/(Time.time - enemy.lastMoveTime);
                return 3f * Mathf.Sqrt(Mathf.Abs(distance - (enemy.gameObject.transform.position - target.transform.position).magnitude))* distance * Mathf.Sqrt(enemy.agressivity) * k;
            }
            public void PrepareBehavior()
            {
                enemy.isBlockUp = false;
                enemy.isAttacking = false;
            }
        }


    }
    namespace DefendBehaviors
    {
        public abstract class DefendBehaviorBase
        {
            protected Enemy enemy;
            protected GameObject target;
            protected Animator anim;
            protected Rigidbody body;
            protected string animationName;
            protected float heroAtkRange;
            public DefendBehaviorBase(Enemy enemy, string animName)
            {
                this.enemy = enemy;
                anim = enemy.anim;
                animationName = animName;
                body = enemy.rigid;
            }

            public void InitNewStats() { }
        }
        public class BlockBehavior : DefendBehaviorBase, IEnemyBehavior
        {
            public BlockBehavior(Enemy enemy, string animName) : base(enemy, animName) { }
            public void Update()
            {
                enemy.canChooseBehavior = Time.time - enemy.blockPlacedTime < 2 ? false : true;

                body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, 0.15f);

                enemy.facing = enemy.CalcFacing(target.transform.position - enemy.gameObject.transform.position);

                anim.CrossFade(animationName + '_' + enemy.facing, 0);
                anim.speed = 1;
                enemy.isBlockUp = true;
                enemy.agressivity += 4*Time.deltaTime;
            }

            public float Analyze()
            {
                target = enemy.target;

                int k = target.GetComponent<Hero>().mode == Hero.eMode.attack ? 1 : 0;

                float distance = (enemy.gameObject.transform.position - target.transform.position).magnitude;

                float result = -10 * enemy.agressivity / distance + 4f * enemy.baseAgressivity + Random.value * Mathf.Exp(8*k/distance);
                return result;
            }
            public void PrepareBehavior()
            {
                enemy.blockPlacedTime = Time.time;
                body.velocity = Vector3.zero;
                enemy.isAttacking = false;
            }
        }
    }
}