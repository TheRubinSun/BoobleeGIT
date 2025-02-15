using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.U2D;

public enum damageT
{
    Magic,
    Technical,
    Cutting,
    Crushing,
    Stabbing,
    Bursting
}
[Serializable]
public abstract class Weapon : Item
{
    public bool rangeType { get; set; }
    public float range { get; set; }

    public damageT typeDamage;
    public int damage { get; set; }
    public float attackSpeed { get; set; }

    public Weapon(int id, string name, int maxCount, int spriteID, Quality quality, string decription, bool rangeType, float range, damageT typeDamage, int damage, float attackSpeed):base(id, name, maxCount, spriteID, quality, decription)
    {
        this.rangeType = rangeType;
        this.range = range;
        this.typeDamage = typeDamage;
        this.damage = damage;
        this.attackSpeed = attackSpeed;
        this.TypeItem = TypeItem.Weapon;
    }

    public abstract void Attack();
}
[Serializable]
public class Gun : Weapon
{
    public float projectileSpeed { get; set; }
    public int idBulletPref {  get; set; }
    public Gun(int id, string name, int maxCount, int spriteID, Quality quality, string decription, bool rangeType, float range, damageT typeDamage, int damage, float attackSpeed, float _projectileSpeed, int _idBulletPref) : base(id, name, maxCount, spriteID, quality, decription, rangeType, range, typeDamage, damage, attackSpeed)
    {
        projectileSpeed = _projectileSpeed;
        idBulletPref = _idBulletPref;
        
    }
    public override void Attack()
    {
        
    }
}
[Serializable]
public class Sword: Weapon
{
    public Sword(int id, string name, int maxCount, int spriteID, Quality quality, string decription, bool rangeType, float range, damageT typeDamage, int damage, float attackSpeed) : base(id, name, maxCount, spriteID, quality, decription, rangeType, range, typeDamage, damage, attackSpeed)
    {

    }
    public override void Attack()
    {

    }
}
