using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEffectsManager : MonoBehaviour
{
    public static WorldEffectsManager instance;

    public GameObject invokationEffectPrefab;

    private void Awake()
    {
        instance = this;
    }
}
