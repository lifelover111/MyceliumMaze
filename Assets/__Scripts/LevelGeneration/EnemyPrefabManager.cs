using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefabManager : MonoBehaviour
{
    public static EnemyPrefabManager instance;
    [SerializeField] GameObject[] enemyPrefabs;
    public List<KeyValuePair<int, GameObject>> weightedEnemyPrefabs = new List<KeyValuePair<int, GameObject>>();
    
    public GameObject[] bossPrefabs;

    private void Awake()
    {
        if(instance is null)
            instance = this;

        GetEnemiesWeight();
    }

    private void GetEnemiesWeight()
    {
        foreach(var go in enemyPrefabs)
        {
            var aiCharacter = go.GetComponent<AICharacterManager>();
            weightedEnemyPrefabs.Add(new KeyValuePair<int, GameObject>(aiCharacter.SpawnCost, go));
        }
    }
}
