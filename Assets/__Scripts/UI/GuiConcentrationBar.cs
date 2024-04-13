using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiConcentrationBar : MonoBehaviour
{
    StatsManager target;
    GameObject concentartionLine;
    SpriteRenderer sRend;
    SpriteRenderer CLsRend;
    
    public void Init(StatsManager statsManager)
    {
        target = statsManager.GetComponent<StatsManager>();
        concentartionLine = transform.GetChild(0).gameObject;
        sRend = GetComponent<SpriteRenderer>();
        CLsRend = concentartionLine.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (target is null)
            return;

        if (target.IsDead)
            return;

        if (target.Concentration <= 0)
        {
            sRend.color = Color.Lerp(sRend.color, new Color(0, 0, 0, 0), Time.deltaTime);
            concentartionLine.transform.localScale = Vector3.zero;
        }
        else
        {
            sRend.color = Color.black;
            concentartionLine.transform.localScale = Vector3.Lerp(concentartionLine.transform.localScale, new Vector3(target.Concentration / target.MaxConcentration, 1, 1), 10*Time.deltaTime);
            CLsRend.color = new Color(0.8f, 1 - target.Concentration / target.MaxConcentration, 1 - target.Concentration / target.MaxConcentration);
        }
    }
}
