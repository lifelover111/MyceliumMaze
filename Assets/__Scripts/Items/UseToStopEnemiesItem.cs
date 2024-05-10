using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(menuName = "Items/Active/Use To Stop Enemies")]
public class UseToStopEnemiesItem : ActiveItem
{
    [Header("Stopping time")]
    public float seconds;

    [Header("Effect Prefab")]
    public GameObject effectPrefab;

    public override void Reset()
    {
        base.Reset();
        OnTryUse += CastStopEnemies;
    }

    private bool CastStopEnemies(PlayerManager player)
    {
        if (player.isPerformingAction)
            return false;

        var effect = Instantiate(effectPrefab);
        effect.transform.position = player.transform.position + Vector3.up;

        if (player.CurrentRoom == null)
            return true;

        foreach (var enemy in player.CurrentRoom.Enemies)
        {
            if(enemy != null)
                enemy.StartCoroutine(StopEnemyCoroutine(enemy));
        }

        return true;
    }

    private IEnumerator StopEnemyCoroutine(AICharacterManager aICharacter)
    {
        aICharacter.movable = false;
        yield return new WaitForSecondsRealtime(seconds);
        aICharacter.movable = true;
    }

}
