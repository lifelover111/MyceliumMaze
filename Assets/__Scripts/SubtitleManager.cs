//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;

//public class SubtitleManager : MonoBehaviour
//{
//    public TextMeshProUGUI subtitleTMP;
//    public Image subtitleBackground; // �������� Image ��� ����
//    public List<string> subtitles;
//    public float displayTime = 3.0f; // ����� �� �������
//    public Color backgroundColor = new Color(0, 0, 0, 0.95f); // ������ ���� � 50% �������������

//    private int currentSubtitleIndex = 0;

//    void Start()
//    {
//        if (subtitleTMP != null && subtitleBackground != null && subtitles.Count > 0)
//        {
//            subtitleBackground.color = backgroundColor; // ������������� ���� ���� ��� ������
//            StartCoroutine(DisplaySubtitles());
//        }
//        else
//        {
//            Debug.LogError("Subtitle TMP, subtitle background, or subtitle list not assigned.");
//        }
//    }

//    IEnumerator DisplaySubtitles()
//    {
//        while (currentSubtitleIndex < subtitles.Count)
//        {
//            subtitleTMP.text = subtitles[currentSubtitleIndex];
//            subtitleBackground.gameObject.SetActive(true); // ���������� ���
//            currentSubtitleIndex++;
//            yield return new WaitForSeconds(displayTime);
//            subtitleTMP.text = "";
//            subtitleBackground.gameObject.SetActive(false); // �������� ���
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    [System.Serializable]
    public struct SubtitleInfo
    {
        public string subtitleText;
        public float displayTime;
    }

    public TextMeshProUGUI subtitleTMP;
    public Image subtitleBackground;
    public float typingSpeed = 0.05f; // �������� ��������� ������
    public List<SubtitleInfo> subtitlesInfo;

    private int currentSubtitleIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (subtitleTMP != null && subtitleBackground != null && subtitlesInfo.Count > 0)
        {
            StartCoroutine(DisplaySubtitles());
        }
        else
        {
            Debug.LogError("Subtitle TMP, subtitle background, or subtitle list not assigned.");
        }
    }

    IEnumerator DisplaySubtitles()
    {
        while (currentSubtitleIndex < subtitlesInfo.Count)
        {
            string subtitleText = subtitlesInfo[currentSubtitleIndex].subtitleText;
            float displayTime = subtitlesInfo[currentSubtitleIndex].displayTime;

            // �������� �������� ��������� ������
            typingCoroutine = StartCoroutine(TypeText(subtitleText));

            // ���� ����� ����������� ��������
            yield return new WaitForSeconds(displayTime);

            // ������������� �������� ��������� � ������� �����
            StopCoroutine(typingCoroutine);
            subtitleTMP.text = "";

            // ��������� � ���������� ��������
            currentSubtitleIndex++;
        }
    }

    IEnumerator TypeText(string text)
    {
        subtitleTMP.text = "";
        foreach (char letter in text.ToCharArray())
        {
            subtitleTMP.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
