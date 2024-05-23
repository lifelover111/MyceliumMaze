using UnityEngine;
using UnityEngine.UI;

public class ImageMovement : MonoBehaviour
{
    public RectTransform imageRectTransform; // ������ �� RectTransform �����������
    public float speed = 0.5f; // �������� ����������� �����������

    private Vector2 startPosition;
    private Vector2 targetPosition;

    void Start()
    {
        // ������ ��������� � ������� ������� �����������, �������� ��� �������
        startPosition = imageRectTransform.anchoredPosition;
        targetPosition = new Vector2(startPosition.x, startPosition.y - 375); // ��������, ������� ������� �� 100 �������� ���� ���������
    }

    void Update()
    {
        // ���������� Lerp ��� �������� ����������� ����������� �� ��������� ������� � �������
        imageRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, Time.time * speed);
    }
}
