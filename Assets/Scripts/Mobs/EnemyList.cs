using NUnit.Framework;
using System.Collections.Generic;
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
            mobs.Add(new MeleMob("Daizen", 10, 0.5f, false, 3, 60, 0.1f));
            mobs.Add(new RangeMob("Rainger", 5, 4f, true, 1, 30, 1.5f, bullet_Rainger, 10f));
        }
    }
    public void GetMobs(int id)
    {
        if(mobs.Count<1)
        {
            CreateMobs();
        }
    }
}
public class Mob
{
    public string Name { get; set; }
    public int Hp { get; set; }
    public float rangeAttack { get; set; }
    public bool isRanged { get; set; }
    public int damage { get; set; }
    public int attackSpeed { get; set; }
    public float speed { get; set; }

    public Mob(string _name, int _hp, float _rangeAt, bool _isranged, int _damage, int _attackspeed, float _speed)
    {
        Name = _name;
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

