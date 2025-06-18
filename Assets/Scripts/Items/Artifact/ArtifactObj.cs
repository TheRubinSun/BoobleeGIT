using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ArtifactObj
{
    public int ID_Art { get; set; }
    public int art_level { get; set; }
    public int chars_level {  get; set; }
    public int curse_level { get; set; }
    public int costMultiply { get; set; }


    public int Artif_Strength { get; set; }
    public int Artif_Agility { get; set; }
    public int Artif_Intelligence { get; set; }
    public int Artif_Hp { get; set; }
    public int Artif_Mana { get; set; }
    public int Artif_Armor { get; set; }
    public int Artif_Evasion { get; set; }
    public float Artif_Mov_Speed { get; set; }
    public float Artif_Att_Range { get; set; }
    public int Artif_Att_Speed { get; set; }
    public int Artif_Proj_Speed { get; set; }
    public float Artif_ExpBust { get; set; }
    public float Artif_Mage_Resis { get; set; }
    public float Artif_Tech_Resis { get; set; }
    public int Artif_Damage { get; set; }


    private int statCount;
    public bool StatsGen { get; set; }
    private HashSet<StatType> statTypes = new HashSet<StatType>();
    public ArtifactObj(int id,  int _art_level)
    {
        art_level = _art_level;
        ID_Art = id;

        SetRandomAttributes();
    }
    public ArtifactObj(int id, int _art_level, System.Random random)
    {
        art_level = _art_level;
        ID_Art = id;

        SetRandomAttributes(random);
    }
    public ArtifactObj(int id)
    {
        ID_Art = id;
    }
    public ArtifactObj() { }
    public void SetAttributes()
    {

    }
    public void SetRandomAttributes(System.Random random = null)
    {
        statCount = System.Enum.GetValues(typeof(StatType)).Length;

        int precentNewCharm = 0;
        int levelCharm = 0;
        CheckLevel(out precentNewCharm, out levelCharm, random);

        while (true)
        {
            if(random != null)
            {
                if (random.Next(0, 101) <= precentNewCharm)
                {
                    GetStat(levelCharm, random);
                }
                else break;
            }
            else
            {
                if (Random.Range(0, 101) <= precentNewCharm)
                {
                    GetStat(levelCharm, random);
                }
                else break;
            }
        }
        StatsGen = true;
    }
    private void CheckLevel(out int precent, out int levelCharm, System.Random random = null)
    {
        int baseChance = 50;

        // –ассчитываем шанс
        int minLevel = 1;
        int maxLevel = art_level + 1;
        
        if(random != null)
            levelCharm = random.Next(minLevel, maxLevel + 1);
        else
            levelCharm = Random.Range(minLevel, maxLevel + 1);
        //levelCharm = Mathf.Max(1, Random.Range(art_level - 1, art_level + 1));

        precent = baseChance + art_level * 6; // ”величение шанса с каждым уровнем на 7%
        // ƒл€ уровн€ выше 5 можно задать максимальный шанс
        if (precent > 85)
        {
            precent = 85; // ќграничиваем шанс максимальным значением, например, 80%
        }
    }
    private void GetStat(int levelCharm, System.Random random = null)
    {
        StatType stat;

        if (random != null) 
            stat = (StatType)random.Next(0, statCount);
        else 
            stat = (StatType)Random.Range(0, statCount);

        switch (stat)
        {
            case StatType.Strength:
                Artif_Strength += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.STRENGTH, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_STRENGTH, random);
                break;
            case StatType.Agility:
                Artif_Agility += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.AGILITY, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_AGILITY, random);
                break;
            case StatType.Intelligence:
                Artif_Intelligence += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.INTELLIGENCE, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_INTELLIGENCE, random);
                break;
            case StatType.Hp:
                Artif_Hp += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.HP, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_HP, random);
                break;
            case StatType.Armor:
                Artif_Armor += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ARMOR, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_ARMOR, random);
                break;
            case StatType.Evasion:
                Artif_Evasion += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.EVASION, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_EVASION, random);
                break;
            case StatType.Mov_Speed:
                Artif_Mov_Speed += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MOV_SPEED, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MOV_SPEED, random);
                break;
            case StatType.Att_Range:
                Artif_Att_Range += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ATT_RANGE, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_ATT_RANGE, random);
                break;
            case StatType.Att_Speed:
                Artif_Att_Speed += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ATT_SPEED, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_ATT_SPEED, random);
                break;
            case StatType.Proj_Speed:
                Artif_Proj_Speed += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.PROJ_SPEED, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_PROJ_SPEED, random);
                break;
            case StatType.ExpBust:
                Artif_ExpBust += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.EXPBUST, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_EXPBUST, random);
                break;
            case StatType.Mage_Resis:
                Artif_Mage_Resis += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MAGE_RESIS, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MAGE_RESIS, random);
                break;
            case StatType.Tech_Resis:
                Artif_Tech_Resis += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.TECH_RESIS, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_TECH_RESIS, random);
                break;
            case StatType.Damage:
                Artif_Damage += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.DAMAGE, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_DAMAGE, random);
                break;
            case StatType.Mana:
                Artif_Mana += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MANA, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MANA, random);
                break;
        }

    }
    private float GetValueStat(int levelCharm, float baseValue, float addForChar, System.Random random = null)
    {
        float positive, negative, randValue;

        //float result = Random.value < 0.35 ? negative : positive;
        if(random != null)
        {
            positive = (float)(random.NextDouble() * ((addForChar * levelCharm))) + baseValue;

            float negMin = -addForChar * levelCharm;
            float negMax = -baseValue;
            float negRange = negMax - negMin;

            negative = Mathf.Min(-0.01f, (float)(random.NextDouble() * negRange + negMin));
            randValue = (float)random.NextDouble(); // от 0 до 1
        }
        else
        {
            positive = Random.Range(baseValue, (addForChar * levelCharm) + baseValue);
            negative = Mathf.Min(-0.01f, Random.Range(-addForChar * levelCharm, -baseValue));
            randValue = Random.value;
        }

        float result;
        if (randValue < 0.35)
        {
            result = negative;
            curse_level += levelCharm;
        }
        else
        {
            result = positive;
            chars_level += levelCharm;
        }

        return result;
    }
    public bool isAllNull()
    {
        return Artif_Strength == 0 &&
       Artif_Agility == 0 &&
       Artif_Intelligence == 0 &&
       Artif_Hp == 0 &&
       Artif_Armor == 0 &&
       Artif_Evasion == 0 &&
       Artif_Mov_Speed == 0f &&
       Artif_Att_Range == 0f &&
       Artif_Att_Speed == 0 &&
       Artif_Proj_Speed == 0 &&
       Artif_ExpBust == 0f &&
       Artif_Mage_Resis == 0f &&
       Artif_Tech_Resis == 0f &&
       Artif_Damage == 0f &&
       Artif_Mana == 0f;
    }
}
public enum StatType
{
    Strength,
    Agility,
    Intelligence,
    Hp,
    Armor,
    Evasion,
    Mov_Speed,
    Att_Range,
    Att_Speed,
    Proj_Speed,
    ExpBust,
    Mage_Resis,
    Tech_Resis,
    Damage,
    Mana
}