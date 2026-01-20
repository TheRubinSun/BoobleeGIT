using NUnit.Framework;
using System;
using UnityEngine;

[System.Serializable]
public class PlayerStats : CharacterStats
{
    public RoleClass classPlayer { get; set; }
    //Base Характеристики
    public int Base_Strength {  get; set; }
    public int Base_Agility { get; set; }
    public int Base_Intelligence { get; set; }
    public float Base_ExpBust {  get; set; }

    public float Int_Resis;

    //Итоговые Характеристики

    [NonSerialized] public int Strength;
    [NonSerialized] public int Agility;
    [NonSerialized] public int Intelligence;
    [NonSerialized] public float ExpBust;

    //Exp
    public int level { get; set; }
    public int freeSkillPoints { get; set; }
    public int cur_exp { get; set; }
    public int nextLvl_exp { get; set; }


    //Trade
    public int trade_level { get; set; }
    public int trade_cur_exp { get; set; }
    public int trade_nextLvl_exp { get; set; }

    //Farm
    public int farm_level { get; set; }
    public int farm_cur_exp { get; set; }
    public int farm_nextLvl_exp { get; set; }

    //Res collections
    public int collect_level { get; set; }
    public int collect_cur_exp { get; set; }
    public int collec_nextLvl_exp { get; set; }

    //Knowlange
    public int MagicPoints { get; set; }
    public int TechniquePoints { get; set; }
    public int AdjacentPoints { get; set; }

    //Gold
    public int Gold {  get; set; }
    public int TraderSkill { get; set; }

    //Mod Attack
    public int count_Projectile { get; set; }

    private const int AddHP_PerLvl = 2;
    private const int AddMana_PerLvl = 4;

    private EquipStats equipStats;
    public bool[] DirectionOrVectorWeapon { get; set; }
    public PlayerStats() { }

    public void SetBaseStats()
    {
        Base_Strength = 0;
        Base_Agility = 0;
        Base_Intelligence = 0;
        Base_Max_Hp = 2;
        Base_Max_Mana = 6;
        Base_Regen_Mana = 0.25f;
        Base_Armor = 0;
        Base_Evasion = 0;
        Base_Mov_Speed = 1f;
        Base_Att_Range = 0;
        Base_Att_Speed = 0;
        Base_Att_Damage = 0;
        Base_Att_Speed = 0;
        Base_Proj_Speed = 0;
        Base_ExpBust = 1f;

        Base_Magic_Resis = 0;
        Base_Tech_Resis = 0;

        nextLvl_exp = 10;
        level = 0;

        trade_level = 0;
        trade_nextLvl_exp = 30;

        farm_level = 0;
        farm_nextLvl_exp = 30;

        collect_level = 0;
        collec_nextLvl_exp = 30;


        count_Projectile = 0;
        MagicPoints = 1;
        TechniquePoints = 1;
        AdjacentPoints = 0;

        freeSkillPoints = 0;

        Gold = 10;
        TraderSkill = 1;

        classPlayer = GlobalData.Classes.GetRoleClass("Shooter");
        DirectionOrVectorWeapon = new bool[4];

        equipStats = GlobalData.Player.GetEquipStats();
        buffsStats = GlobalData.Player.GetBuffStatsPlayer();
    }
    public void LoadStats(PlayerStats playerSaveData)
    {
        Base_Strength = playerSaveData.Base_Strength;
        Base_Agility = playerSaveData.Base_Agility;
        Base_Intelligence = playerSaveData.Base_Intelligence;
        Base_Max_Hp = playerSaveData.Base_Max_Hp;
        Base_Max_Mana = playerSaveData.Base_Max_Mana;
        Base_Regen_Mana = playerSaveData.Base_Regen_Mana;
        Base_Armor = playerSaveData.Base_Armor;
        Base_Evasion = playerSaveData.Base_Evasion;
        Base_Mov_Speed = playerSaveData.Base_Mov_Speed;
        Base_Att_Range = playerSaveData.Base_Att_Range;
        Base_Att_Speed = playerSaveData.Base_Att_Speed;
        Base_Att_Damage = playerSaveData.Base_Att_Damage;
        Base_Att_Speed = playerSaveData.Base_Att_Speed;
        Base_Proj_Speed = playerSaveData.Base_Proj_Speed;
        Base_ExpBust = playerSaveData.Base_ExpBust;

        Base_Magic_Resis = playerSaveData.Base_Magic_Resis;
        Base_Tech_Resis = playerSaveData.Base_Tech_Resis;

        nextLvl_exp = playerSaveData.nextLvl_exp;
        cur_exp = playerSaveData.cur_exp;
        level = playerSaveData.level;

        trade_level = playerSaveData.trade_level;
        trade_cur_exp = playerSaveData.trade_cur_exp;
        trade_nextLvl_exp = playerSaveData.trade_nextLvl_exp;

        farm_level = playerSaveData.farm_level;
        farm_cur_exp = playerSaveData.farm_cur_exp;
        farm_nextLvl_exp = playerSaveData.farm_nextLvl_exp;

        collect_level = playerSaveData.collect_level;
        collect_cur_exp = playerSaveData.collect_cur_exp;
        collec_nextLvl_exp = playerSaveData.collec_nextLvl_exp;

        count_Projectile = playerSaveData.count_Projectile;
        MagicPoints = playerSaveData.MagicPoints;
        TechniquePoints = playerSaveData.TechniquePoints;
        AdjacentPoints = playerSaveData.AdjacentPoints;

        Gold = playerSaveData.Gold;
        TraderSkill = playerSaveData.TraderSkill;

        freeSkillPoints = playerSaveData.freeSkillPoints;

        classPlayer = playerSaveData.classPlayer;
        DirectionOrVectorWeapon = playerSaveData.DirectionOrVectorWeapon;

        Cur_Hp = playerSaveData.Cur_Hp;
        Cur_Mana = playerSaveData.Cur_Mana;

        equipStats = GlobalData.Player.GetEquipStats();
        buffsStats = GlobalData.Player.GetBuffStatsPlayer();
    }
    public override void UpdateTotalStats()
    {
        bool isFullyHp = Max_Hp == Cur_Hp ? true : false;
        bool isFullyMana = Max_Mana == Cur_Mana ? true : false;

        Strength = Base_Strength + classPlayer.Bonus_Class_Strength + equipStats.Bonus_Equip_Strength + buffsStats.Buff_Strength;
        Agility = Base_Agility + classPlayer.Bonus_Class_Agility + equipStats.Bonus_Equip_Agility + buffsStats.Buff_Agility;
        Intelligence = Base_Intelligence + classPlayer.Bonus_Class_Intelligence + equipStats.Bonus_Equip_Intelligence + buffsStats.Buff_Intelligence;

        Max_Hp = (Strength * 2) + Base_Max_Hp + classPlayer.Bonus_Class_Hp + equipStats.Bonus_Equip_Hp + buffsStats.Buff_Max_Hp;
        Max_Mana = (Intelligence * 4) + Base_Max_Mana + classPlayer.Bonus_Class_Mana + equipStats.Bonus_Equip_Mana + buffsStats.Buff_Max_Mana;
        Regen_Mana = (Intelligence * 0.125f) + Base_Regen_Mana + classPlayer.Bonus_Class_Regen_Mana + equipStats.Bonus_Equip_Regen_Mana + buffsStats.Buff_Regen_Mana;

        Armor = (int)(Strength / 10) + Base_Armor + classPlayer.Bonus_Class_Armor + equipStats.Bonus_Equip_Armor + buffsStats.Buff_Armor;
        Mov_Speed = (Agility * 0.015f) + Base_Mov_Speed + classPlayer.Bonus_Class_SpeedMove + equipStats.Bonus_Equip_Mov_Speed + buffsStats.Buff_Mov_Speed;
        Evasion = (Agility) + Base_Evasion + equipStats.Bonus_Equip_Evasion + buffsStats.Buff_Evasion;
        Att_Speed = (Agility * 2) + Base_Att_Speed + classPlayer.Bonus_Class_AttackSpeed + equipStats.Bonus_Equip_Att_Speed + buffsStats.Buff_Att_Speed;
        Att_Range = Base_Att_Range + classPlayer.Bonus_Class_Range + equipStats.Bonus_Equip_Att_Range + buffsStats.Buff_Att_Range;
        Proj_Speed = Base_Proj_Speed + classPlayer.Bonus_Class_ProjectileSpeed + equipStats.Bonus_Equip_Proj_Speed + buffsStats.Buff_Proj_Speed;
        Att_Damage = (int)(((Strength * 2) + (Intelligence * 2)) / 10) + Base_Att_Damage + classPlayer.Bonus_Class_Damage + equipStats.Bonus_Equip_Att_Damage + buffsStats.Buff_Att_Damage;

        ExpBust = Base_ExpBust + equipStats.Bonus_Equip_ExpBust;

        float Int_Resis = Intelligence / (Intelligence + 100f);
        Tech_Resis = 1 - ((1 - Int_Resis) * (1 - Base_Tech_Resis) * (1 - classPlayer.Bonus_Tech_Resis) * (1 - equipStats.Bonus_Tech_Resis)) + buffsStats.Buff_Tech_Resis;
        Magic_Resis = 1 - ((1 - Int_Resis) * (1 - Base_Magic_Resis) * (1 - classPlayer.Bonus_Magic_Resis) * (1 - equipStats.Bonus_Magic_Resis)) + buffsStats.Buff_Magic_Resis;

        if (isFullyHp)
            Cur_Hp = Max_Hp;
        if (isFullyMana)
            Cur_Mana = Max_Mana;

        if (Cur_Hp > Max_Hp)
            Cur_Hp = Max_Hp;
        if (Cur_Mana > Max_Mana)
            Cur_Mana = Max_Mana;
    }
    public override void ApplyStat(AllStats stat, int multiplier)
    {
        switch (stat)
        {
            case AllStats.Strength:
                Strength = Base_Strength + classPlayer.Bonus_Class_Strength + equipStats.Bonus_Equip_Strength + buffsStats.Buff_Strength * multiplier;
                break;
            case AllStats.Agility:
                Agility = Base_Agility + classPlayer.Bonus_Class_Agility + equipStats.Bonus_Equip_Agility + buffsStats.Buff_Agility * multiplier;
                break;
            case AllStats.Intelligence:
                Intelligence = Base_Intelligence + classPlayer.Bonus_Class_Intelligence + equipStats.Bonus_Equip_Intelligence + buffsStats.Buff_Intelligence * multiplier;
                break;
            case AllStats.Max_Hp:
                Max_Hp = (Strength * 2) + Base_Max_Hp + classPlayer.Bonus_Class_Hp + equipStats.Bonus_Equip_Hp + buffsStats.Buff_Max_Hp * multiplier;
                break;
            case AllStats.Max_Mana:
                Max_Mana = (Intelligence * 4) + Base_Max_Mana + classPlayer.Bonus_Class_Mana + equipStats.Bonus_Equip_Mana + buffsStats.Buff_Max_Mana * multiplier;
                break;
            case AllStats.Regen_Mana:
                Regen_Mana = (Intelligence * 0.125f) + Base_Regen_Mana + classPlayer.Bonus_Class_Regen_Mana + equipStats.Bonus_Equip_Regen_Mana + buffsStats.Buff_Regen_Mana * multiplier;
                break;
            case AllStats.Armor:
                Armor = (int)(Strength / 10f) + Base_Armor + classPlayer.Bonus_Class_Armor + equipStats.Bonus_Equip_Armor + buffsStats.Buff_Armor * multiplier;
                break;
            case AllStats.Mov_Speed:
                Mov_Speed = (Agility * 0.015f) + Base_Mov_Speed + classPlayer.Bonus_Class_SpeedMove + equipStats.Bonus_Equip_Mov_Speed + buffsStats.Buff_Mov_Speed * multiplier;
                break;
            case AllStats.Evasion:
                Evasion = Agility + Base_Evasion + equipStats.Bonus_Equip_Evasion + buffsStats.Buff_Evasion * multiplier;
                break;
            case AllStats.Att_Speed:
                Att_Speed = (int)((Agility * 2) + Base_Att_Speed + classPlayer.Bonus_Class_AttackSpeed + equipStats.Bonus_Equip_Att_Speed + buffsStats.Buff_Att_Speed * multiplier);
                break;
            case AllStats.Att_Range:
                Att_Range = Base_Att_Range + classPlayer.Bonus_Class_Range + equipStats.Bonus_Equip_Att_Range + buffsStats.Buff_Att_Range * multiplier;
                break;
            case AllStats.Proj_Speed:
                Proj_Speed = Base_Proj_Speed + classPlayer.Bonus_Class_ProjectileSpeed + equipStats.Bonus_Equip_Proj_Speed + buffsStats.Buff_Proj_Speed * multiplier;
                break;
            case AllStats.Att_Damage:
                Att_Damage = (int)(((Strength * 2) + (Intelligence * 2)) / 10f) + Base_Att_Damage + classPlayer.Bonus_Class_Damage + equipStats.Bonus_Equip_Att_Damage + buffsStats.Buff_Att_Damage * multiplier;
                break;
            case AllStats.ExpBust:
                ExpBust = Base_ExpBust + equipStats.Bonus_Equip_ExpBust + buffsStats.Buff_ExpBust * multiplier;
                break;
            case AllStats.Tech_Resis:
                {
                    float Int_Resis = Intelligence / (Intelligence + 100f);
                    Tech_Resis = 1 - ((1 - Int_Resis) * (1 - Base_Tech_Resis) * (1 - classPlayer.Bonus_Tech_Resis) * (1 - equipStats.Bonus_Tech_Resis)) + buffsStats.Buff_Tech_Resis * multiplier;
                }
                break;
            case AllStats.Magic_Resis:
                {
                    float Int_Resis = Intelligence / (Intelligence + 100f);
                    Magic_Resis = 1 - ((1 - Int_Resis) * (1 - Base_Magic_Resis) * (1 - classPlayer.Bonus_Magic_Resis) * (1 - equipStats.Bonus_Magic_Resis)) + buffsStats.Buff_Magic_Resis * multiplier;
                }
                break;
        }
    }
    public void RegenMana()
    {
        if(Cur_Mana < Max_Mana)
        {
            if (Cur_Mana + (Regen_Mana/10) < Max_Mana) Cur_Mana += (Regen_Mana/10);
            else Cur_Mana = Max_Mana;
        }
    }
    public void FillHp()
    {
        Cur_Hp = Max_Hp;
    }
    public void FillMana()
    {
        Cur_Mana = Max_Mana;
    }
    public void AddMaxHPBaseStat(int addMaxHp)
    {
        Base_Max_Hp += addMaxHp;
        Max_Hp = (Strength * 2) + Base_Max_Hp + classPlayer.Bonus_Class_Hp + GlobalData.EquipStats.Bonus_Equip_Hp;
        Cur_Hp += addMaxHp;
    }
    public void AddMaxManaBaseStat(int addMaxMana)
    {
        Base_Max_Mana += addMaxMana;
        Max_Mana = (Intelligence * 4) + Base_Max_Mana + classPlayer.Bonus_Class_Mana + GlobalData.EquipStats.Bonus_Equip_Mana;
        Cur_Mana += addMaxMana;
    }
    public bool isEvasion()
    {
        int random = UnityEngine.Random.Range(0, 100);
        if (Evasion >= random && random < 90)
        {
            return true;
        }
        return false;
    }
    public bool TakePhysicalDamageStat(int damage)
    {
        float decreasePhisDamage = (Mathf.Max(damage / (1 + Armor / 10f), 1));
        decreasePhisDamage -= Armor;
        Cur_Hp -= (int)Mathf.Max(decreasePhisDamage, 1);
        return true;
    }
    public bool TakeMagicDamageStat(int damage)
    {
        float finalDamage = damage * (1 - Magic_Resis);
        Cur_Hp -= (Mathf.Max((int)finalDamage, 1));
        return true;
    }
    public bool TakeTechDamageStat(int damage)
    {
        float finalDamage = damage * (1 - Tech_Resis);
        Cur_Hp -= (Mathf.Max((int)finalDamage, 1));
        return true;
    }
    public bool TakePosionDamageStat(int damage)
    {
        Cur_Hp -= damage;
        return true;
    }
    public bool HaveMana(int spendMana) => spendMana <= Cur_Mana ? true : false;

    public bool SpendMana(int spendMana)
    {
        Cur_Mana -= spendMana;
        return true;
    }
    public void AddAttribute(AllStats bosterType, int count)
    {
        switch(bosterType)
        {
            case AllStats.Strength:
                {
                    Base_Strength += count;
                    break;
                }
            case AllStats.Agility:
                {
                    Base_Agility += count;
                    break;
                }
            case AllStats.Intelligence:
                {
                    Base_Intelligence += count;
                    break;
                }
            default:
                {
                    Debug.LogError("Такого бустера нет!");
                    break;
                }

        }
    }
    public bool PlayerHealStat(int count_heal)
    {
        if (Cur_Hp < Max_Hp)
        {
            if (Cur_Hp + count_heal >= Max_Hp)
            {
                Cur_Hp = Max_Hp;
            }
            else
            {
                Cur_Hp += count_heal;
            }
            return true;
        }
        return false;
    }
    public bool PlayerManaHealStat(int count_mana_heal)
    {
        if (Cur_Mana < Max_Mana)
        {
            if (Cur_Mana + count_mana_heal >= Max_Mana)
            {
                Cur_Mana = Max_Mana;
            }
            else
            {
                Cur_Mana += count_mana_heal;
            }
            return true;
        }
        return false;
    }

    public void AddExpStat(int add_exp)
    {
        int added_exp = Mathf.RoundToInt(add_exp * ExpBust);
        cur_exp += added_exp;
        CheckLevel();
    }
    private void CheckLevel()
    {
        if (isNewLevel())
            LvlUpStat();
    }
    private bool isNewLevel()
    {
        if (cur_exp >= nextLvl_exp)
        {
            cur_exp -= nextLvl_exp;
            nextLvl_exp = (int)((nextLvl_exp + 30) * 1.3f);
            return true;
        }
        return false;
    }
    public void LvlUpStat()
    {
        level++;
        freeSkillPoints++;
        AddMaxHPBaseStat(AddHP_PerLvl);
        AddMaxManaBaseStat(AddMana_PerLvl);
        GlobalData.Player.LvlUp();
        if (cur_exp >= nextLvl_exp) CheckLevel();
    }
    public void AddTypeExp(TypeExp typeExp, int add_exp)
    {
        switch (typeExp)
        {
            case TypeExp.None:
                break;
            case TypeExp.Trade:
                trade_cur_exp += add_exp;
                CheckTradeLevel();
                break;
            case TypeExp.Farm:
                farm_cur_exp += add_exp;
                CheckFarmLevel();
                break;
            case TypeExp.Collect:
                collect_cur_exp += add_exp;
                CheckCollectLevel();
                break;
        }
    }
    //Trade
    private void CheckTradeLevel()
    {
        if (isNewTradeLevel())
            TradeLvlUp();
    }
    private bool isNewTradeLevel()
    {
        if (trade_cur_exp >= trade_nextLvl_exp)
        {
            trade_cur_exp -= trade_nextLvl_exp;
            trade_nextLvl_exp = (int)((trade_nextLvl_exp + 30 * trade_level) * 1.2f);
            return true;
        }
        return false;
    }
    public void TradeLvlUp()
    {
        trade_level++;
        TraderSkill++;
        GlobalData.Player.TradeLvlUp();
        if (trade_cur_exp >= trade_nextLvl_exp) CheckTradeLevel();
    }
    //Farm
    private void CheckFarmLevel()
    {
        if (isNewFarmLevel())
            FarmLvlUp();
    }
    private bool isNewFarmLevel()
    {
        if (farm_cur_exp >= farm_nextLvl_exp)
        {
            farm_cur_exp -= farm_nextLvl_exp;
            farm_nextLvl_exp = (int)((farm_nextLvl_exp + 30 * farm_level) * 1.2f);
            return true;
        }
        return false;
    }
    public void FarmLvlUp()
    {
        farm_level++;
        GlobalData.Player.FarmLvlUp();
        if (farm_cur_exp >= farm_nextLvl_exp) CheckFarmLevel();
    }
    //Collect
    private void CheckCollectLevel()
    {
        if (isNewCollectLevel())
            CollectLvlUp();
    }
    private bool isNewCollectLevel()
    {
        if (collect_cur_exp >= collec_nextLvl_exp)
        {
            collect_cur_exp -= collec_nextLvl_exp;
            collec_nextLvl_exp = (int)((collec_nextLvl_exp + 30 * collect_level) * 1.2f);
            return true;
        }
        return false;
    }
    public void CollectLvlUp()
    {
        collect_level++;

        GlobalData.Player.CollectLvlUp();
        if (collect_cur_exp >= collec_nextLvl_exp) CheckCollectLevel();
    }
}
