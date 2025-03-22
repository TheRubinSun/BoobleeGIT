using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Classes : MonoBehaviour 
{
    public static Classes Instance { get; private set; }

    [SerializeField] Dictionary<string, RoleClass> roleClasses = new Dictionary<string, RoleClass>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }
    public void LoadOrCreateClasses(Dictionary<string, RoleClass> classes)
    {
        if(classes!=null && classes.Count > 0)
        {
            roleClasses = classes;
        }
        else
        {
            //                    strength agility intell range, damage, at_sp, projSpeed, speed, hp, def
            //                                       s  a  i  r   d  a   p   s  h   d
            roleClasses.Add("Shooter", new RoleClass(0, 2, 0, 2f, 0, 35, 4f, 0.2f, 2, 0));
            roleClasses.Add("Mage",    new RoleClass(0, 0, 2, 1f, 4, 30, 0, 0.2f, 3, 0));
            roleClasses.Add("Warrior", new RoleClass(2, 0, 0, 1f, 2, 25, 0, 0.4f, 4, 2));
        }
    }
    void Start()
    {

    }
    public RoleClass GetRoleClass(string name)
    {
        return roleClasses[name];
    }
    public Dictionary<string, RoleClass> GetClasses()
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
    public int Bonus_Class_Armor { get; set; }

    public RoleClass(int bonusStrength, int bonusAgility, int bonisIntelligence ,float bonusRange, int bonusDamage, int bonusAttackSpeed, float bonusProjectileSpeed, float bonusSpeedMove, int bonushp, int bonusDeffence)
    {
        this.Bonus_Class_Strength = bonusStrength;
        this.Bonus_Class_Agility = bonusAgility;
        this.Bonus_Class_Intelligence = bonisIntelligence;

        this.Bonus_Class_Range = bonusRange;
        this.Bonus_Class_Damage = bonusDamage;
        this.Bonus_Class_AttackSpeed = bonusAttackSpeed;
        this.Bonus_Class_ProjectileSpeed = bonusProjectileSpeed;
        this.Bonus_Class_SpeedMove = bonusSpeedMove;
        this.Bonus_Class_Hp = bonushp;
        this.Bonus_Class_Armor = bonusDeffence;
    }
}
