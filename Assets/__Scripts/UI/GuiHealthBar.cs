using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiHealthBar : MonoBehaviour
{
    StatsManager enemy;
    GameObject healthLine;
    SpriteRenderer sRend;
    SpriteRenderer hsRend;
    float memorizedHealth;
    float memorizeTime;
    public void Init(StatsManager statsManager)
    {
        enemy = statsManager;
        healthLine = transform.GetChild(0).gameObject;
        sRend = GetComponent<SpriteRenderer>();
        hsRend = healthLine.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (enemy is null)
            return;

        if (Time.time - memorizeTime >= 5)
        {
            sRend.color = Color.Lerp(sRend.color, new Color(sRend.color.r, sRend.color.g, sRend.color.b, 0), Time.deltaTime);
            hsRend.color = Color.Lerp(hsRend.color, new Color(hsRend.color.r, hsRend.color.g, hsRend.color.b, 0), Time.deltaTime);
        }
        else
        {
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, 1);
            hsRend.color = new Color(hsRend.color.r, hsRend.color.g, hsRend.color.b, 1);
        }
        if(memorizedHealth != enemy.Health)
        {
            memorizedHealth = enemy.Health;
            memorizeTime = Time.time;
        }

        healthLine.transform.localScale = Vector3.Lerp(healthLine.transform.localScale, new Vector3(enemy.Health / enemy.MaxHealth, 1, 1), 10 * Time.deltaTime);
        healthLine.transform.localPosition = Vector3.Lerp(healthLine.transform.localPosition, new Vector3(-((enemy.MaxHealth - enemy.Health) / enemy.MaxHealth)/2, healthLine.transform.localPosition.y, healthLine.transform.localPosition.z), 10 * Time.deltaTime);
    }
}
