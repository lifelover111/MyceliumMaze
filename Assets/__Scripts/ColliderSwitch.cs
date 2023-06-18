using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSwitch : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("SwitchOff", 0.5f);
    }
    void SwitchOff()
    {
        gameObject.SetActive(false);
    }
}
