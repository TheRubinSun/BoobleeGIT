using System;
using UnityEditor.Playables;
using UnityEngine;

public class CharacterStats
{
    public int Base_Max_Hp { get; set; }
    public float Base_Max_Mana { get; set; }
    public float Base_Regen_Mana { get; set; }
    public int Base_Armor { get; set; }
    public int Base_Evasion { get; set; }
    public float Base_Mov_Speed { get; set; }
    public float Base_Att_Range { get; set; }
    public int Base_Att_Damage { get; set; }
    public int Base_Att_Speed { get; set; }
    public float Base_Proj_Speed { get; set; }
    public float Base_Magic_Resis { get; set; }
    public float Base_Tech_Resis { get; set; }

    [NonSerialized] public int Max_Hp;
    [NonSerialized] public float Max_Mana;
    [NonSerialized] public float Regen_Mana;
    [NonSerialized] public int Armor;
    [NonSerialized] public int Evasion;
    [NonSerialized] public float Mov_Speed;
    [NonSerialized] public float Att_Range;
    [NonSerialized] public int Att_Damage;
    [NonSerialized] public int Att_Speed;
    [NonSerialized] public float Proj_Speed;
    [NonSerialized] public float Magic_Resis;
    [NonSerialized] public float Tech_Resis;
    [NonSerialized] public BuffsStats buffsStats;
    public int Cur_Hp { get; set; }
    public float Cur_Mana { get; set; }

    public virtual void UpdateTotalStats()
    {
        Max_Hp = Base_Max_Hp + buffsStats.Buff_Max_Hp;
        Max_Mana = Base_Max_Mana + buffsStats.Buff_Max_Mana;
        Regen_Mana = Base_Regen_Mana + buffsStats.Buff_Regen_Mana;

        Armor = Base_Armor + buffsStats.Buff_Armor;
        Evasion = Base_Evasion + buffsStats.Buff_Evasion;
        Mov_Speed = Base_Mov_Speed + buffsStats.Buff_Mov_Speed;

        Att_Range = Base_Att_Range + buffsStats.Buff_Att_Range;
        Att_Damage = Base_Att_Damage + buffsStats.Buff_Att_Damage;
        Att_Speed = Base_Att_Speed + buffsStats.Buff_Att_Speed;
        Proj_Speed = Base_Proj_Speed + buffsStats.Buff_Proj_Speed;

        Magic_Resis = Base_Magic_Resis + buffsStats.Buff_Magic_Resis;
        Tech_Resis = Base_Tech_Resis + buffsStats.Buff_Tech_Resis;
    }
    public virtual void ApplyStat(AllParametrs param, int multiplier)
    {
        switch (param)
        {
            case AllParametrs.Max_Hp:
                Max_Hp = Base_Max_Hp + buffsStats.Buff_Max_Hp * multiplier;
                break;
            case AllParametrs.Max_Mana:
                Max_Mana = Base_Max_Mana + buffsStats.Buff_Max_Mana * multiplier;
                break;
            case AllParametrs.Regen_Mana:
                Regen_Mana = Base_Regen_Mana + buffsStats.Buff_Regen_Mana * multiplier;
                break;
            case AllParametrs.Armor:
                Armor = Base_Armor + buffsStats.Buff_Armor * multiplier;
                break;
            case AllParametrs.Evasion:
                Evasion = Base_Evasion + buffsStats.Buff_Evasion * multiplier;
                break;
            case AllParametrs.Mov_Speed:
                Mov_Speed = Base_Mov_Speed + buffsStats.Buff_Mov_Speed * multiplier;
                break;
            case AllParametrs.Att_Range:
                Att_Range = Base_Att_Range + buffsStats.Buff_Att_Range * multiplier;
                break;
            case AllParametrs.Att_Damage:
                Att_Damage = Base_Att_Damage + buffsStats.Buff_Att_Damage * multiplier;
                break;
            case AllParametrs.Att_Speed:
                Att_Speed = Base_Att_Speed + buffsStats.Buff_Att_Speed * multiplier;
                break;
            case AllParametrs.Proj_Speed:
                Proj_Speed = Base_Proj_Speed + buffsStats.Buff_Proj_Speed * multiplier;
                break;
            case AllParametrs.Magic_Resis:
                Magic_Resis = Base_Magic_Resis + buffsStats.Buff_Magic_Resis * multiplier;
                break;
            case AllParametrs.Tech_Resis:
                Tech_Resis = Base_Tech_Resis + buffsStats.Buff_Tech_Resis * multiplier;
                break;

        }
    }
}
public enum AllParametrs
{
    Strength,
    Agility,
    Intelligence,
    Max_Hp,
    Max_Mana,
    Regen_Mana,
    Armor,
    Mov_Speed,
    Evasion,
    Att_Speed,
    Att_Range,
    Proj_Speed,
    Att_Damage,
    ExpBust,
    Tech_Resis,
    Magic_Resis
}

public interface ITakeDamage
{
    public void TakeDamage(int damage, damageT typeAttack, bool canEvade, EffectData effect);
}
public interface IAttack
{
    public void RangeAttack();
    public void MeleeAttack();
}
public interface IItemMove
{
    public void SetItemsPosIdle(int frame);
    public void SetItemsPosMove(int frame);
    public void SetItemsPosShoot(int frame);
    public void SetItemsPosMeleAttack(int frame);
}

