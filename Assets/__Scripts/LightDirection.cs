using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirection : MonoBehaviour
{
    [SerializeField] Sprite[] dirs;
    SpriteRenderer sRend;
    Hero hero;
    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        hero = transform.GetComponentInParent<Hero>();
    }
    void Update()
    {
        sRend.sprite = dirs[hero.facing];
    }
}
