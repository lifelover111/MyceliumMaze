using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ConcentrationDamageCollider : DamageCollider
{
    public CharacterManager characterCausingDamage;

    protected override void DamageTarget(CharacterManager target, bool withConcentrationDamage = true)
    {
        if (target == characterCausingDamage) return;
        //base.DamageTarget(target, withConcentrationDamage);

        if (charactersDamaged.Contains(target)) return;
        charactersDamaged.Add(target);

        TakeConcentrationDamageEffect concentrationDamageEffect = Instantiate(WorldCharacterEffectManager.instance.concentrationDamageEffect);
        concentrationDamageEffect.concentrationDamage = concentrationDamage;
        target.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
        if (target.statsManager.Concentration >= target.statsManager.MaxConcentration)
        {
            //поменять анимацию
            target.statsManager.OverflowConcentration();
            target.animatorManager.PlayTargetHitAnimation(target.animationKeys.BreakBlock, true);
        }
    }
}
