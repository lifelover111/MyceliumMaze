using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiPanel : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Hero hero;
    GameObject healthBar;
    GameObject sanityBar;
    float emptyHealthY = -3.5f;
    float emptySanityY = -3.5f;
    float maxHealth;
    float maxSanity;
    Text soulsCount;
    Text itemCount;
    static bool needUpdateInfo;

    void Start () {
        Transform healthPanel = transform.Find("HP_Bar");
        healthBar = healthPanel.Find("Bar_Health").gameObject;
        sanityBar = healthPanel.Find("Bar_Sanity").gameObject;
        maxHealth = hero.maxHealth;
        maxSanity = hero.maxSanity;
        soulsCount = healthPanel.Find("Souls_Count").gameObject.GetComponent<Text>();
        itemCount = healthPanel.Find("Item").Find("Item_Count").gameObject.GetComponent<Text>();
    }   

    void Update () {
        if (needUpdateInfo)
        {
            maxHealth = hero.maxHealth;
            maxSanity = hero.maxSanity;
            needUpdateInfo = false;
        }
        float health = hero.health;
        float sanity = hero.sanity;
        float healthScale = (maxHealth - health) / maxHealth;
        float sanityScale = (maxSanity - sanity) / maxSanity;
        healthBar.transform.localPosition = Vector3.Lerp(healthBar.transform.localPosition, new Vector3(healthBar.transform.localPosition.x, healthScale*emptyHealthY, healthBar.transform.localPosition.z), 0.05f);
        sanityBar.transform.localPosition = Vector3.Lerp(sanityBar.transform.localPosition, new Vector3(sanityBar.transform.localPosition.x, sanityScale * emptySanityY, sanityBar.transform.localPosition.z), 0.05f);

        int souls = int.Parse(soulsCount.text);
        if (souls < hero.souls)
        {
            souls+= hero.souls*0.99f - souls <= 100 ? 1 : Mathf.RoundToInt((hero.souls - souls)*Time.deltaTime);
            soulsCount.text = souls.ToString();
        }
        else if(souls > hero.souls)
        {
            souls-= souls * 0.99f - hero.souls <= 100 ? 1 : Mathf.RoundToInt((souls - hero.souls) * Time.deltaTime);
            soulsCount.text = souls.ToString();
        }

        itemCount.text = hero.flaskCount.ToString();
    }

    static public void UpdateInfo()
    {
        needUpdateInfo = true;
    }
}