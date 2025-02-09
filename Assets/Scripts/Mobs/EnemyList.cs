using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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
        CreateMobs();
    }
    public void CreateMobs()
    {
        mobs.Clear();
        if (mobs.Count < 1)
        {
            mobs.Add(new MeleMob("daizen_enem", 10, 0.75f, false, 2, 45, 1.5f));
            mobs.Add(new RangeMob("rainger_enem", 5, 4f, true, 1, 30, 1f, bullet_Rainger, 8f));

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

    public Mob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed)
    {
        NameKey = _name;
        Hp = _hp;
        rangeAttack = _rangeAt;
        isRanged = _isranged;
        damage = _damage;
        attackSpeed = _attackspeed;
        speed = _speed;
    }
    public virtual void Attack()
    {
        //Debug.Log("This mob attack");
    }

    //public string GetLocalizationName()
    //{
    //    if (LocalizationManager.Instance != null)
    //    {
    //        Dictionary<string, string> localizedName = LocalizationManager.Instance.GetLocalizedValue("mobs", NameKey);
    //        if (!string.IsNullOrEmpty(localizedName))
    //        {
    //            return localizedName;
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Локализация для ключа {NameKey} не найдена.");
    //            return NameKey; // Возвращаем ключ, если локализация отсутствует
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("LocalizationManager нет на сцене.");
    //        return NameKey; // Возвращаем ключ, если LocalizationManager не найден
    //    }
    //}
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
public class RangeMob : Mob
{
    public GameObject PrefabBullet;
    public float SpeedProjectile { get; set; }

    public RangeMob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed, GameObject prefabBullet, float speedProjectile) : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed)
    {
        this.PrefabBullet = prefabBullet;
        this.SpeedProjectile = speedProjectile;
    }
    public override void Attack()
    {
        //Debug.Log("Shoot");
    }
}
public class MeleMob : Mob
{
    public MeleMob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed) : base(_name, _hp, _rangeAt, _isranged, _damage, _attackspeed, _speed)
    {

    }
    public override void Attack()
    {
        //Debug.Log("Hit");
    }
}

