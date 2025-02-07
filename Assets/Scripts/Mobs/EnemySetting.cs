using UnityEngine;

public class EnemySetting : MonoBehaviour
{
    public int IdMobs;
    private Mob mob;
    public string Name;
    public int hp;
    public float attackRange;
    public bool isRanged;
    public int damage;
    public int attackSpeed;
    public float attackInterval;
    public float speed;
    public GameObject bulletPrefab { get; set; }
    public float speedProjectile;
    private void Start()
    {
        LoadParametrs();
    }
    void LoadParametrs()
    {
        mob = EnemyList.Instance.mobs[IdMobs];
        name = mob.Name;
        hp = mob.Hp;
        attackRange = mob.rangeAttack;
        speed = mob.speed;
        isRanged = mob.isRanged;
        damage = mob.damage;
        attackSpeed = mob.attackSpeed;
        attackInterval  = 60f / attackSpeed;
        speed = mob.speed;
        if(isRanged && mob is RangeMob rangeMob)
        {
            bulletPrefab = rangeMob.PrefabBullet;
            speedProjectile = rangeMob.SpeedProjectile;
        }

    }
    private void Update()
    {
        if(mob != null)
        {
            mob.Attack();
        }
    }
}
