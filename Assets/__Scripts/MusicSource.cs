using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    public static MusicSource instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
