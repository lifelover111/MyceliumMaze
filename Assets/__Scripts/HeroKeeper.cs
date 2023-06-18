using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKeeper : MonoBehaviour
{
    public static HeroKeeper instance;
    [SerializeField] public List<Hero> heroList = new List<Hero>();
    void Awake()
    {
        instance = this;
    }
}
