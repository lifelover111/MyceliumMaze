using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CameraMovement : MonoBehaviour
{
    public GameObject secondObject; // ������ ������ ��� ���������
    public GameObject firstObject;  // ������ ������, ������� ����� ��������
    public float moveSpeedX = 0.5f; // �������� ����������� �� ��� X
    public float moveSpeedY = 0.5f; // �������� ����������� �� ��� Y
    public float moveRightDistance = 2.0f; // ���������� ��� �������� ������
    public float moveUpDistance = 1.0f; // ���������� ��� �������� �����
    public float verticalOffset = 1.0f; // ������������ ��������
    public float fadeInDelay = 5.0f; // �������� ����� ���������� ������� �������
    public float fadeDuration = 1.0f; // ����������������� ���������

    public Image fadeImage; // Image ��� ����������

    private Vector3 startPosition;
    private bool movedRight = false;
    private bool movedUp = false;
    private bool stopMovement = false;
    private bool pausedAtY7 = false;
    private bool fading = false;
    private bool shouldFadeOut = false; // ���� ��� ������ �� ����������
    private bool shouldFadeIn = false; // ���� ��� ������ ����������
    private Camera mainCamera;

    void Start()
    {
        startPosition = transform.position;
        mainCamera = Camera.main;

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0); // ���������� ��������� ����� �� 0
        }
        else
        {
            Debug.LogError("Fade image is not assigned in the inspector.");
        }
    }

    void Update()
    {
        if (stopMovement)
        {
            return;
        }

        if (!movedRight)
        {
            float newX = Mathf.MoveTowards(transform.position.x, startPosition.x + moveRightDistance, moveSpeedX * Time.deltaTime);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            if (transform.position.x >= startPosition.x + moveRightDistance)
            {
                movedRight = true;
            }
        }
        else if (!movedUp)
        {
            float newY = Mathf.MoveTowards(transform.position.y, startPosition.y + moveUpDistance, moveSpeedY * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            if (transform.position.y >= startPosition.y + moveUpDistance)
            {
                movedUp = true;
            }
            else if (transform.position.y >= 5.5f && !pausedAtY7)
            {
                StartCoroutine(PauseAndFade());
                pausedAtY7 = true;
            }
        }

        if (movedUp && !shouldFadeOut && !fading)
        {
            float newY = transform.position.y + verticalOffset * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (transform.position.y >= 15.5f && pausedAtY7)
        {
            StartCoroutine(FadeOut());
            //shouldFadeOut = true;
        }

        if (transform.position.y >= 19.5f)
        {
            stopMovement = true;
        }
    }

    IEnumerator PauseAndFade()
    {
        stopMovement = true;

        yield return new WaitForSeconds(2.0f);

        yield return StartCoroutine(FadeIn());

        stopMovement = false;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        fading = true; 
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���������, ��� ����� ����� 0
        fadeImage.color = new Color(0, 0, 0, 0);
        fading = false; // ��������� ������� ����������

        // ������������ ������ ������
        if (secondObject != null)
        {
            secondObject.SetActive(true);
        }
    }

}