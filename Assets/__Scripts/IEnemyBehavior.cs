using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehavior
{
    void Update();
    void PrepareBehavior();
    float Analyze();
    void Stop();
}