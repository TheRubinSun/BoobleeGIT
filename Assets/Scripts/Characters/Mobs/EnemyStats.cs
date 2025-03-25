using System;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    //Статы
    public bool isRanged { get; set;}
    public float Attack_Interval { get; set;}
    public int GiveExp { get; set;}

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
}
