using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        character.OnDead += () => StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        var renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        bool isFullyTransparent = renderers.Select(r => r.color.a > 0).Count() == 0;

        while (!isFullyTransparent)
        {
            isFullyTransparent = true;
            foreach (var renderer in renderers)
            {
                renderer.color = Color.Lerp(renderer.color, new Color(0, 0, 0, 0), Time.deltaTime);
                if(renderer.color.a > 0.1f)
                    isFullyTransparent = false;
            }
            yield return null;
        }

        Destroy(gameObject);
    }

}
