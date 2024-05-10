using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossPhase : ScriptableObject
{
    public abstract void InitializePhaseListeners(BossCharacterManager boss);

    protected abstract void StartPhase(BossCharacterManager boss);

}
