using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField] public List<Player> playerList = new List<Player>();
    void Awake()
    {
        instance = this;
    }
}
