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

    //protected Animator _doorAnim;
    //public Animator doorAnim { get { return _doorAnim; } }
    //[SerializeField] Animator _doorAnim;
    private void Awake()
    {
        //_doorAnim = GetComponentInChildren<Animator>();
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
    //
    public void OpenAllDoors()
    {
        //if (_doorAnim == null)
        //{
        //    Debug.LogError("Animator!");
        //    return;
        //}
        foreach (var door in transitionsForward)
        {
            
            if (door != null)
            {
                //door.transform.GetChild(1).gameObject.SetActive(false);
                //door.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                door.OpenDoor();
                //_doorAnim.SetBool("IsOpen", true);
            }
        }
        foreach (var door in transitionsBackward)
        {
            
            if (door != null)
            {
                //door.transform.GetChild(1).gameObject.SetActive(false);
                //door.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                door.OpenDoor();
                //_doorAnim.SetBool("IsOpen", true);
            }
        }
    }


    private void CloseAllDoors()
    {
        //if (_doorAnim == null)
        //{
        //    Debug.LogError("Animator!");
        //    return;
        //}
       
        foreach (var door in transitionsForward)
        {
           
            if (door != null)
            {
                //door.transform.GetChild(1).gameObject.SetActive(true);
                //door.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                door.CloseDoor();
                //door.fogAnimator.SetTrigger("Close");
                //_doorAnim.SetBool("IsOpen", false);
            }
        }
        foreach (var door in transitionsBackward)
        {
            if (door != null)
            {
                //door.transform.GetChild(1).gameObject.SetActive(true);
                //door.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                door.CloseDoor();
                //door.fogAnimator.SetTrigger("Close");
                // _doorAnim.SetBool("IsOpen", false);
            }
        }
    }

    //public void OnEnemyDeath()
    //{
    //    enemies.Remove(enemy.gameObject);
    //    int enemyCount = EnemyAnchor.childCount;
    //    if (enemyCount > 0)
    //    {
    //        CloseAllDoors();
    //    }
    //    else
    //        OpenAllDoors();
    //}

    //private void OnDestroy()
    //{
    //    Enemy.OnEnemyDeath -= HandleEnemyDeath;
    //}

    //

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
            Enemy.OnEnemyDeath += EnemyDeath;
            //
            //CloseAllDoors();
            //Enemy.OnEnemyDeath += HandleEnemyDeath;
            //
            enemy.SetActive(false);
            enemy.transform.SetParent(EnemyAnchor, true);
            
        }
    }

    public void EnemyDeath()
    {
        //GameObject enemy = Instantiate(EnemyPrefabManager.instance.enemyPrefabs[Random.Range(0, EnemyPrefabManager.instance.enemyPrefabs.Length)]);
        //enemies.Remove(EnemyAnchor.gameObject);
        Debug.Log(EnemyAnchor.childCount);
        //1 из-за проблем с задержкой
        if (EnemyAnchor.childCount == 1)
        {

            OpenAllDoors();
        }
        //else
        //    CloseAllDoors();
    }


    public void WakeEnemies()
    {
        if (EnemyAnchor.childCount == 0)
            return;
        
        CloseAllDoors();
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