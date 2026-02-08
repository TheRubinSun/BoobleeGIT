using System;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    //Статы
    public bool isRanged { get; set;}
    public float Attack_Interval { get; set;}
    public int GiveExp { get; set;}

    public void SetBuff(BuffsStats bf)
    {
        buffsStats = bf;
    }
    public bool isEvasion()
    {
        if (Evasion <= 0) return false;
        int random = UnityEngine.Random.Range(0, 100);
        if (Evasion >= random && random < 90)
        {
            return true;
        }
        return false;
    }
    public int TakePhysicalDamageStat(int damage)
    {
        int finalDamage = ((int)Mathf.Max(damage / (1 + Armor / 10f), 1));
        finalDamage -= Armor;
        finalDamage = Mathf.Max(finalDamage, 1);
        Cur_Hp -= finalDamage;
        return finalDamage;
    }
    public int TakeMagicDamageStat(int damage)
    {
        int finalDamage = Mathf.Max((int)(damage * (1 - Magic_Resis)), 1);
        Cur_Hp -= finalDamage;
        return finalDamage;
    }
    public int TakeTechDamageStat(int damage)
    {
        int finalDamage = Mathf.Max((int)(damage * (1 - Tech_Resis)), 1);
        Cur_Hp -= finalDamage;
        return finalDamage;
    }
    public int TakePosionDamageStat(int damage)
    {
        Cur_Hp -= damage;
        return damage;
    }
}
