using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefabManager : MonoBehaviour
{
    public static EnemyPrefabManager instance;
    [SerializeField] public GameObject[] enemyPrefabs;
    private void Awake()
    {
        instance = this;
    }
}
