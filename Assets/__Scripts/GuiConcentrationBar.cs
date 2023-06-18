using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiConcentrationBar : MonoBehaviour
{
    IHavingConcentration target;
    GameObject concentartionLine;
    SpriteRenderer sRend;
    SpriteRenderer CLsRend;
    void Start()
    {
        target = transform.parent.GetComponent<IHavingConcentration>();
        concentartionLine = transform.GetChild(0).gameObject;
        sRend = GetComponent<SpriteRenderer>();
        CLsRend = concentartionLine.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (target.GetConcentration() <= 0)
        {
            sRend.color = Color.Lerp(sRend.color, new Color(0, 0, 0, 0), Time.deltaTime);
            concentartionLine.transform.localScale = Vector3.zero;
        }
        else
        {
            sRend.color = Color.black;
            concentartionLine.transform.localScale = Vector3.Lerp(concentartionLine.transform.localScale, new Vector3(target.GetConcentration() / target.GetMaxConcentration(), 1, 1), 10*Time.deltaTime);
            CLsRend.color = new Color(0.8f, 1 - target.GetConcentration() / target.GetMaxConcentration(), 1 - target.GetConcentration() / target.GetMaxConcentration());
        }
    }
}
