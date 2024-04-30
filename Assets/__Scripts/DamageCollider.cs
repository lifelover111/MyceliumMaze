using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class DamageCollider : MonoBehaviour
{
    private new Collider collider;
    [Header("Damage")]
    public float physicalDamage;
    public float mentalDamage; 

    [Header("Concentration Damage")]
    public bool withConcentrationDamage = true;
    public float concentrationDamage;
    public float concentrationDamageBlockMultiplier = 2;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("VFX")]
    public bool withVFX = true;

    [Header("Blood")]
    public bool withBlood = true;

    public event System.Action OnHitEnemy = delegate { };

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
        OnHitEnemy?.Invoke();
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
            DamageTarget(damageTarget, withConcentrationDamage);
        }
    }

}
