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

        if (!roleClasses.ContainsKey(name))
        {
            roleClasses.Add("Shooter", new RoleClass(10, 0, 10, 10, 3, 10, 1));
        }
        roleClasses.Add("Mage", new RoleClass(10, 0, 10, 10, 10, 3, 1));
        roleClasses.Add("Warrior", new RoleClass(10, 0, 10, 10, 3, 10, 1));
    }

    void Start()
    {

    }
    public RoleClass GetRoleClass(string name)
    {
        if (!roleClasses.ContainsKey(name))
        {
            roleClasses.Add("Shooter", new RoleClass(10, 0, 10, 10, 3, 10, 1));
        }
        return roleClasses[name];
    }
}
public class RoleClass
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
