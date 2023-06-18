using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShadow : MonoBehaviour
{
    SpriteRenderer sRend;
    float fadeInterpolationParam = 0;
    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (sRend.color.a != 0)
        {
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, Mathf.Lerp(1, 0, fadeInterpolationParam));
            fadeInterpolationParam += 0.005f;
        }
        else
            Destroy(gameObject);
    }
}
