using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ArtifactObj
{
    public int ID_Art { get; set; }
    public int SEED_Art { get; set; }
    public int art_level { get; set; }
    public int chars_level {  get; set; }
    public int curse_level { get; set; }
    public int costMultiply { get; set; }


    public int Artif_Strength { get; set; }
    public int Artif_Agility { get; set; }
    public int Artif_Intelligence { get; set; }
    public int Artif_Hp { get; set; }
    public int Artif_Mana { get; set; }
    public float Artif_ManaRegen { get; set; }
    public int Artif_Armor { get; set; }
    public int Artif_Evasion { get; set; }
    public float Artif_Mov_Speed { get; set; }
    public float Artif_Att_Range { get; set; }
    public int Artif_Att_Speed { get; set; }
    public float Artif_Proj_Speed { get; set; }
    public float Artif_ExpBust { get; set; }
    public float Artif_Mage_Resis { get; set; }
    public float Artif_Tech_Resis { get; set; }
    public int Artif_Damage { get; set; }
    


    private int statCount;
    public bool StatsGen { get; set; }
    private HashSet<AllStats> statTypes = new HashSet<AllStats>();
    public ArtifactObj(int id,  int _art_level) //СОздаем новый артефакт
    {
        art_level = _art_level;
        ID_Art = id;

        SetRandomAttributes();
    }
    public ArtifactObj(int id, int _art_level, System.Random random, int seed) //По сиду
    {
        art_level = _art_level;
        ID_Art = id;
        SEED_Art = seed;

        chars_level = 0;
        curse_level = 0;
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
    public void SetRandomAttributes(System.Random random = null) //Назначаем чары или атрибуты по сиду
    {
        statCount = System.Enum.GetValues(typeof(AllStats)).Length;

        int precentNewCharm = 0;
        int levelCharm = 0;
        bool firstCharm = true;

        GetPrecentForLevelCharm(out precentNewCharm);
        Debug.Log($"precentNewCharm: {precentNewCharm} || artLevel: {art_level}");
        while (true)
        {
            GetLevelCharm(out levelCharm, random);
            int roll = (random != null) ? random.Next(0, 101) : Random.Range(0, 101);

            if(firstCharm || roll <= precentNewCharm)
            {
                GetStat(levelCharm, random);
                firstCharm = false;
            }
            else break;
            precentNewCharm -= Mathf.Max(10 - art_level, 5); //Уменьшение шанаса на доп чар
        }
        StatsGen = true;
    }
    private void GetLevelCharm(out int levelCharm, System.Random random = null) //Рассчитываем уровень чара по уровню артефакта
    {
        int minLevel = 1;
        int maxLevel = art_level + 1;

        if (random != null)
            levelCharm = random.Next(minLevel, maxLevel + 1); //По сиду
        else
            levelCharm = Random.Range(minLevel, maxLevel + 1); // Если сида нет, то просто на рандоме
    }
    private int GetPrecentForLevelCharm(out int precent) // Рассчитываем шанс на получения чара
    {
        int baseChance = 60;
        precent = baseChance + art_level * 6; // Увеличение шанса с каждым уровнем на 6%
        // Для уровня выше 5 можно задать максимальный шанс
        if (precent > 100)
        {
            precent = 100; // Ограничиваем шанс максимальным значением, например, 85%
        }
        return precent;
    }
    private void GetStat(int levelCharm, System.Random random = null)
    {
        AllStats stat;

        if (random != null) 
            stat = (AllStats)random.Next(0, statCount);
        else 
            stat = (AllStats)Random.Range(0, statCount);

        switch (stat)
        {
            case AllStats.Strength:
                Artif_Strength += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.STRENGTH, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_STRENGTH, random);
                break;
            case AllStats.Agility:
                Artif_Agility += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.AGILITY, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_AGILITY, random);
                break;
            case AllStats.Intelligence:
                Artif_Intelligence += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.INTELLIGENCE, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_INTELLIGENCE, random);
                break;
            case AllStats.Max_Hp:
                Artif_Hp += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.HP, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_HP, random);
                break;
            case AllStats.Armor:
                Artif_Armor += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ARMOR, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_ARMOR, random);
                break;
            case AllStats.Evasion:
                Artif_Evasion += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.EVASION, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_EVASION, random);
                break;
            case AllStats.Mov_Speed:
                Artif_Mov_Speed += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MOV_SPEED, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MOV_SPEED, random);
                break;
            case AllStats.Att_Range:
                Artif_Att_Range += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ATT_RANGE, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_ATT_RANGE, random);
                break;
            case AllStats.Att_Speed:
                Artif_Att_Speed += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ATT_SPEED, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_ATT_SPEED, random);
                break;
            case AllStats.Proj_Speed:
                Artif_Proj_Speed += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.PROJ_SPEED, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_PROJ_SPEED, random);
                break;
            case AllStats.ExpBust:
                Artif_ExpBust += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.EXPBUST, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_EXPBUST, random);
                break;
            case AllStats.Magic_Resis:
                Artif_Mage_Resis += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MAGE_RESIS, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MAGE_RESIS, random);
                break;
            case AllStats.Tech_Resis:
                Artif_Tech_Resis += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.TECH_RESIS, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_TECH_RESIS, random);
                break;
            case AllStats.Att_Damage:
                Artif_Damage += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.DAMAGE, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_DAMAGE, random);
                break;
            case AllStats.Max_Mana:
                Artif_Mana += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MANA, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MANA, random);
                break;
            case AllStats.Regen_Mana:
                Artif_ManaRegen += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MANA_REGEN, BASE_VALUE_STATS_ARTEFACT.ADD_FOR_CHAR_MANA_REGEN, random);
                break;
        }
    }
    /// <summary>
    /// Вычисляем позитивный или негативный эффект и его силу
    /// </summary>
    /// <param name="levelCharm">Текущий уровень атрибута.</param>
    /// <param name="baseValue">Базовые значение атрибута(например инта 1, а скорость атаки 5)</param>
    /// <param name="addForChar">Множитель за уровень выше 1</param>
    /// <param name="random">Рандом по сиду</param>
    /// <returns>Итоговая сила + или -</returns>
    private float GetValueStat(int levelCharm, float baseValue, float addForChar, System.Random random = null)//Высчитываем позитивный или негативный чар
    {
        if (levelCharm < 1) return 0;
        float value, randValue;

        value = (addForChar * (levelCharm - 1)) + baseValue;

        if (random != null)
            randValue = (float)random.NextDouble(); // от 0 до 1
        else
            randValue = Random.value;

        float chancePositive = Mathf.Min(44f + art_level * 6f, 98) / 100f; //Чем выше уровень артефакта, тем меньше шанс на проклятие

        if (randValue < chancePositive)
        {
            chars_level += levelCharm;
        }
        else
        {
            value *= -1;
            curse_level += levelCharm;
        }

        return value;
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
       Artif_Mana == 0f &&
       Artif_ManaRegen == 0f;
    }
}
//public enum StatType
//{
//    Strength,
//    Agility,
//    Intelligence,
//    Hp,
//    Armor,
//    Evasion,
//    Mov_Speed,
//    Att_Range,
//    Att_Speed,
//    Proj_Speed,
//    ExpBust,
//    Mage_Resis,
//    Tech_Resis,
//    Damage,
//    Mana,
//    ManaRegen
//}