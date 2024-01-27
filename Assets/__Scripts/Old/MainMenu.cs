using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject seedForm;
    [SerializeField] GameObject screenShadow;
    [SerializeField] Texture2D cursor;
    private SpriteRenderer sSRend;
    private Text seedText;
    private string seed;
    private float fadeDeltaTime = 0.005f;
    private float fadeInterpolationParam = 0;
    private float targetTransparancy = 0;
    private bool startPressed = false;
    private AudioSource audioSource;


    private void Awake()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        seedText = seedForm.GetComponent<Text>();
        Cursor.visible = true;
        sSRend = screenShadow.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        seed = seedText.text;
        seed = seed == "" ? null : seed;
        if(sSRend.color.a != targetTransparancy)
        {
            FadeScreenShadow();
            return;
        }
        if (startPressed)
        {
            SceneManager.LoadScene("Scene_1");
        }
    }

    private void FadeScreenShadow()
    {
        sSRend.color = new Color(sSRend.color.r, sSRend.color.g, sSRend.color.b, Mathf.Lerp(1 - targetTransparancy, targetTransparancy, fadeInterpolationParam));
        fadeInterpolationParam += fadeDeltaTime;
    }

    public void StartGame()
    {
        targetTransparancy = 1;
        fadeInterpolationParam = 0;
        startPressed = true;
        Cursor.visible = false;
        audioSource.Play();
    }

    public void ExitGame()
    {
        audioSource.Play();
        Application.Quit();
    }



}
