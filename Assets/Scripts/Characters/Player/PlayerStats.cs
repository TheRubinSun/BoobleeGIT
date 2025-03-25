using NUnit.Framework;
using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerStats : CharacterStats
{
    public RoleClass classPlayer { get; set; }
    //Base Характеристики
    public int Base_Strength {  get; set; }
    public int Base_Agility { get; set; }
    public int Base_Intelligence { get; set; }
    public int Base_Max_Hp { get; set;}
    public int Base_Armor { get; set; }
    public int Base_Evasion {  get; set; }
    public float Base_Mov_Speed { get; set; }
    public float Base_Att_Range { get; set; }
    public int Base_Att_Damage { get; set; }
    public int Base_Att_Speed { get; set; }
    public float Base_Proj_Speed { get; set; }
    public float Base_ExpBust {  get; set; }
    public float Base_Magic_Resis { get; set; }
    public float Base_Tech_Resis { get; set; }
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

    public bool[] DirectionOrVectorWeapon { get; set; }
    public PlayerStats() { }
    public void SetBaseStats()
    {
        Base_Strength = 0;
        Base_Agility = 0;
        Base_Intelligence = 0;
        Base_Max_Hp = 2;
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
        count_Projectile = 0;
        MagicPoints = 1;
        TechniquePoints = 1;
        AdjacentPoints = 0;

        freeSkillPoints = 0;

        Gold = 0;
        TraderSkill = 1;

        classPlayer = Classes.Instance.GetRoleClass("Warrior");
        DirectionOrVectorWeapon = new bool[4];

        
    }
    public void LoadStats(PlayerStats playerSaveData)
    {
        Base_Strength = playerSaveData.Base_Strength;
        Base_Agility = playerSaveData.Base_Agility;
        Base_Intelligence = playerSaveData.Base_Intelligence;
        Base_Max_Hp = playerSaveData.Base_Max_Hp;
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
    }
    public void UpdateTotalStats()
    {
        EquipStats equipStats = Player.Instance.GetEquipStats();

        Strength = Base_Strength + classPlayer.Bonus_Class_Strength + equipStats.Bonus_Equip_Strength;
        Agility = Base_Agility + classPlayer.Bonus_Class_Agility + equipStats.Bonus_Equip_Agility;
        Intelligence = Base_Intelligence + classPlayer.Bonus_Class_Intelligence + equipStats.Bonus_Equip_Intelligence;

        Max_Hp = (Strength * 2) + Base_Max_Hp + classPlayer.Bonus_Class_Hp + equipStats.Bonus_Equip_Hp;
        Armor = (int)(Strength / 10) + Base_Armor + classPlayer.Bonus_Class_Armor + equipStats.Bonus_Equip_Armor;
        Mov_Speed = (Agility * 0.015f) + Base_Mov_Speed + classPlayer.Bonus_Class_SpeedMove + equipStats.Bonus_Equip_Mov_Speed;
        Evasion = (Agility) + Base_Evasion + equipStats.Bonus_Equip_Evasion;
        Att_Speed = (Agility * 2) + Base_Att_Speed + classPlayer.Bonus_Class_AttackSpeed + equipStats.Bonus_Equip_Att_Speed;
        Att_Range = (Intelligence * 0.1f) + Base_Att_Range + classPlayer.Bonus_Class_Range + equipStats.Bonus_Equip_Att_Range;
        Proj_Speed = (Intelligence * 0.1f) + Base_Proj_Speed + classPlayer.Bonus_Class_ProjectileSpeed + equipStats.Bonus_Equip_Proj_Speed;
        Att_Damage = (int)(((Strength * 2) + (Intelligence * 2)) / 10) + Base_Att_Damage + classPlayer.Bonus_Class_Damage + equipStats.Bonus_Equip_Att_Damage;

        ExpBust = Base_ExpBust + equipStats.Bonus_Equip_ExpBust;

        float Int_Resis = Intelligence / (Intelligence + 100f);
        Tech_Resis = 1 - ((1 - Int_Resis) * (1 - Base_Tech_Resis) * (1 - classPlayer.Bonus_Tech_Resis) * (1 - equipStats.Bonus_Tech_Resis));
        Magic_Resis = 1 - ((1 - Int_Resis) * (1 - Base_Magic_Resis) * (1 - classPlayer.Bonus_Magic_Resis) * (1 - equipStats.Bonus_Magic_Resis));
    }
    public void FillHp()
    {
        Cur_Hp = Max_Hp;
    }
    public void AddMaxHPBaseStat(int addMaxHp)
    {
        Base_Max_Hp += addMaxHp;
        Max_Hp = (Strength * 2) + Base_Max_Hp + classPlayer.Bonus_Class_Hp + EquipStats.Instance.Bonus_Equip_Hp;
        Cur_Hp += addMaxHp;
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
            nextLvl_exp = (int)((nextLvl_exp + 20) * 1.5f);
            return true;
        }
        return false;
    }
    public void LvlUpStat()
    {
        level++;
        freeSkillPoints++;
        AddMaxHPBaseStat(AddHP_PerLvl);
        Player.Instance.LvlUp();
        if (cur_exp >= nextLvl_exp) CheckLevel();
    }
}
