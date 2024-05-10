using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance;

    [Header("�����������")]
    public AudioClip parrySound;
    [Header("���� �����")]
    public AudioClip blockSound;
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
    private void Awake()
    {
        instance = this;
    }
}
