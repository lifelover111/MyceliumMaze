using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public Dictionary<string, System.Action> levelUpDICT;

    protected int _level = 1;
    protected int _vitality = 2;
    protected int _endurance = 2;
    protected int _strength = 2;
    protected int _dexterity = 2;
    protected int soulsRequired = 200;
    protected int priceIncrement = 50;
    public bool isChanged = false;

    public Stats()
    {
        _level = 1;
        levelUpDICT = new Dictionary<string, System.Action>() {
            {"Vit", () => LevelUp("Vit") },
            {"End", () => LevelUp("End") },
            {"Str", () => LevelUp("Str") },
            {"Dex", () => LevelUp("Dex") },
        };
    }

    public Stats(int vit, int end, int str, int dex)
    {
        _vitality = vit;
        _endurance = end;
        _strength = str;
        _dexterity = dex;
        _level = (_vitality + _endurance + _strength + _dexterity) - 7;
        levelUpDICT = new Dictionary<string, System.Action>() {
            {"Vit", () => LevelUp("Vit") },
            {"End", () => LevelUp("End") },
            {"Str", () => LevelUp("Str") },
            {"Dex", () => LevelUp("Dex") },
        };
    }


    public int Level { get { return _level; } }
    public int Vitality { get { return _vitality; } }
    public int Endurance { get { return _endurance; } }
    public int Strength { get { return _strength; } }
    public int Dexterity { get { return _dexterity; } }

    protected virtual void LevelUp(string stat)
    {
        switch (stat)
        {
            case "Vit":
                _vitality++;
                break;
            case "End":
                _endurance++;
                break;
            case "Str":
                _strength++;
                break;
            case "Dex":
                _dexterity++;
                break;
            default:
                return;
        }
        isChanged = true;
        _level++;
        soulsRequired += priceIncrement;
        
    }
}



public class HeroStats : Stats 
{
    private int _will = 2;
    public int Will { get { return _will; } }
    public HeroStats() : base()
    {
        _level += _will;
        levelUpDICT.Add("Wil", () => LevelUp("Wil"));
    }
    public HeroStats(int vit, int end, int str, int dex, int wil) : base(vit, end, str, dex)
    {
        _will = wil;
        _level = 1;
        levelUpDICT.Add("Wil", () => LevelUp("Wil"));
    }
    protected override void LevelUp(string stat)
    {
        switch (stat)
        {
            case "Vit":
                _vitality++;
                break;
            case "End":
                _endurance++;
                break;
            case "Str":
                _strength++;
                break;
            case "Dex":
                _dexterity++;
                break;
            case "Wil":
                _will++;
                break;
            default:
                return;
        }
        
        isChanged = true;
        _level++;
        soulsRequired += priceIncrement;
    }

    public int GetSoulsRequired()
    {
        return soulsRequired;
    }
    public int GetPriceIncrement()
    {
        return priceIncrement;
    }
}
