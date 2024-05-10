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
    [Header("Сундук")]
    public AudioClip openingChestSound;
    [Header("Дэш")]
    public AudioClip dashSound;
    [Header("Поднятие предмета")]
    public AudioClip itemPickup;
    private void Awake()
    {
        instance = this;
    }
}
