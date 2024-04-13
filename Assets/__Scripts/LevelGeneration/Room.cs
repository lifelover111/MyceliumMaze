using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    int enemyCount = 0;
    int depth;

    [Header("Fight Parameters")]
    [SerializeField] private int maxWaveCost = 0;

    [Header("Long Fight Parameters")]
    private int waveCount = 0;
    private int currentWave = 0;

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

        switch (type)
        {
            case LevelGenerator.RoomNode.RoomType.quickFight:
                waveCount = 1; 
                break;
            case LevelGenerator.RoomNode.RoomType.longFight: 
                waveCount = Random.Range(2, 5); 
                break;
        }
    }
    //
    public void OpenAllDoors()
    {
        foreach (var door in transitionsForward)
        {
            door.OpenDoor();
        }
        foreach (var door in transitionsBackward)
        {
            door.OpenDoor();
        }
    }


    private void CloseAllDoors()
    {
        foreach (var door in transitionsForward)
        {
            door.CloseDoor();
        }
        foreach (var door in transitionsBackward)
        {
            door.CloseDoor();
        }
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
            Debug.LogError("Tried to spawn enemies in " + gameObject.name);

        currentWave++;
        Debug.Log(currentWave + " / " + waveCount);

        if (EnemyPrefabManager.instance.weightedEnemyPrefabs.Count == 0)
            return;

        var potentialSpawnPoints = enemySpawnPoints.ToList();
        int currentSpawnCost = 0;

        while(potentialSpawnPoints.Count > 0 && currentSpawnCost < maxWaveCost && EnemyPrefabManager.instance.weightedEnemyPrefabs.Where(p => p.Key <= maxWaveCost - currentSpawnCost).Count() > 0)
        {
            var possibleEnemies = EnemyPrefabManager.instance.weightedEnemyPrefabs.Where(p => p.Key <= maxWaveCost - currentSpawnCost).ToArray();
            var chosenEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Length)];
            currentSpawnCost += chosenEnemy.Key;
            GameObject enemy = Instantiate(chosenEnemy.Value);

            var currentSpawnPoint = potentialSpawnPoints[Random.Range(0, potentialSpawnPoints.Count)];
            potentialSpawnPoints.Remove(currentSpawnPoint);
            enemy.transform.position = currentSpawnPoint.position;

            enemies.Add(enemy);
            var characterManager = enemy.GetComponent<CharacterManager>();
            characterManager.OnDead += EnemyDied;
            enemy.SetActive(false);
            enemy.transform.SetParent(EnemyAnchor, true);
            enemyCount++;
        }

    }

    public void EnemyDied()
    {
        enemyCount--;
        if (enemyCount == 0)
        {
            if (currentWave == waveCount)
                OpenAllDoors();
            else
            {
                SpawnEnemies();
                WakeEnemies();
            }
        }
    }

    public void DestroyEnemies()
    {
        for(int i = 0; i < EnemyAnchor.childCount; i++)
            Destroy(EnemyAnchor.GetChild(i).gameObject);
    }


    public void WakeEnemies()
    {
        if (EnemyAnchor.childCount == 0)
            return;
        
        foreach (GameObject e in enemies)
        {
            if (currentWave == 1)
                e.SetActive(true);
            else
                StartCoroutine(WakeEnemyCoroutine(e));
        }
        enemies.Clear();
        CloseAllDoors();
    }

    public int GetDepth()
    {
        return depth;
    }
    public void SetDepth(int d)
    {
        depth = d;
    }

    private IEnumerator WakeEnemyCoroutine(GameObject enemyGameObject)
    {
        var enemy = enemyGameObject.GetComponent<AICharacterManager>();
        enemy.isSleeping = true;
        var normalPosition = enemyGameObject.transform.position;
        enemy.transform.position += Vector3.down * enemy.characterController.height*enemyGameObject.transform.localScale.y;
        var startPosition = enemy.transform.position;
        enemyGameObject.SetActive(true);

        var invokation = Instantiate(WorldEffectsManager.instance.invokationEffectPrefab);
        invokation.transform.position = new Vector3(enemyGameObject.transform.position.x, invokation.transform.position.y, enemyGameObject.transform.position.z);

        float risingDuration = 3f;

        float timeRisingStarts = Time.time;
        while (Time.time - timeRisingStarts < 4 - risingDuration)
            yield return null;

        timeRisingStarts = Time.time;

        while (enemy.transform.position.y < normalPosition.y)
        {
            enemy.transform.position = Vector3.Lerp(startPosition, normalPosition, (Time.time - timeRisingStarts)/risingDuration);
            yield return null;
        }

        Destroy(invokation);
        enemy.isSleeping = false;
    }
}