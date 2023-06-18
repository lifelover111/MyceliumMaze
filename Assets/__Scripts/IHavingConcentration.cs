using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHavingConcentration
{
    float GetConcentration();
    float GetMaxConcentration();
    void IncreaseConcentration(float val);
}
