using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersInGameManager : MonoBehaviour
{
    public static PlayersInGameManager instance;
    [SerializeField] GameObject playerPrefab;

    public List<PlayerManager> playerList = new List<PlayerManager>();
    void Awake()
    {
        if (instance is null)
        {
            instance = this;
            var player = Instantiate(playerPrefab);
            player.transform.position = Vector3.zero;
            playerList.Add(player.GetComponent<PlayerManager>());
            playerList.First().OnDead += () => instance = null;
        } 
    }
}
