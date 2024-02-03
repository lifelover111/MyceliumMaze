using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] LevelGenerator.RoomNode.RoomType type;
    [SerializeField] Transform[] doorsForward;
    [SerializeField] Transform[] doorsBackward;
    [SerializeField] Transform[] enemySpawnPoints;
    Transform EnemyAnchor;
    [Header("Sets dynamycally")]
    public Door[] transitionsForward;
    public Door[] transitionsBackward;
    List<GameObject> enemies = new List<GameObject>();
    int depth;
    private void Awake()
    {
        transitionsForward = new Door[doorsForward.Length];
        transitionsBackward = new Door[doorsBackward.Length];
        for(int i = 0; i < doorsForward.Length; i++)
        {
            transitionsForward[i] = doorsForward[i].GetComponent<Door>();
        }
        for (int i = 0; i < doorsBackward.Length; i++)
        {
            transitionsBackward[i] = doorsBackward[i].GetComponent<Door>();
        }
        EnemyAnchor = new GameObject(nameof(EnemyAnchor)).transform;
        EnemyAnchor.SetParent(transform);
    }

    public void ConnectWith(Room[] toConnect, int[] connectionsBackwardCount)
    {
        if (toConnect.Length > doorsForward.Length)
        {
            Debug.LogError("Connection error: too many rooms to connect with one");
            return;
        }
        int maxShift = doorsForward.Length - toConnect.Length;
        int shift = 0;
        for (int i = 0; i < toConnect.Length; i++)
        {
            int busyDoorsInNextRoomCount = toConnect[i].doorsBackward.Length - toConnect[i].transitionsBackward.Where(x => x.IsEmptyTransition()).Count();
            shift += Random.Range(0, maxShift + 1);
            maxShift -= shift;
            Door[] doorsToConnect = toConnect[i].transitionsBackward.Reverse().TakeWhile(door => door.IsEmptyTransition()).Reverse().ToArray();
            transitionsForward[i + shift].ConnectWith(doorsToConnect[Random.Range(0, doorsToConnect.Length - (connectionsBackwardCount[i] - busyDoorsInNextRoomCount))]);
        }
    }

    public LevelGenerator.RoomNode.RoomType GetRoomType()
    {
        return type;
    }

    public int GetEntersNum()
    {
        return doorsBackward.Length;
    }
    public int GetExitsNum() 
    {
        return doorsForward.Length;
    }

    public void SpawnEnemies()
    {
        if (enemySpawnPoints.Length == 0)
            return;
        if (enemySpawnPoints == null)
            Debug.Log(gameObject.name);
        foreach (var p in enemySpawnPoints)
        {
            if (EnemyPrefabManager.instance.enemyPrefabs.Length == 0)
                return;
            GameObject enemy = Instantiate(EnemyPrefabManager.instance.enemyPrefabs[Random.Range(0, EnemyPrefabManager.instance.enemyPrefabs.Length)]);
            enemy.transform.position = p.position;
            enemies.Add(enemy);
            enemy.SetActive(false);
            enemy.transform.SetParent(EnemyAnchor, true);
        }
    }

    public void WakeEnemies()
    {
        foreach (GameObject e in enemies)
        {
            e.SetActive(true);
        }
        enemies.Clear();
    }

    public int GetDepth()
    {
        return depth;
    }
    public void SetDepth(int d)
    {
        depth = d;
    }
}