using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrafabs : MonoBehaviour
{
    public static EnemyPrafabs instance;
    [SerializeField] public GameObject[] enemyPrefabs;
    private void Awake()
    {
        instance = this;
    }
}
