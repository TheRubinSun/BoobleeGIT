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
            //                           range, damage, at_sp, projSpeed, speed, hp, def
            //                                       r   d  a   p   s  h   d
            roleClasses.Add("Shooter", new RoleClass(2f, 0, 50, 4f, 2, 5, 0));
            roleClasses.Add("Mage",    new RoleClass(1f, 4, 40, 0, 2, 7, 0));
            roleClasses.Add("Warrior", new RoleClass(1f, 2, 30, 0, 4, 10, 2));
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
    public float BonusRange { get; set; }
    public int BonusDamage { get; set; }
    public int BonusAttackSpeed { get; set; }
    public float BonusProjectileSpeed { get; set; }
    public int BonusSpeedMove { get; set; }
    public int BonusHp { get; set; }
    public int BonusDeffence { get; set; }

    public RoleClass(float bonusRange, int bonusDamage, int bonusAttackSpeed, float bonusProjectileSpeed, int bonusSpeedMove, int bonushp, int bonusDeffence)
    {
        this.BonusRange = bonusRange;
        this.BonusDamage = bonusDamage;
        this.BonusAttackSpeed = bonusAttackSpeed;
        this.BonusProjectileSpeed = bonusProjectileSpeed;
        this.BonusSpeedMove = bonusSpeedMove;
        this.BonusHp = bonushp;
        this.BonusDeffence = bonusDeffence;
    }
}
