using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance;

    [Header("�����������")]
    public AudioClip parrySound;
    [Header("���� �����")]
    public AudioClip[] blockSounds;
    [Header("��������� �����")]
    public AudioClip takeDamageSound;
    [Header("���� ��")]
    public AudioClip[] playerStepSounds;
    [Header("������")]
    public AudioClip openingChestSound;
    [Header("���")]
    public AudioClip dashSound;
    [Header("�������� ��������")]
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
