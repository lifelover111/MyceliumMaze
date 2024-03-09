using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICharacterCombatManager : CombatManager
{
    public void FindTarget()
    {
        currentTarget = PlayersInGameManager.instance.playerList.FirstOrDefault();
    }
}
