using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEffectsManager : MonoBehaviour
{
    public static WorldEffectsManager instance;

    public GameObject invokationEffectPrefab;
    public GameObject blockEffectPrefab;
    public GameObject parryEffectPrefab;
    public GameObject takeDamageEffectPrefab;

    private void Awake()
    {
        if (instance is null)
            instance = this;
    }
}
