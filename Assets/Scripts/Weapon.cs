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
public abstract class Weapon : Item
{
    public bool rangeType { get; protected set; }
    public float range { get; protected set; }

    public damageT typeDamage;
    public int damage { get; protected set; }
    public int attackSpeed { get; protected set; }

    public Weapon(int id, string name, int maxCount, Sprite sprite, Quality quality, string decription, bool rangeType, float range, damageT typeDamage, int damage, int attackSpeed):base(id, name, maxCount, sprite, quality, decription)
    {
        this.rangeType = rangeType;
        this.range = range;
        this.typeDamage = typeDamage;
        this.damage = damage;
        this.attackSpeed = attackSpeed;
        
    }

    public abstract void Attack();
}
public class Gun : Weapon
{
    public int projectileSpeed { get; private set; }
    public Gun(int id, string name, int maxCount, Sprite sprite, Quality quality, string decription, bool rangeType, float range, damageT typeDamage, int damage, int attackSpeed, int projectileSpeed): base(id, name, maxCount, sprite, quality, decription, rangeType, range, typeDamage, damage, attackSpeed)
    {
        this.projectileSpeed = projectileSpeed;
        this.TypeItem = TypeItem.Gun;
    }
    public override void Attack()
    {
        
    }
}
public class Sword: Weapon
{
    public Sword(int id, string name, int maxCount, Sprite sprite, Quality quality, string decription, bool rangeType, float range, damageT typeDamage, int damage, int attackSpeed) : base(id, name, maxCount, sprite, quality, decription, rangeType, range, typeDamage, damage, attackSpeed)
    {
        this.TypeItem = TypeItem.Weapon;
    }
    public override void Attack()
    {

    }
}
