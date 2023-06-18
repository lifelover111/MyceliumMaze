using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonfireMenu : MonoBehaviour
{
    [SerializeField] Hero hero;
    Transform background;
    Transform rootMenu;
    Transform levelUpMenu;
    Transform restorationMenu;
    HeroStats stats;
    LevelUpData levelUpData;

    Transform levelData;
    Transform priceData;
    Transform vitData;
    Transform endData;
    Transform strData;
    Transform dexData;
    Transform wilData;

    Text vitValue;
    Text endValue;
    Text strValue;
    Text dexValue;
    Text wilValue;


    Transform vitInc;
    Transform vitDec;
    Transform endInc;
    Transform endDec;
    Transform strInc;
    Transform strDec;
    Transform dexInc;
    Transform dexDec;
    Transform wilInc;
    Transform wilDec;

    int levelsDelta = 0;
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        background = transform.GetChild(0);
        rootMenu = transform.GetChild(1);
        levelUpMenu = transform.GetChild(2);
        restorationMenu = transform.GetChild(3);
        stats = hero.GetStats();


        levelData = levelUpMenu.GetChild(0);
        priceData = levelUpMenu.GetChild(1);
        vitData = levelUpMenu.GetChild(2);
        endData = levelUpMenu.GetChild(3);
        strData = levelUpMenu.GetChild(4);
        dexData = levelUpMenu.GetChild(5);
        wilData = levelUpMenu.GetChild(6);


        vitValue = vitData.Find("Value").gameObject.GetComponent<Text>();
        endValue = endData.Find("Value").gameObject.GetComponent<Text>();
        strValue = strData.Find("Value").gameObject.GetComponent<Text>();
        dexValue = dexData.Find("Value").gameObject.GetComponent<Text>();
        wilValue = wilData.Find("Value").gameObject.GetComponent<Text>();

        vitInc = vitData.GetChild(0);
        vitDec = vitData.GetChild(1);
        endInc = endData.GetChild(0);
        endDec = endData.GetChild(1);
        strInc = strData.GetChild(0);
        strDec = strData.GetChild(1);
        dexInc = dexData.GetChild(0);
        dexDec = dexData.GetChild(1);
        wilInc = wilData.GetChild(0);
        wilDec = wilData.GetChild(1);
    }

    private void Update()
    {
        if (levelUpData == null)
            return;
        if(levelUpData.baseSouls < levelUpData.souls + stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement())
        {
            vitInc.gameObject.SetActive(false);
            endInc.gameObject.SetActive(false);
            strInc.gameObject.SetActive(false);
            dexInc.gameObject.SetActive(false);
            wilInc.gameObject.SetActive(false);
        }
        else
        {
            vitInc.gameObject.SetActive(true);
            endInc.gameObject.SetActive(true);
            strInc.gameObject.SetActive(true);
            dexInc.gameObject.SetActive(true);
            wilInc.gameObject.SetActive(true);
        }


        if(levelUpData.vit <= levelUpData.baseVit)
            vitDec.gameObject.SetActive(false);
        else
            vitDec.gameObject.SetActive(true);

        if(levelUpData.end <= levelUpData.baseEnd)
            endDec.gameObject.SetActive(false);
        else
            endDec.gameObject.SetActive(true);

        if(levelUpData.str <= levelUpData.baseStr)
            strDec.gameObject.SetActive(false);
        else
            strDec.gameObject.SetActive(true);

        if(levelUpData.dex <= levelUpData.baseDex)
            dexDec.gameObject.SetActive(false);
        else
            dexDec.gameObject.SetActive(true);

        if(levelUpData.wil <= levelUpData.baseWil)
            wilDec.gameObject.SetActive(false);
        else
            wilDec.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        background.gameObject.SetActive(true);
        rootMenu.gameObject.SetActive(true);
        audioSource.clip = SoundBank.instance?.Start_Rest;
        audioSource.Play();
    }

    private void OnDisable()
    {
        background.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(false);
        levelUpMenu.gameObject.SetActive(false);
        restorationMenu.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (levelUpData == null)
            return;

        levelData.gameObject.GetComponent<Text>().text = "Level: " + levelUpData.baseLevel + " -> " + levelUpData.level;
        priceData.GetChild(0).Find("Value").gameObject.GetComponent<Text>().text = (stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement()).ToString();
        priceData.GetChild(1).Find("Value").gameObject.GetComponent<Text>().text = levelUpData.baseSouls.ToString() + " -> " + (levelUpData.baseSouls - levelUpData.souls).ToString();
        vitValue.text = levelUpData.vit.ToString();
        endValue.text = levelUpData.end.ToString();
        strValue.text = levelUpData.str.ToString();
        dexValue.text = levelUpData.dex.ToString();
        wilValue.text = levelUpData.wil.ToString();
    }

    public void Leave()
    {
        audioSource.clip = SoundBank.instance?.Click_1;
        audioSource.Play();
        System.Action leave;
        leave = delegate () { hero.EndRest(); };
        Invoke(leave.Method.Name, 0.4f);
    }
    public void RestoreFlasks()
    {
        audioSource.clip = SoundBank.instance?.Click_1;
        audioSource.Play();
        rootMenu.gameObject.SetActive(false);
        restorationMenu.gameObject.SetActive(true);
        Transform message = restorationMenu.Find("Message");
        string text;
        if(hero.souls >= stats.GetSoulsRequired())
        {
            text = "You will restore "+(hero.maxNumFlasks-hero.numFlasks)+" flasks. \n It will cost "+stats.GetSoulsRequired()+" spores.\n Do you wish to continue?";
            restorationMenu.Find("Ok").gameObject.SetActive(true);
        }
        else
        {
            text = "You don't have enough spores.\n Spores required: " + stats.GetSoulsRequired() + '.';
            restorationMenu.Find("Ok").gameObject.SetActive(false);
        }
        message.gameObject.GetComponent<Text>().text = text;
    }

    public void ConfirmFlasksRestoration()
    {
        audioSource.clip = SoundBank.instance?.Click_LevelUp;
        audioSource.Play();
        hero.souls -= stats.GetSoulsRequired();
        hero.numFlasks = hero.maxNumFlasks;
        restorationMenu.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(true);
    }
    public void CancelFlasksRestoration()
    {
        audioSource.clip = SoundBank.instance?.Click_1;
        audioSource.Play();
        restorationMenu.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(true);
    }

    public void LevelUp()
    {
        audioSource.clip = SoundBank.instance?.Click_1;
        audioSource.Play();
        levelUpData = new LevelUpData(hero, stats);
        rootMenu.gameObject.SetActive(false);
        levelUpMenu.gameObject.SetActive(true);

        levelsDelta = 0;
    }

    public void IncrementVit()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelUpData.souls += stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
        levelUpData.vit += 1;
        levelUpData.level += 1;
        levelsDelta += 1;
    }
    public void DecrementVit()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelsDelta -= 1;
        levelUpData.level -= 1;
        levelUpData.vit -= 1;
        levelUpData.souls -= stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
    }

    public void IncrementEnd()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelUpData.souls += stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
        levelUpData.end += 1;
        levelUpData.level += 1;
        levelsDelta += 1;
    }
    public void DecrementEnd()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelsDelta -= 1;
        levelUpData.level -= 1;
        levelUpData.end -= 1;
        levelUpData.souls -= stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
    }


    public void IncrementStr()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelUpData.souls += stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
        levelUpData.str += 1;
        levelUpData.level += 1;
        levelsDelta += 1;
    }
    public void DecrementStr()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelsDelta -= 1;
        levelUpData.level -= 1;
        levelUpData.str -= 1;
        levelUpData.souls -= stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
    }


    public void IncrementDex()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelUpData.souls += stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
        levelUpData.dex += 1;
        levelUpData.level += 1;
        levelsDelta += 1;
    }
    public void DecrementDex()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelsDelta -= 1;
        levelUpData.level -= 1;
        levelUpData.dex -= 1;
        levelUpData.souls -= stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
    }


    public void IncrementWil()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelUpData.souls += stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
        levelUpData.wil += 1;
        levelUpData.level += 1;
        levelsDelta += 1;
    }
    public void DecrementWil()
    {
        audioSource.clip = SoundBank.instance?.Click_2;
        audioSource.Play();
        levelsDelta -= 1;
        levelUpData.level -= 1;
        levelUpData.wil -= 1;
        levelUpData.souls -= stats.GetSoulsRequired() + levelsDelta * stats.GetPriceIncrement();
    }


    public void CancelLevelingUp()
    {
        audioSource.clip = SoundBank.instance?.Click_1;
        audioSource.Play();
        levelsDelta = 0;
        levelUpMenu.gameObject.SetActive(false);
        rootMenu.gameObject.SetActive(true);
    }

    public void ConfirmLevelingUp()
    {
        audioSource.clip = SoundBank.instance?.Click_LevelUp;
        audioSource.Play();
        while (true)
        {
            if (levelUpData.baseVit < levelUpData.vit)
            {
                stats.levelUpDICT["Vit"].Invoke();
                levelUpData.baseVit++;
            }
            else if (levelUpData.baseEnd < levelUpData.end)
            {
                stats.levelUpDICT["End"].Invoke();
                levelUpData.baseEnd++;
            }
            else if (levelUpData.baseStr < levelUpData.str)
            {
                stats.levelUpDICT["Str"].Invoke();
                levelUpData.baseStr++;
            }
            else if (levelUpData.baseDex < levelUpData.dex)
            {
                stats.levelUpDICT["Dex"].Invoke();
                levelUpData.baseDex++;
            }
            else if (levelUpData.baseWil < levelUpData.wil)
            {
                stats.levelUpDICT["Wil"].Invoke();
                levelUpData.baseWil++;
            }
            else
                break;
        }
        hero.souls -= levelUpData.souls;
        levelUpData = new LevelUpData(hero, stats);
        levelsDelta = 0;
    }


}


public class LevelUpData
{
    public int baseLevel;
    public int level;
    public int baseSouls;
    public int souls;
    public int baseVit;
    public int vit;
    public int baseEnd;
    public int end;
    public int baseStr;
    public int str;
    public int baseDex;
    public int dex;
    public int baseWil;
    public int wil;

    public LevelUpData(Hero hero, HeroStats stats)
    {
        baseLevel = stats.Level;
        level = stats.Level;
        baseSouls = hero.souls;
        souls = 0;
        baseVit = stats.Vitality;
        vit = stats.Vitality;
        baseEnd = stats.Endurance;
        end = stats.Endurance;
        baseStr = stats.Strength;
        str = stats.Strength;
        baseDex = stats.Dexterity;
        dex = stats.Dexterity;
        baseWil = stats.Will;
        wil = stats.Will;
    }
}