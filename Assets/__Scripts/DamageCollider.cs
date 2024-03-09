using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private new Collider collider;
    [Header("Damage")]
    public float physicalDamage;
    public float mentalDamage;
    public float concentrationDamage;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    public virtual void Enable()
    {
        collider.enabled = true;
    }

    public virtual void Disable()
    {
        collider.enabled = false;
        charactersDamaged.Clear();
    }

    protected virtual void DamageTarget(CharacterManager target, bool withConcentrationDamage = true)
    {
        if (charactersDamaged.Contains(target)) return;
        charactersDamaged.Add(target);

        if (withConcentrationDamage)
        {
            TakeConcentrationDamageEffect concentrationDamageEffect = Instantiate(WorldCharacterEffectManager.instance.concentrationDamageEffect);
            concentrationDamageEffect.concentrationDamage = concentrationDamage;
            target.effectsManager.ProcessInstantEffect(concentrationDamageEffect);
        }

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.damageEffect);
        damageEffect.physycalDamage = physicalDamage;
        damageEffect.mentalDamage = mentalDamage;
        damageEffect.contactPoint = contactPoint;
        target.effectsManager.ProcessInstantEffect(damageEffect);
    }

    protected virtual void Awake()
    {
        collider = GetComponent<Collider>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if(damageTarget is not null)
        {
            contactPoint = other.ClosestPointOnBounds(transform.position);
            DamageTarget(damageTarget);
        }
    }

}
