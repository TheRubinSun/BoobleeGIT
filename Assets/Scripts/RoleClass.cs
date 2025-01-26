using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoleClass: MonoBehaviour
{
    public float BonusRange { get; set; }

    public int BonusDamage { get; set; }
    public int BonusAttackSpeed { get; set; }
    public int BonusProjectileSpeed { get; set; }

    public int BonusSpeedMove { get; set; }
    public int BonusHp { get; set; }
    public int BonusDeffence { get; set; }

    public RoleClass(float bonusRange, int bonusDamage, int bonusAttackSpeed, int bonusProjectileSpeed, int bonusSpeedMove, int bonushp, int bonusDeffence)
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
[SerializeField]
public class AllClasses:MonoBehaviour
{
    [SerializeField]
    public Dictionary<string,RoleClass> roleClasses = new Dictionary<string, RoleClass>();
    public void Start()
    {
        roleClasses.Add("Shooter", new RoleClass(10, 0, 10, 10, 3, 10, 1));
        roleClasses.Add("Mage", new RoleClass(10, 0, 10, 10, 10, 3, 1));
        roleClasses.Add("Warrior", new RoleClass(10, 0, 10, 10, 3, 10, 1));
    }


}
