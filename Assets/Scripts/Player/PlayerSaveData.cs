using System;
using UnityEngine;
[Serializable]
public class PlayerSaveData
{
    public int Cur_Hp;
    public int Max_Hp;
    public int Armor_Hp;

    public int Mov_Speed;

    public float Att_Range;
    public int Att_Damage;
    public int Att_Speed;
    public int Proj_Speed;

    public int level;
    public int freeSkillPoints;
    public int cur_exp;
    public int nextLvl_exp;
    

    public bool[] DirectionOrVectorWeapon = new bool[4];

    public RoleClass classPlayer;

    //Attacks
    public int count_Projectile;

    // Конструктор для преобразования Player в PlayerSaveData
    public PlayerSaveData(Player player)
    {
        Cur_Hp = player.Cur_Hp;
        Max_Hp = player.Max_Hp;
        Armor_Hp = player.Armor_Hp;

        Mov_Speed = player.Mov_Speed;

        Att_Range = player.Att_Range;
        Att_Damage = player.Att_Damage;
        Att_Speed = player.Att_Speed;
        Proj_Speed = player.Proj_Speed;

        level = player.level;
        freeSkillPoints = player.freeSkillPoints;
        cur_exp = player.cur_exp;
        nextLvl_exp = player.nextLvl_exp;
        count_Projectile = player.count_Projectile;
    }

    // Пустой конструктор для десериализации
    public PlayerSaveData() { }
}
