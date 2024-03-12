using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] GameObject concentrationBarPrefab;
    private GuiHealthBar healthBar;
    private GuiConcentrationBar concentrationBar;
    private CharacterManager character;

    private void Awake()
    {
        healthBar = Instantiate(healthBarPrefab).GetComponent<GuiHealthBar>();
        concentrationBar = Instantiate(concentrationBarPrefab).GetComponent<GuiConcentrationBar>();
        healthBar.transform.SetParent(transform, false);
        concentrationBar.transform.SetParent(transform, false);
        healthBar.gameObject.SetActive(false);
        concentrationBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (character is null)
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
