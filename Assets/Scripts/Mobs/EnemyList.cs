using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    Adjacent
}
public class EnemyList: MonoBehaviour 
{
    public static EnemyList Instance {  get; private set; }
    public List<Mob> mobs = new List<Mob>();
    [SerializeField] GameObject bullet_Rainger;
    private void Awake()
    {
        // Проверка на существование другого экземпляра
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }
        
    }
    public void LoadOrCreateMobsList(List<Mob> mobslist)
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
    public void CreateMobs()
    {
        mobs.Clear();
        if (mobs.Count < 1)
        {
            //             name hp range isRange damage attackSpeed speed
            mobs.Add(new DaizenMob("daizen_enem", 15, 1.2f, false, 2, 45, 2f, 2, TypeMob.Technology));
            mobs.Add(new RangerMob("rainger_enem", 10, 6f, true, 1, 30, 1.2f, 10f, 3, TypeMob.Technology));

            DisplayMobsList.Instance.DisplayLinesMobs(mobs);
        }
    }
    public void LocalizaitedMobs()
    {
        foreach (var mob in mobs)
        {
            mob.LocalizationMobs();
        }
    }
    public TypeMob GetTypeMob(string NameKey)
    {
        foreach(Mob mob in mobs)
        {
            if (mob.NameKey == NameKey) return mob.TypeMob;
        }
        Debug.LogError($"Ошибка! Не найден моб с ключом {NameKey}");
        return TypeMob.None;
    }
    public void GetMobs(int id)
    {
        if(mobs.Count<1)
        {
            CreateMobs();
        }
    }
    public Mob GetMobByName(string Name)
    {
        foreach(Mob mob in mobs)
        {
            if(mob.Name == Name) return mob;
        }
        return null;
    }
    public int GetIdByMob(Mob mob)
    {
        for (int i = 0; i < mobs.Count; i++)
        {
            if(mob == mobs[i]) return i;
        }
        return 0;
    }
}
[Serializable]
public class Mob
{
    public string NameKey { get; set; }
    public string Name;
    public int Hp { get; set; }
    public float rangeAttack { get; set; }
    public bool isRanged { get; set; }
    public int damage { get; set; }
    public int attackSpeed { get; set; }
    public float speed { get; set; }
    public string Description { get; set; }
    public int GiveExp { get; set; }

    public TypeMob  TypeMob{get;set;}
    public Mob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, TypeMob typeMob)
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

    public RangerMob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, float speedProjectile, int giveExp, TypeMob typeMob) : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob)
    {
        this.SpeedProjectile = speedProjectile;
    }
    public override void Attack()
    {
        //Debug.Log("Shoot");
    }
}
[Serializable]
public class DaizenMob : Mob
{
    public DaizenMob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, int giveExp, TypeMob typeMob) : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed, giveExp, typeMob)
    {

    }
    public override void Attack()
    {
        //Debug.Log("Hit");
    }
}
