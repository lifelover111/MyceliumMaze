using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Test3DEnemy : Enemy
{
    protected override void Awake()
    {
        name = "TestEnemy";
        healthCoeff = 10;
        concCoeff = 2.5f;

        soulsPerLevel = 15;

        base.Awake();

        behaviors.Add(new EnemyBehaviors.MoveBehaviors.NewFollowBehavior(this));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.NewAvoidBehavior(this));
        behaviors.Add(new EnemyBehaviors.MoveBehaviors.NewSurroundBehavior(this));
        behaviors.Add(new EnemyBehaviors.AttackBehaviors.NewCommonAttackBehavior(this, weapon, 0.5f, 1, 1));
    }

    protected override void Update()
    {
        base.Update();
    }

}
