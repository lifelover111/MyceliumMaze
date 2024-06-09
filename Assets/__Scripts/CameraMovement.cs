using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public GameObject secondObject; 
    public GameObject firstFlashObject; 
    public GameObject secondFlashObject; 
    public float moveSpeedX = 0.5f; 
    public float moveSpeedY = 0.5f; 
    public float moveRightDistance = 2.0f; 
    public float fadeDuration = 4.0f; 
    public float moveUpOffset = 5.0f; 
    public float zoomOutDuration = 2.0f; 
    public float increasedMoveSpeedY = 2.0f; 

    public Image fadeImage; 

    private Vector3 startPosition;
    private bool stopMovement = false;
    private int movementPhase = 0; 
    private Camera mainCamera;
    private float originalMoveSpeedY; 

    private readonly float[] stopPositions = { 5.5f, 16.5f, 26.0f, 36.0f, 45.0f }; 
    private readonly float[] fadeOutPositions = { 11.0f, 21.0f, 32.0f, 42.0f }; 

    void Start()
    {
        startPosition = transform.position;
        mainCamera = Camera.main;
        originalMoveSpeedY = moveSpeedY; 

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0); 
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

        switch (movementPhase)
        {
            case 0:
                MoveRight();
                break;
            default:
                MoveUpAndPause(movementPhase - 1);
                break;
        }
    }

    void MoveRight()
    {
        float newX = Mathf.MoveTowards(transform.position.x, startPosition.x + moveRightDistance, moveSpeedX * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        if (transform.position.x >= startPosition.x + moveRightDistance)
        {
            StartCoroutine(SmoothZoomOut(2.1f, zoomOutDuration));
            movementPhase++;
        }
    }

    void MoveUpAndPause(int phase)
    {
        if (phase < stopPositions.Length)
        {
            float targetY = stopPositions[phase];
            float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeedY * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            if (transform.position.y >= targetY)
            {
                if (targetY == 45.0f)
                {
                    StartCoroutine(SmoothZoomInAndFixate());
                }
                else
                {
                    StartCoroutine(PauseAndFade(phase));
                }
                movementPhase++;
            }
        }
        else
        {
            stopMovement = true;
        }
    }

    IEnumerator PauseAndFade(int phase)
    {
        stopMovement = true;

        yield return StartCoroutine(FadeIn());

        moveSpeedY = increasedMoveSpeedY;

        float initialY = transform.position.y;
        float targetY = initialY + moveUpOffset;
        while (transform.position.y < targetY)
        {
            transform.position += new Vector3(0, moveSpeedY * Time.deltaTime, 0);
            yield return null;
        }

        if (phase < fadeOutPositions.Length)
        {
            float fadeOutTargetY = fadeOutPositions[phase];
            while (transform.position.y < fadeOutTargetY)
            {
                transform.position += new Vector3(0, moveSpeedY * Time.deltaTime, 0);
                yield return null;
            }
        }

        yield return StartCoroutine(FadeOut());

        moveSpeedY = originalMoveSpeedY;

        if (transform.position.y >= 14.0f)
        {
            StartCoroutine(SmoothZoomOut(2.5f, zoomOutDuration)); 
        }
        else if (transform.position.y >= 7.0f)
        {
            StartCoroutine(SmoothZoomOut(2.2f, zoomOutDuration)); 
        }

        stopMovement = false;
    }

    IEnumerator SmoothZoomInAndFixate()
    {
        float targetSize = 1.5f; 
        float duration = 5.0f; 
        float initialSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= duration / 2 && elapsedTime < duration / 2 + 0.5f)
            {
                StartCoroutine(FlashOrderInLayer(firstFlashObject));
            }
            if (elapsedTime >= 2 * duration / 2 && elapsedTime < 2 * duration / 2 + 0.3f)
            {
                StartCoroutine(FlashOrderInLayer(secondFlashObject));
            }

            yield return null;
        }

        mainCamera.orthographicSize = targetSize;
        stopMovement = true; 
    }

    IEnumerator FlashOrderInLayer(GameObject flashObject)
    {
        SpriteRenderer renderer = flashObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 1; 
            yield return new WaitForSeconds(0.1f); 
            renderer.sortingOrder = 0;
        }
    }

    IEnumerator SmoothZoomOut(float targetSize, float duration)
    {
        float initialSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = targetSize;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
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

        fadeImage.color = new Color(0, 0, 0, 0);

        if (secondObject != null)
        {
            secondObject.SetActive(true);
        }
    }
}
