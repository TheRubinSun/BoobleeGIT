using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.U2D;

public enum damageT
{
    Magic,
    Technical,
    Physical,
    Posion,
    Mixed
}
[Serializable]
public abstract class Weapon : Item
{
    public bool rangeType { get; set; }
    public float range { get; set; }

    public damageT typeDamage;
    public int damage { get; set; }
    public float attackSpeedCoof { get; set; }
    public int addAttackSpeed {  get; set; }
    public int conut_Projectiles { get; set; }

    public Weapon(int id, string name, int maxCount, int spriteID, Quality quality,int cost, string decription, bool rangeType, float range, damageT typeDamage, int damage, float _attackSpeedCoof,  int _addAttackSpeed, int conut_Projectiles) :base(id, name, maxCount, spriteID, quality, cost, decription)
    {
        this.rangeType = rangeType;
        this.range = range;
        this.typeDamage = typeDamage;
        this.damage = damage;
        this.attackSpeedCoof = _attackSpeedCoof;
        this.addAttackSpeed = _addAttackSpeed;

        this.TypeItem = TypeItem.Weapon;
        this.conut_Projectiles = conut_Projectiles;
    }
}
[Serializable]
public class Gun : Weapon
{
    public float projectileSpeed { get; set; }
    public float projectileSpeedCoof { get; set; }
    public int idBulletPref {  get; set; }
    public float spreadAngle { get; set; }
    public Gun(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription, bool rangeType, float range, damageT typeDamage, int damage, float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles, float _projectileSpeed, float _projectileSpeedCoof, float _spreadAngle, int _idBulletPref) : base(id, name, maxCount, spriteID, quality, cost, decription, rangeType, range, typeDamage, damage, _attackSpeedCoof, _addAttackSpeed, conut_Projectiles)
    {
        projectileSpeed = _projectileSpeed;
        idBulletPref = _idBulletPref;
        projectileSpeedCoof = _projectileSpeedCoof;
        spreadAngle = _spreadAngle;

    }
}
[Serializable]
public class MeleWeapon: Weapon
{
    public MeleWeapon(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription, bool rangeType, float range, damageT typeDamage, int damage, float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles) : base(id, name, maxCount, spriteID, quality, cost, decription, rangeType, range, typeDamage, damage, _attackSpeedCoof, _addAttackSpeed, conut_Projectiles)
    {

    }
}
