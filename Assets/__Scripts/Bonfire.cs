using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    float radius = 2f;
    List<bool> isRest = new List<bool>();
    List<Hero> heros = new List<Hero>();
    [SerializeField] private LayerMask layer;
    InRoom inRoom;
    Animator anim;
    AudioSource audioSource;
    bool isPlayingAudio = false;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        inRoom = GetComponent<InRoom>();
        InvokeRepeating("FindPlayers", 0, 0.5f);
        audioSource.clip = SoundBank.instance.Bonfire;
    }

    private void Update()
    {
        if (inRoom.roomNum != HeroKeeper.instance.heroList[0].GetInRoom.roomNum)
        {
            audioSource.Pause();
            isPlayingAudio = false;
            return;
        }
        else
        {
            if (!isPlayingAudio)
            {
                audioSource.Play();
                isPlayingAudio = true;
            }
        }
        if (heros?.Count > 0)
        {
            anim.CrossFade("BonfireSelected", 0);
            anim.speed = 1;
            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (Hero hero in heros)
                {
                    hero.StartRest();
                    isRest[heros.IndexOf(hero)] = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                foreach (Hero hero in heros)
                {
                    if (isRest[heros.IndexOf(hero)])
                    {
                        hero.EndRest();
                        isRest[heros.IndexOf(hero)] = false;
                    }
                }
            }
        }
        else
        {
            anim.CrossFade("BonfireDefault", 0);
            anim.speed = 1;
        }
    }

    void FindPlayers()
    {
        if (inRoom.roomNum != TileCamera.heroInrm.roomNum)
            return;

        Collider[] colls = Physics.OverlapSphere(transform.position, radius, layer);
        if (colls.Length > 0)
            foreach (Collider coll in colls)
            {
                heros.Add(coll.gameObject.GetComponent<Hero>());
                isRest.Add(false);
            }
        else
        {
            heros.Clear();
        }
    }
}
