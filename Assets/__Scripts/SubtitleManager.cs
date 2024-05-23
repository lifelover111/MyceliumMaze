using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleTMP; 
    public List<string> subtitles;
    public float displayTime = 3.0f; // Время на субтитр

    private int currentSubtitleIndex = 0;

    void Start()
    {
        if (subtitleTMP != null && subtitles.Count > 0)
        {
            StartCoroutine(DisplaySubtitles());
        }
        else
        {
            Debug.LogError("Subtitle TMP or subtitle list not assigned.");
        }
    }

    IEnumerator DisplaySubtitles()
    {
        while (currentSubtitleIndex < subtitles.Count)
        {
            subtitleTMP.text = subtitles[currentSubtitleIndex];
            currentSubtitleIndex++;
            yield return new WaitForSeconds(displayTime);
        }

        subtitleTMP.text = ""; 
    }
}
