using UnityEngine;
using UnityEngine.UI;

public class ImageMovement : MonoBehaviour
{
    public RectTransform imageRectTransform; // —сылка на RectTransform изображени€
    public float speed = 0.5f; // —корость перемещени€ изображени€

    private Vector2 startPosition;
    private Vector2 targetPosition;

    void Start()
    {
        // «адаем начальную и целевую позиции изображени€, учитыва€ его масштаб
        startPosition = imageRectTransform.anchoredPosition;
        targetPosition = new Vector2(startPosition.x, startPosition.y - 375); // Ќапример, целева€ позици€ на 100 пикселей выше начальной
    }

    void Update()
    {
        // »спользуем Lerp дл€ плавного перемещени€ изображени€ от начальной позиции к целевой
        imageRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, Time.time * speed);
    }
}
