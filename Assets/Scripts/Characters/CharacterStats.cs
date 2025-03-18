using System;
using UnityEngine;

public class CharacterStats
{
    public int Cur_Hp { get; set; }
    [NonSerialized] public int Max_Hp;
    [NonSerialized] public int Armor;
    [NonSerialized] public int Evasion;
    [NonSerialized] public float Mov_Speed;
    [NonSerialized] public float Att_Range;
    [NonSerialized] public int Att_Damage;
    [NonSerialized] public int Att_Speed;
    [NonSerialized] public float Proj_Speed;

}
