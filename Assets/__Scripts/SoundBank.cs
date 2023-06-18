using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance;
    [SerializeField] public AudioClip Start_Rest;
    [SerializeField] public AudioClip Click_1;
    [SerializeField] public AudioClip Click_2;
    [SerializeField] public AudioClip Click_LevelUp;

    [SerializeField] public AudioClip Block;
    [SerializeField] public AudioClip Bonfire;
    [SerializeField] public AudioClip Damage_Hero;
    [SerializeField] public AudioClip Damage_Necromancer;
    [SerializeField] public AudioClip Damage_Skeleton;
    [SerializeField] public AudioClip Footstep_Hero_0;
    [SerializeField] public AudioClip Footstep_Hero_1;
    [SerializeField] public AudioClip Footstep;
    [SerializeField] public AudioClip Heal;
    [SerializeField] public AudioClip Hit_Hero;
    [SerializeField] public AudioClip Hit_Skeleton;
    [SerializeField] public AudioClip Hit_SkeletonBow;
    [SerializeField] public AudioClip Die_Necromancer;
    [SerializeField] public AudioClip Die_Skeleton;
    [SerializeField] public AudioClip Shot_Necromancer;
    [SerializeField] public AudioClip Shot_Bow;
    [SerializeField] public AudioClip Parry;
    [SerializeField] public AudioClip Death;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
