using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShadow : MonoBehaviour
{
    SpriteRenderer sRend;
    float fadeInterpolationParam = 0;
    Vector3 camPosDelta;
    float eps = 0.0025f;
    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, 0);

        camPosDelta = Camera.main.transform.position - transform.position;
        camPosDelta.z = -5;
        Door.OnTransition += () => {
            fadeInterpolationParam = 0;
            transform.position = Camera.main.transform.position - camPosDelta;
            StartCoroutine(nameof(ShowThenHideCoroutine)); 
        };
    }

    IEnumerator ShowThenHideCoroutine()
    {
        float time = Time.time;
        while (sRend.color.a < 1 - eps)
        {
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, Mathf.Lerp(0, 1, Mathf.Sin(Time.time - time)));
            yield return null;
        }
        while (sRend.color.a > 0 + eps)
        {
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, Mathf.Lerp(0, 1, Mathf.Sin(Time.time - time)));
            yield return null;
        }
    }

    private void Update()
    {
        /*
        if (sRend.color.a != 0)
        {
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, Mathf.Lerp(1, 0, fadeInterpolationParam));
            fadeInterpolationParam += 0.005f;
        }
        */
    }
}
