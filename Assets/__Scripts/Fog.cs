using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    Animator anim;
    string rotation;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        rotation = transform.GetComponentInParent<Door>().rotation;
    }

    public void CreateFog()
    {
        anim.enabled = true;
        anim.CrossFade("Fog_" + rotation + "_Appearance", 0);
        anim.speed = 1;
    }

    public void ClearFog()
    {
        if(!anim.enabled)
            return;
        anim.CrossFade("Fog_" + rotation + "_Fade", 0);
        anim.speed = 1;
    }

    void FogStay()
    {
        anim.CrossFade("Fog_" + rotation + "_Stay", 0);
    }

    void FogFade()
    {
        anim.enabled = false;
        gameObject.SetActive(false);
    }

}
