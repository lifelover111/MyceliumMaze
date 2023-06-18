using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    protected override void Awake()
    {
        healthCoeff = 10;
        concCoeff = 10;

        stats = new Stats(2, 2, 2, 2);

        soulsPerLevel = 50;

        base.Awake();


        atkHitBoxes.Add(transform.GetChild(0).gameObject);
        atkHitBoxes.Add(transform.GetChild(1).gameObject);
        foreach (var box in atkHitBoxes)
            box.SetActive(false);

        behaviors.Add(new EnemyBehaviors.AttackBehaviors.CommonAttackBehavior(this, "TestEnemy_Attack_0_", atkHitBoxes[0], 0.5f, 1, 2));
        behaviors.Add(new EnemyBehaviors.AttackBehaviors.CommonAttackBehavior(this, "TestEnemy_Attack_1_", atkHitBoxes[1], 0.5f, 1, 3));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.FollowBehavior(this, "TestEnemy_Move_"));
        behaviors.Add(new EnemyBehaviors.DefendBehaviors.BlockBehavior(this, "TestEnemy_Block_"));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.AvoidBehavior(this, "TestEnemy_Move_"));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.SurroundBehavior(this, "TestEnemy_Move_"));
    }

    protected override void Update()
    {
        base.Update();
    }

}
