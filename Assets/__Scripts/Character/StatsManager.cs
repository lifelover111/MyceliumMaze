using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [HideInInspector] 
    protected CharacterManager character;

    [SerializeField] float _maxConcentration;
    [SerializeField] float _maxHealth;
    [SerializeField] float _concentration;
    [SerializeField] float _health;
    public float MaxConcentration => _maxConcentration;
    public float MaxHealth => _maxHealth;
    public float Concentration { get { return _concentration; } set { ResetConcentrationRegenerationTimer(_concentration, value); _concentration = value; } }
    public float Health { get { return _health; } set { _health = value > _maxHealth ? _maxHealth : value; if (_health <= 0) HandleDeath(); } }
    public bool IsDead => character.isDead;

    [Header("Concentration Regeneration")]
    private float concentrationRegenerationTimer;
    private float concentrationTickTimer;
    [SerializeField] float concentrationRegenerationDelay;
    [SerializeField] float concentrationRegenAmount;
    
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        Concentration = 0;
        Health = _maxHealth;
    }

    public virtual void TryHeal()
    {
        if (character.isPerformingAction) return;
        character.canMove = false;
        character.animatorManager.PlayTargetActionAnimation(character.animationKeys.Heal, true);
    }

    public virtual void Heal(float amount, int flaskCost = 0)
    {
        Health += amount;
    }


    public virtual void OverflowConcentration()
    {
        HandleConcentrationOveflow();
    }

    public virtual void RegenerateConcentration(float multiplier = 1, float delayMultiplier = 1, bool regenerateWhilePerformAction = false)
    {
        if(!regenerateWhilePerformAction)
        {
            if (character.isPerformingAction)
                return;
        }


        concentrationRegenerationTimer += Time.deltaTime;

        if (concentrationRegenerationTimer >= concentrationRegenerationDelay * delayMultiplier)
        {
            if (Concentration > 0)
            {
                concentrationTickTimer += Time.deltaTime;

                if (concentrationTickTimer >= 0.1f)
                {
                    concentrationTickTimer = 0;
                    Concentration = Concentration > 0 ? Concentration - concentrationRegenAmount * multiplier : 0;
                }
            }
        }
    }

    protected virtual void HandleDeath()
    {
        if (character.isDead) return;
        
        character.isDead = true;
        character.StartCoroutine(character.ProcessDeathEvent());
    }

    protected virtual void HandleConcentrationOveflow()
    {
        if (character.isDead) return;
        if (Concentration < MaxConcentration) return;

        StartCoroutine(character.ProcessConcentrationOverflowEvent());
    }

    private void ResetConcentrationRegenerationTimer(float oldValue, float newValue)
    {
        if(newValue > oldValue)
        {
            concentrationRegenerationTimer = 0;
        }
    }
}
