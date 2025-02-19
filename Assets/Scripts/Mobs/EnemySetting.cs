using System;
using System.Collections;
using UnityEngine;

public class EnemySetting : MonoBehaviour
{
    public static event Action<EnemySetting> OnEnemyDeath;

    public int IdMobs;
    private Mob mob;
    public string Name;
    public int cur_Hp;
    public int max_Hp;
    public int armor_Hp;

    public float attackRange;
    public bool isRanged;
    public int damage;
    public int attackSpeed;
    public float attackInterval;

    public float speed;
    public int GiveExp;
    public GameObject bulletPrefab { get; set; }
    public float speedProjectile;

    SpriteRenderer sr;
    private bool IsDead;
    private void Start()
    {
        LoadParametrs();
        sr = GetComponent<SpriteRenderer>();
    }
    void LoadParametrs()
    {
        mob = EnemyList.Instance.mobs[IdMobs];
        Name = mob.NameKey;
        max_Hp = mob.Hp;
        cur_Hp = max_Hp;
        attackRange = mob.rangeAttack;
        speed = mob.speed;
        isRanged = mob.isRanged;
        damage = mob.damage;
        attackSpeed = mob.attackSpeed;
        attackInterval  = 60f / attackSpeed;
        speed = mob.speed;
        GiveExp = mob.GiveExp;
        if (isRanged && mob is RangeMob rangeMob)
        {
            bulletPrefab = WeaponDatabase.GetProjectilesPrefab(1);
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
    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
        cur_Hp -= (int)(Mathf.Max(damage / (1 + armor_Hp / 10f), 1));
        if (cur_Hp <= 0)
        {
            Death();
        }
    }
    private IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (sr != null)
        {
            sr.color = color;
            yield return new WaitForSeconds(time);
            sr.color = new Color32(255, 255, 255, 255);
        }
    }

    private void Death()
    {
        if (IsDead) return;
        IsDead = true;

        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);

    }
}
