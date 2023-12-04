using System.Collections;
using System.Collections.Generic;
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
    }

    IEnumerator ShowThenHideCoroutine()
    {
        float time = Time.time;
        while (image.color.a < 1 - eps)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, Mathf.Sin(Time.time - time)));
            yield return null;
        }
        while (image.color.a > 0 + eps)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, Mathf.Sin(Time.time - time)));
            yield return null;
        }
    }

}
