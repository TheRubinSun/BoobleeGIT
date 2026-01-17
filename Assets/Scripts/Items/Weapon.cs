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
public interface ILaserWeapon
{
    int CountPenetration { get; set; }
}
public interface IBulletWeapon
{
    float projectileSpeed { get; set; }
    float projectileSpeedCoof { get; set; }
}
public interface IPrefabShot
{
    int idPrefabShot{ get; set; }
}
public interface IManaCost
{
    public int manaCost { get; set; }
}

[Serializable]
public abstract class Weapon : Item, IArtifact
{
    public bool rangeType { get; set; }
    public float range { get; set; }

    public damageT typeDamage;
    public int damage { get; set; }
    public float attackSpeedCoof { get; set; }
    public int addAttackSpeed {  get; set; }
    public int conut_Projectiles { get; set; }
    public int effectID { get; set; }
    public int artifactLevel { get; set; }

    public Weapon(int id, string name, int maxCount, int spriteID, Quality quality,int cost, string decription, bool rangeType, float range, damageT typeDamage, int damage, float _attackSpeedCoof,  int _addAttackSpeed, int conut_Projectiles, int effectID = -1) :base(id, name, maxCount, spriteID, quality, cost, decription)
    {
        this.rangeType = rangeType;
        this.range = range;
        this.typeDamage = typeDamage;
        this.damage = damage;
        this.attackSpeedCoof = _attackSpeedCoof;
        this.addAttackSpeed = _addAttackSpeed;

        this.TypeItem = TypeItem.Weapon;
        this.conut_Projectiles = conut_Projectiles;
        this.effectID = effectID;
    }
}
[Serializable]
public class MeleWeapon : Weapon
{
    public MeleWeapon(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription, bool rangeType, float range, damageT typeDamage, int damage, float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles, int effectID = -1) : base(id, name, maxCount, spriteID, quality, cost, decription, rangeType, range, typeDamage, damage, _attackSpeedCoof, _addAttackSpeed, conut_Projectiles, effectID)
    {

    }
}
[Serializable]
public abstract class RangedWeapon : Weapon, IPrefabShot
{
    public float spreadAngle { get; set; }
    public int idPrefabShot { get; set; }

    protected RangedWeapon(
        int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription,
        bool rangeType, float range, damageT typeDamage, int damage,
        float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles,
        float _projectileSpeed, float _projectileSpeedCoof, float _spreadAngle, int _idPrefabShot,
        int effectID = -1
    ) : base(id, name, maxCount, spriteID, quality, cost, decription,
             rangeType, range, typeDamage, damage,
             _attackSpeedCoof, _addAttackSpeed, conut_Projectiles, effectID)
    {
        spreadAngle = _spreadAngle;
        idPrefabShot = _idPrefabShot;
    }
}
[Serializable]
public class Gun : RangedWeapon, IBulletWeapon
{
    public float projectileSpeed { get; set; }
    public float projectileSpeedCoof { get; set; }

    public Gun(
        int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription,
        bool rangeType, float range, damageT typeDamage, int damage,
        float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles,
        float _projectileSpeed, float _projectileSpeedCoof, float _spreadAngle, int _idBulletPref,
        int effectID = -1
    ) : base(
        id, name, maxCount, spriteID, quality, cost, decription,
        rangeType, range, typeDamage, damage,
        _attackSpeedCoof, _addAttackSpeed, conut_Projectiles,
        _projectileSpeed, _projectileSpeedCoof, _spreadAngle, _idBulletPref, effectID
    )
    {
        projectileSpeed = _projectileSpeed;
        projectileSpeedCoof = _projectileSpeedCoof;
    }
}

[Serializable]
public class LazerGun : RangedWeapon, ILaserWeapon
{
    public int CountPenetration { get; set; }

    public LazerGun(
        int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription,
        bool rangeType, float range, damageT typeDamage, int damage,
        float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles,
        float _spreadAngle, int idLazerPref, int CountPenetration,
        int effectID = -1
    ) : base(
        id, name, maxCount, spriteID, quality, cost, decription,
        rangeType, range, typeDamage, damage,
        _attackSpeedCoof, _addAttackSpeed, conut_Projectiles,
        0f, 0f, _spreadAngle, idLazerPref, effectID
    )
    {
        this.CountPenetration = CountPenetration;
    }
}
[Serializable]
public class StaffBullet : Gun, IManaCost
{
    public int manaCost { get; set; }

    public StaffBullet(
        int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription,
        bool rangeType, float range, damageT typeDamage, int damage,
        float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles,
        float _projectileSpeed, float _projectileSpeedCoof, float _spreadAngle, int _idBulletPref,
        int _manaCost, int effectID = -1
    ) : base(
        id, name, maxCount, spriteID, quality, cost, decription,
        rangeType, range, typeDamage, damage,
        _attackSpeedCoof, _addAttackSpeed, conut_Projectiles,
        _projectileSpeed, _projectileSpeedCoof, _spreadAngle, _idBulletPref, effectID
    )
    {
        manaCost = _manaCost;
    }
}
[Serializable]
public class StaffLazer : LazerGun, IManaCost
{
    public int manaCost { get; set; }

    public StaffLazer(
        int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription,
        bool rangeType, float range, damageT typeDamage, int damage,
        float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles,
        float _spreadAngle, int idLazerPref, int CountPenetration,
        int manaCost, int effectID = -1
    ) : base(
        id, name, maxCount, spriteID, quality, cost, decription,
        rangeType, range, typeDamage, damage,
        _attackSpeedCoof, _addAttackSpeed, conut_Projectiles,
        _spreadAngle, idLazerPref, CountPenetration, effectID
    )
    {
        this.manaCost = manaCost;
    }
}
[Serializable]
public class LazerStaffGun : StaffLazer
{
    public LazerStaffGun(
        int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription,
        bool rangeType, float range, damageT typeDamage, int damage,
        float _attackSpeedCoof, int _addAttackSpeed, int conut_Projectiles,
        float _spreadAngle, int idLazerPref, int CountPenetration,
        int manaCost, int effectID = -1
    ) : base(
        id, name, maxCount, spriteID, quality, cost, decription,
        rangeType, range, typeDamage, damage,
        _attackSpeedCoof, _addAttackSpeed, conut_Projectiles,
        _spreadAngle, idLazerPref, CountPenetration, manaCost, effectID
    )
    { }
}