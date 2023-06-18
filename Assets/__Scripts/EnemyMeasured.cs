using UnityEngine;

[System.Serializable] public class EnemyMeasured
{
    [SerializeField] public GameObject enemy;
    [SerializeField] public int weight;

    public EnemyMeasured(GameObject enemy, int weight)
    { 
        this.enemy = enemy; 
        this.weight = weight; 
    }
}
