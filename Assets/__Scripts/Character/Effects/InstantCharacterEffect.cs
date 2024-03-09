using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantCharacterEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int instantEffectId;

    public virtual void ProcessEffect(CharacterManager character)
    {

    }
}
