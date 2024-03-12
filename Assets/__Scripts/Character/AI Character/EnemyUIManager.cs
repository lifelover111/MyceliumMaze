using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    [SerializeField] private GuiHealthBar healthBar;
    [SerializeField] private GuiConcentrationBar concentrationBar;
    private CharacterManager character;

    private void Awake()
    {
        healthBar.gameObject.SetActive(false);
        concentrationBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (character is null || character.isDead)
            return;

        transform.position = character.transform.position;
    }



    public void Enable(StatsManager statsManager)
    {
        healthBar.gameObject.SetActive(true);
        healthBar.Init(statsManager);
        concentrationBar.gameObject.SetActive(true);
        concentrationBar.Init(statsManager);
        character = statsManager.gameObject.GetComponent<CharacterManager>();
        character.OnDead += () => { Destroy(gameObject); };
    }

}
