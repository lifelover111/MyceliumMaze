using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComponentKeeper : MonoBehaviour
{
    public static LevelComponentKeeper instance;

    [SerializeField] public GameObject[] frontWallPrefabs;
    [SerializeField] public GameObject[] backWallPrefabs;
    [SerializeField] public GameObject[] envPrefabs;
    [SerializeField] public GameObject[] addEnvPrefabs;
    [SerializeField] public GameObject doorTopPrefab;
    [SerializeField] public GameObject doorBottomPrefab;
    [SerializeField] public GameObject doorRightPrefab;
    [SerializeField] public GameObject doorLeftPrefab;

    [SerializeField] public GameObject bonfire;

    [SerializeField] public EnemyMeasured[] enemyPrefabs;

    void Awake()
    {
        instance = this;
    }
}
