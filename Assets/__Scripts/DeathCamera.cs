using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCamera : MonoBehaviour
{
    static public DeathCamera instance;
    [SerializeField] private GameObject deathScreen;
    private SpriteRenderer sRend;
    private GameObject text;
    private float appearInterpolationParam = 0;
    public float appearDeltaTime = 0.01f;
    private bool isNextSceneLoading = false;
    private void Awake()
    {
        instance = this;
        sRend = deathScreen.GetComponent<SpriteRenderer>();
        text = deathScreen.transform.GetChild(0).gameObject;
    }
   

    public void Load()
    {
        InvokeRepeating("DisplayDeathCam", 0, appearDeltaTime);
    }

    private void DisplayDeathCam()
    {
        sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, Mathf.Lerp(0, 1, appearInterpolationParam));
        appearInterpolationParam += appearDeltaTime;
        if(sRend.color.a == 1)
        {
            Invoke("DisplayText", 0.25f);
        }
    }

    private void DisplayText()
    {
        text.SetActive(true);
        if (!isNextSceneLoading)
        {
            isNextSceneLoading = true;

            StartCoroutine(GameManager.LoadSceneAfterTime("Scene_0", 4f));
        }
    }
}
