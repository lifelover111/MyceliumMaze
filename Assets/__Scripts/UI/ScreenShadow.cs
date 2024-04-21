using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenShadow : MonoBehaviour
{
    UnityEngine.UI.Image image;
    float eps = 0.0025f;
    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }
    private void Start()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        Door.OnTransition += () => {
            StartCoroutine(ShowThenHideCoroutine()); 
        };
        BossRoom.OnLoadNextLevel += () => {
            StartCoroutine(ShowThenHideCoroutine()); 
            PlayersInGameManager.instance.playerList.First().OnDead -= OnPlayerDies;
        };
        PlayersInGameManager.instance.playerList.First().OnDead += OnPlayerDies;
    }

    private void OnPlayerDies()
    {
        StartCoroutine(ShowCoroutine());
    }


    IEnumerator ShowThenHideCoroutine()
    {
        float time = Time.time;
        while (image.color.a < 1 - eps)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, Door.transitionSpeed*Mathf.Sin(Time.time - time)));
            yield return null;
        }
        while (image.color.a > 0 + eps)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, Door.transitionSpeed*Mathf.Sin(Time.time - time)));
            yield return null;
        }
    }

    IEnumerator ShowCoroutine()
    {
        float time = Time.time;
        yield return new WaitForSeconds(2.5f);
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(1, 0, Door.transitionSpeed * Mathf.Sin(Time.time - time)));
            if(image.color.a > 1)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            yield return null;
        }
    }

}
