using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;
    private bool musicEnabled = true;

    [SerializeField] private AudioClip bossTheme;
    [SerializeField] private List<AudioClip> musicClips;


    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += ResetMusic;
        DontDestroyOnLoad(this);
    }

    private void ResetMusic(Scene arg0, LoadSceneMode arg1)
    {
        if(audioSource.clip == bossTheme)
            audioSource.Stop();
    }

    private void Update()
    {
        if(!musicEnabled)
        {
            audioSource.Stop();
            return;
        }
        if(!audioSource.isPlaying)
        {
            audioSource.clip = musicClips[Random.Range(0, musicClips.Count)];
            audioSource.Play();
        }
    }

    public void EnableBossTheme()
    {
        StartCoroutine(BossThemeCoroutine());
    }
    public void DisableBossTheme()
    {
        audioSource.loop = false;
        StartCoroutine(DisableCoroutine());
    }

    public void DisableMusic()
    {
        musicEnabled = false;
    }
    public void EnableMusic()
    {
        musicEnabled = true;
    }

    private IEnumerator BossThemeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        float vol = audioSource.volume;
        float time = Time.time;
        while(Time.time - time <= 1)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        audioSource.Stop();
        audioSource.clip = bossTheme;
        audioSource.loop = true;
        audioSource.Play();
        time = Time.time;
        while (Time.time - time <= 1)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, vol, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator DisableCoroutine()
    {
        float time = Time.time;
        while (Time.time - time <= 1)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        audioSource.Stop();
    }
}
