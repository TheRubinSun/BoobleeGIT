using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ArtifactObj
{
    public int ID_Art {  get; set; }
    public int art_level { get; set; }

    public int Artif_Strength { get; set; }
    public int Artif_Agility { get; set; }
    public int Artif_Intelligence { get; set; }
    public int Artif_Hp { get; set; }
    public int Artif_Armor { get; set; }
    public int Artif_Evasion { get; set; }
    public float Artif_Mov_Speed { get; set; }
    public float Artif_Att_Range { get; set; }
    public int Artif_Att_Speed { get; set; }
    public int Artif_Proj_Speed { get; set; }
    public float Artif_ExpBust { get; set; }
    public float Artif_Mage_Resis { get; set; }
    public float Artif_Tech_Resis { get; set; }

    public bool StatsGen { get; private set; }
    public ArtifactObj(int id,  int _art_level)
    {
        art_level = _art_level;
        ID_Art = id;

        SetRandomAttributes();
    }
    public ArtifactObj(int id)
    {
        ID_Art = id;
    }
    public ArtifactObj() { }
    public void SetAttributes()
    {

    }
    public void SetRandomAttributes()
    {
        int precentNewCharm = 0;
        int levelCharm = 0;
        ChechLevel(out precentNewCharm, out levelCharm);

        while (true)
        {
            if (Random.Range(0, 101) <= precentNewCharm)
            {
                GetStat(levelCharm);
            }
            else break;
        }
        StatsGen = true;
    }
    private void ChechLevel(out int precent, out int levelCharm)
    {
        int baseChance = 50;

        // –ассчитываем шанс
        precent = baseChance + art_level * 7; // ”величение шанса с каждым уровнем на 7%
        levelCharm = Random.Range(1, art_level + 1);

        // ƒл€ уровн€ выше 5 можно задать максимальный шанс
        if (precent > 80)
        {
            precent = 80; // ќграничиваем шанс максимальным значением, например, 80%
        }
    }
    private void GetStat(int levelCharm)
    {
        StatType stat = (StatType)Random.Range(0, System.Enum.GetValues(typeof(StatType)).Length);
        switch (stat)
        {
            case StatType.Strength:
                Artif_Strength += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.STRENGTH);
                break;
            case StatType.Agility:
                Artif_Agility += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.AGILITY);
                break;
            case StatType.Intelligence:
                Artif_Intelligence += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.INTELLIGENCE);
                break;
            case StatType.Hp:
                Artif_Hp += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.HP);
                break;
            case StatType.Armor:
                Artif_Armor += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ARMOR);
                break;
            case StatType.Evasion:
                Artif_Evasion += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.EVASION);
                break;
            case StatType.Mov_Speed:
                Artif_Mov_Speed += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MOV_SPEED);
                break;
            case StatType.Att_Range:
                Artif_Att_Range += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ATT_RANGE);
                break;
            case StatType.Att_Speed:
                Artif_Att_Speed += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.ATT_SPEED);
                break;
            case StatType.Proj_Speed:
                Artif_Proj_Speed += (int)GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.PROJ_SPEED);
                break;
            case StatType.ExpBust:
                Artif_ExpBust += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.EXPBUST);
                break;
            case StatType.Mage_Resis:
                Artif_Mage_Resis += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.MAGE_RESIS);
                break;
            case StatType.Tech_Resis:
                Artif_Tech_Resis += GetValueStat(levelCharm, BASE_VALUE_STATS_ARTEFACT.TECH_RESIS);
                break;
        }

    }
    private float GetValueStat(int levelCharm, float baseValue)
    {
        return baseValue *= Random.Range(-levelCharm, levelCharm + 1);
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
    Tech_Resis
}