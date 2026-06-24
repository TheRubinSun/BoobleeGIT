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
    public int TakePhysicalDamageWithArmor(int damage, int def)
    {
        return TakePhysicalDamageStat(damage, true, def);
    }
    public int TakePhysicalDamageStat(int damage, bool cheatArmor = false, int def = 0)
    {
        if (!cheatArmor) def = Armor;

        float damageMultiply = 1f;

        if(def >= 0)
        {
            damageMultiply = 1f / (1f + def / 10f);
        }
        else
        {
            damageMultiply = 1f - (def / 10f);
        }
        int finalDamage = Mathf.RoundToInt(damage * damageMultiply);
        if(def != 0) //Доп вычет урона или добавка, можно убрать
        {
            finalDamage -= def;
        }
        //int finalDamage = ((int)Mathf.Max(damage / (1 + def / 10f), 1));
        //finalDamage -= def;
        finalDamage = Mathf.Max(finalDamage, 1);
        Cur_Hp -= finalDamage;
        return finalDamage;
    }
    public int TakeMagicDamageStat(int damage)
    {
        int finalDamage;
        if (Magic_Resis >= 0)
        {
            finalDamage = (int)(damage * (1 - Magic_Resis));
        }
        else
        {
            finalDamage = (int)(damage * (1 + Magic_Resis));
        }
        finalDamage = Mathf.Max(finalDamage, 1);
        Cur_Hp -= finalDamage;
        return finalDamage;
    }
    public int TakeTechDamageStat(int damage)
    {
        int finalDamage;
        if (Tech_Resis >= 0)
        {
            finalDamage = (int)(damage * (1 - Tech_Resis));
        }
        else
        {
            finalDamage = (int)(damage * (1 + Tech_Resis));
        }
        finalDamage = Mathf.Max(finalDamage, 1);
        Cur_Hp -= finalDamage;
        return finalDamage;
    }
    public int TakePosionDamageStat(int damage)
    {
        Cur_Hp -= damage;
        return damage;
    }
}
