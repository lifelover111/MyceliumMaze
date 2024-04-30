using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundManager : MonoBehaviour
{
    public static WorldSoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }
}
