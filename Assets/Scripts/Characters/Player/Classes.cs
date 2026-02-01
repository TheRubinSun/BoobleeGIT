using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public static class Classes
{
    private static Dictionary<string, RoleClass> roleClasses = new Dictionary<string, RoleClass>();

    public static void LoadOrCreateClasses(Dictionary<string, RoleClass> classes)
    {
        if(classes!=null && classes.Count > 0)
        {
            roleClasses = classes;
        }
        else
        {
            //                    strength agility intell range, damage, at_sp, projSpeed, speed, hp, def
            //                                       s  a  i  r   d  a   p   s  h   d
            roleClasses.Add("Shooter", new RoleClass(1, 2, 1, 2f, 0, 45, 2f, 0.2f, 2, 0f, 0, 0, 0, 0));
            roleClasses.Add("Mage", new RoleClass(1, 1, 2, 1f, 0, 40, 1f, 0.2f, 3, 10, 0.5f, 0, 10, 10));
            roleClasses.Add("Warrior", new RoleClass(2, 1, 1, 1f, 2, 35, 0, 0.4f, 5, 0f, 0, 2, 0, 0));
        }
    }
    public static RoleClass GetRoleClass(string name)
    {
        return roleClasses[name];
    }
    public static Dictionary<string, RoleClass> GetClasses()
    {
        return roleClasses;
    }
}
[Serializable]
public class RoleClass
{
    public int Bonus_Class_Strength {  get; set; }
    public int Bonus_Class_Agility { get; set; }
    public int Bonus_Class_Intelligence { get; set; }
    public float Bonus_Class_Range { get; set; }
    public int Bonus_Class_Damage { get; set; }
    public int Bonus_Class_AttackSpeed { get; set; }
    public float Bonus_Class_ProjectileSpeed { get; set; }
    public float Bonus_Class_SpeedMove { get; set; }
    public int Bonus_Class_Hp { get; set; }
    public float Bonus_Class_Mana { get; set; }
    public float Bonus_Class_Regen_Mana { get; set; }
    public int Bonus_Class_Armor { get; set; }
    public float Bonus_Magic_Resis { get; set; }
    public float Bonus_Tech_Resis { get; set; }

    public RoleClass(int bonusStrength, int bonusAgility, int bonisIntelligence ,float bonusRange, int bonusDamage, 
        int bonusAttackSpeed, float bonusProjectileSpeed, float bonusSpeedMove, int bonusHp, float bonusMana, float regenMana, int bonusDeffence, 
        float bonus_Magic_Resis,float bonus_Tech_Resis)
    {
        Bonus_Class_Strength = bonusStrength;
        Bonus_Class_Agility = bonusAgility;
        Bonus_Class_Intelligence = bonisIntelligence;

        Bonus_Class_Range = bonusRange;
        Bonus_Class_Damage = bonusDamage;
        Bonus_Class_AttackSpeed = bonusAttackSpeed;
        Bonus_Class_ProjectileSpeed = bonusProjectileSpeed;
        Bonus_Class_SpeedMove = bonusSpeedMove;
        Bonus_Class_Hp = bonusHp;
        Bonus_Class_Mana = bonusMana;
        Bonus_Class_Regen_Mana = regenMana;
        Bonus_Class_Armor = bonusDeffence;
        Bonus_Magic_Resis = bonus_Magic_Resis;
        Bonus_Tech_Resis = bonus_Tech_Resis;
    }
}
