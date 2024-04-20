using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoom : Room
{
    [SerializeField] BossDoor bossDoor;
    [SerializeField] BossDoor nextLevelDoor;
    [SerializeField] Transform bossSpawnPoint;
    
    private BossCharacterManager boss;

    public event System.Action OnStartFight;

    protected override void Awake()
    {
        base.Awake();
        bossDoor.OnPlayerTriggerEnter += PlayerEnters;
        nextLevelDoor.OnPlayerTriggerEnter += GoToNextLevel;
    }

    private void PlayerEnters()
    {
        SpawnBoss();
        bossDoor.CloseDoor();
        nextLevelDoor.CloseDoor();
        StartCoroutine(ShowBossCoroutine());
    }

    private void GoToNextLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void SpawnBoss()
    {
        var bossGO = Instantiate(EnemyPrefabManager.instance.bossPrefabs[Random.Range(0, EnemyPrefabManager.instance.bossPrefabs.Length)]);
        boss = bossGO.GetComponent<BossCharacterManager>();
        bossGO.transform.position = bossSpawnPoint.position;
        bossGO.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
        boss.isSleeping = true;
        boss.OnDead += () =>
        {
            bossDoor.OpenDoor();
            nextLevelDoor.OpenDoor();
        };
    }

    private IEnumerator ShowBossCoroutine() 
    {
        var player = PlayersInGameManager.instance.playerList.First();

        player.canMove = false;
        CameraPivot.instance.target = boss.transform;
        yield return new WaitForSecondsRealtime(3);
        CameraPivot.instance.target = player.transform;
        player.canMove = true;
        boss.isSleeping = false;
        OnStartFight?.Invoke();
    }
}
