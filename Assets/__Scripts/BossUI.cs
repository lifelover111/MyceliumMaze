using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public static BossUI instance;

    [SerializeField] private Transform HPBar;
    [SerializeField] private Transform ConcentrationBar;
    private Image concentrationLineImg;
    private GameObject healthLine;
    private GameObject concentrationLine;
    [SerializeField] private TMP_Text bossName;

    private BossCharacterManager boss;

    public void SetBoss(BossCharacterManager boss)
    {
        this.boss = boss;
        
        bossName.text = boss.Name;
        healthLine.transform.localScale = new Vector3(boss.statsManager.Health / boss.statsManager.MaxHealth, 1, 1);
        concentrationLine.transform.localScale = new Vector3(boss.statsManager.Concentration / boss.statsManager.MaxConcentration, 1, 1);
        concentrationLineImg.color = new Color(0.8f, 1 - boss.statsManager.Concentration / boss.statsManager.MaxConcentration, 1 - boss.statsManager.Concentration / boss.statsManager.MaxConcentration);

        gameObject.SetActive(true);
    }

    private void Awake()
    {
        instance = this;

        healthLine = HPBar.GetChild(0).gameObject;
        concentrationLine = ConcentrationBar.GetChild(0).gameObject;
        concentrationLineImg = concentrationLine.GetComponent<Image>();

        gameObject.SetActive(false);
    }

    void Update()
    {
        healthLine.transform.localScale = Vector3.Lerp(healthLine.transform.localScale, new Vector3(boss.statsManager.Health / boss.statsManager.MaxHealth, 1, 1), 10 * Time.deltaTime);
        
        concentrationLine.transform.localScale = Vector3.Lerp(concentrationLine.transform.localScale, new Vector3(boss.statsManager.Concentration / boss.statsManager.MaxConcentration, 1, 1), 10 * Time.deltaTime);
        concentrationLineImg.color = new Color(0.8f, 1 - boss.statsManager.Concentration / boss.statsManager.MaxConcentration, 1 - boss.statsManager.Concentration / boss.statsManager.MaxConcentration);

    }
}
