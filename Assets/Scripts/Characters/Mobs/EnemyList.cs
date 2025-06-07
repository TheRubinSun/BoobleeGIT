using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

public enum TypeMob
{
    None,
    Magic,
    Technology,
    Mixed
}
public enum AllEnemy
{
    Daizen,
    Rainger,
    Slime,
    Slime_boss,
    Mimic
}

public static class EnemyList 
{
    //public static EnemyList Instance {  get; private set; }
    public static List<Mob> mobs = new List<Mob>();

    //[SerializeField] GameObject bullet_Rainger;
    //private void Awake()
    //{
    //    // Проверка на существование другого экземпляра
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        Instance = this;

    //        //if (gameObject.scene.name != "DontDestroyOnLoad")
    //        //{
    //        //    DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
    //        //}
    //    }

    //}

    public static void LoadOrCreateMobsList(List<Mob> mobslist)
    {
        if (mobslist != null && mobslist.Count > 0)
        {
            mobs = mobslist;
        }
        else
        {
            CreateMobs();
        }
    }
    public static void CreateMobs()
    {
        mobs.Clear();
        if (mobs.Count < 1)
        {
            //             name hp range isRange damage attackSpeed speed
            mobs.Add(new DaizenMob("daizen_enem", 8, 1.5f, false, 2, 45, 1.2f, 4, TypeMob.Technology, 1, 0, 0.05f));
            mobs.Add(new RangerMob("rainger_enem", 4, 6f, true, 1, 30, 1f, 10f, 0, 6, TypeMob.Technology, 1, 0, 0.05f));
            mobs.Add(new Slime("slime_enem", 6, 3f, true, 1, 20, 1f, 3.5f, 1, 5, TypeMob.Magic, 3, 1, 0.05f, 0));
            mobs.Add(new Slime("slime_boss_enem", 50, 5f, true, 2, 30, 1.6f, 6f, 1, 50, TypeMob.Magic, 4, 1, 0.1f, 0));
            mobs.Add(new Mimic("mimic_enem", 12, 2f, false, 3, 25, 2f, 50, TypeMob.Magic, 3, 0.15f, 0f));
            mobs.Add(new Car("death_car_enem", 40, 1.7f, false, 2, 40, 2.5f, 100, TypeMob.Technology, 3, 0f, 0.15f));
            mobs.Add(new TastyFly("tasty_fly_enem", 9, 4f, false, 2, 30, 1.7f, 30, 12f, TypeMob.Magic, 3, 0.15f, 0f));
            mobs.Add(new Bur("bur_enem", 20, 2f, false, 4, 15, 1.5f, 45, 4f, TypeMob.Technology, 3, 0f, 0.15f));
            //DisplayMobsList.Instance.DisplayLinesMobs(mobs);
            //CreatePortalUI.Instance.DisplayLinesMobs(mobs);
        }
    }
    public static void LocalizaitedMobs()
    {
        foreach (var mob in mobs)
        {
            mob.LocalizationMobs();
        }
    }
    public static TypeMob GetTypeMob(string NameKey)
    {
        foreach(Mob mob in mobs)
        {
            if (mob.NameKey == NameKey) return mob.TypeMob;
        }
        Debug.LogError($"Ошибка! Не найден моб с ключом {NameKey}");
        return TypeMob.None;
    }
    public static void GetMobs(int id)
    {
        if(mobs.Count<1)
        {
            CreateMobs();
        }
    }
    public static Mob GetMobByName(string Name)
    {
        foreach (Mob mob in mobs)
        {
            if (mob.Name == Name) return mob;
        }
        return null;
    }
    public static int GetIdByMob(Mob mob)
    {
        for (int i = 0; i < mobs.Count; i++)
        {
            if (mob == mobs[i]) return i;
        }
        return 0;
    }
    //public static Item GetItemForId(int id) => itemById.TryGetValue(id, out Item item) ? item : items[0];

}
[Serializable]
public class Mob
{
    public string NameKey { get; set; }
    public string Name;
    public int Hp { get; set; }
    public int Armor { get; set; }
    public float Mag_Resis {  get; set; }
    public float Tech_Resis { get; set; }
    public float rangeAttack { get; set; }
    public bool isRanged { get; set; }
    public int damage { get; set; }
    public int attackSpeed { get; set; }
    public float speed { get; set; }
    public string Description { get; set; }
    public int GiveExp { get; set; }

    public TypeMob TypeMob { get; set; }
    public Mob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, TypeMob typeMob, 
        int _Armor, float _Mag_Resis, float _Tech_Resis)
    {
        NameKey = _name;
        Hp = _hp;
        rangeAttack = _rangeAt;
        isRanged = _isranged;
        damage = _damage;
        attackSpeed = _attackspeed;
        speed = _speed;
        GiveExp = giveExp;
        TypeMob = typeMob;
        Armor = _Armor;
        Mag_Resis = _Mag_Resis;
        Tech_Resis = _Tech_Resis;
    }
    public virtual void Attack()
    {
        //Debug.Log("This mob attack");
    }
    public void LocalizationMobs()
    {
        if (LocalizationManager.Instance != null)
        {
            Dictionary<string, string> localized = LocalizationManager.Instance.GetLocalizedValue("mobs", NameKey);
            if (localized != null)
            {
                Name = localized["Name"];
                Description = localized["Description"];
            }
            else
            {
                Debug.LogWarning($"Локализация для ключа {NameKey} не найдена.");
            }
        }
        else
        {
            Debug.LogWarning("LocalizationManager нет на сцене.");
        }
    }
}
[Serializable]
public class RangerMob : Mob
{
    public float SpeedProjectile { get; set; }
    public int idProj {  get; set; }
    public RangerMob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, float speedProjectile, int _idProj, int giveExp, TypeMob typeMob,
        int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0) 
        : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {
        this.SpeedProjectile = speedProjectile;
        idProj = _idProj;
    }
}
[Serializable]
public class DaizenMob : Mob
{
    public DaizenMob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, TypeMob typeMob,
        int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0) 
        : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {

    }
}
[Serializable]
public class Slime : Mob
{
    public float SpeedProjectile { get; set; }
    public int idProj { get; set; }
    public int idPosion {  get; set; }
    public Slime(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, float speedProjectile, int _idProj, int giveExp, TypeMob typeMob,
        int _idPosion, int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0)
        : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {
        this.SpeedProjectile = speedProjectile;
        idProj = _idProj;
        idPosion = _idPosion;
    }
}
[Serializable]
public class Mimic : Mob
{
    public Mimic(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, TypeMob typeMob,
    int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0)
    : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {

    }
}
[Serializable]
public class Car : Mob
{
    public Car(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, TypeMob typeMob,
    int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0)
    : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {

    }
}
[Serializable]
public class TastyFly : Mob
{
    public float attack_move_speed {  get; set; }
    public TastyFly(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, float _attack_move_speed, TypeMob typeMob,
    int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0)
    : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {
        attack_move_speed = _attack_move_speed;
    }
}
[Serializable]
public class Bur : Mob
{
    public float attack_move_speed { get; set; }
    public Bur(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, float _attack_move_speed, TypeMob typeMob,
    int _Armor = 0, float _Mag_Resis = 0, float _Tech_Resis = 0)
    : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob, _Armor, _Mag_Resis, _Tech_Resis)
    {
        attack_move_speed = _attack_move_speed;
    }
}

