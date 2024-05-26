using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance;

    [Header("Парирование")]
    public AudioClip parrySound;
    [Header("Блок удара")]
    public AudioClip[] blockSounds;
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
    [Header("Sword swing")]
    public AudioClip[] swordSwingSounds;
    [Header("Bow shoot")]
    public AudioClip bowShootSound;
    [Header("Spawn enemy")]
    public AudioClip spawnEnemySound;
    [Header("Door close/open")]
    public AudioClip doorSound;

    private void Awake()
    {
        instance = this;
    }
}
