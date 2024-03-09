using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectManager : MonoBehaviour
{
    public static WorldCharacterEffectManager instance;

    [SerializeField] public TakeDamageEffect damageEffect;
    [SerializeField] public TakeConcentrationDamageEffect concentrationDamageEffect;

    [SerializeField] List<InstantCharacterEffect> instantEffects;

    private void Awake()
    {
        if(instance is null) instance = this;
        GenerateEffectIDs();
    }

    private void GenerateEffectIDs()
    {
        for(int i = 0; i < instantEffects.Count; i++)
        {
            instantEffects[i].instantEffectId = i;
        }
    }
}
