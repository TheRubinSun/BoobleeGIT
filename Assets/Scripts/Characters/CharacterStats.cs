using System;
using UnityEngine;

public class CharacterStats
{
    public int Cur_Hp { get; set; }
    public float Cur_Mana { get; set; }
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

