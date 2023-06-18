using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerController : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public bool isStatic = false;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        float yPosition = transform.position.y;

        int sortingOrder = Mathf.RoundToInt(-yPosition * 100);

        spriteRenderer.sortingOrder = sortingOrder;
    }
    void Update()
    {
        if (isStatic)
            return;

        float yPosition = transform.position.y;

        int sortingOrder = Mathf.RoundToInt(-yPosition * 100);

        spriteRenderer.sortingOrder = sortingOrder;
    }
}
