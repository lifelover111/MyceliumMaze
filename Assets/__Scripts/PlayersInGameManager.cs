using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersInGameManager : MonoBehaviour
{
    public static PlayersInGameManager instance;
    [SerializeField] public List<PlayerManager> playerList = new List<PlayerManager>();
    void Awake()
    {
        instance = this;
    }
}
