using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance;

    [Header("Парирование")]
    public AudioClip parrySound;
    [Header("Блок удара")]
    public AudioClip blockSound;
    [Header("Получение урона")]
    public AudioClip takeDamageSound;
    [Header("Шаги ГГ")]
    public AudioClip[] playerStepSounds;

    private void Awake()
    {
        instance = this;
    }
}
