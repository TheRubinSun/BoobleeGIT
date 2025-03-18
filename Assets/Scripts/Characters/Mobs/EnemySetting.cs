using System;
using System.Collections;
using UnityEngine;

public class EnemySetting : MonoBehaviour
{
    //public static event Action<EnemySetting> OnEnemyDeath;

    //public int IdMobs;
    //private Mob mob;
    //public string Name;
    //public int cur_Hp { get; private set; }
    //public int max_Hp { get; private set; }
    //public int armor_Hp { get; private set; }

    //public float attackRange { get; private set; }
    //public bool isRanged { get; private set; }
    //public int damage { get; private set; }
    //public int attackSpeed { get; private set; }
    //public float attackInterval { get; private set; }

    //public float speed { get; private set; }
    //public int GiveExp { get; private set; }
    //public GameObject bulletPrefab { get; private set; }
    //public float speedProjectile { get; private set; }


    //private bool IsDead;
    //[SerializeField] private Transform childObj;
    //private Color32 originalColor;

    //private SpriteRenderer sr;
    //private SpriteRenderer sr_child;
    //private void Start()
    //{
    //    LoadParametrs();
    //    sr = GetComponent<SpriteRenderer>();
    //    if (childObj != null) sr_child = childObj.GetComponent<SpriteRenderer>();
    //}
    //void LoadParametrs()
    //{
    //    mob = EnemyList.Instance.mobs[IdMobs];
    //    Name = mob.NameKey;
    //    max_Hp = mob.Hp;
    //    cur_Hp = max_Hp;
    //    attackRange = mob.rangeAttack;
    //    speed = mob.speed;
    //    isRanged = mob.isRanged;
    //    damage = mob.damage;
    //    attackSpeed = mob.attackSpeed;
    //    attackInterval  = 60f / attackSpeed;
    //    speed = mob.speed;
    //    GiveExp = mob.GiveExp;
    //    if (isRanged && mob is RangerMob rangeMob)
    //    {
    //        bulletPrefab = WeaponDatabase.GetProjectilesPrefab(1);
    //        speedProjectile = rangeMob.SpeedProjectile;
    //    }

    //}
    //private void Update()
    //{
    //    if(mob != null)
    //    {
    //        mob.Attack();
    //    }
    //}
    //public void TakeDamage(int damage)
    //{
    //    StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
    //    cur_Hp -= (int)(Mathf.Max(damage / (1 + armor_Hp / 10f), 1));
    //    if (cur_Hp <= 0)
    //    {
    //        Death();
    //    }
    //}
    //private IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    //{
    //    if (sr != null)
    //    {
    //        sr.color = color;
    //        if (sr_child != null) sr_child.color = color;


    //        yield return new WaitForSeconds(time);

    //        sr.color = new Color32(255, 255, 255, 255);
    //        if (sr_child != null) sr_child.color = new Color32(255, 255, 255, 255);
    //    }
    //}

    //private void Death()
    //{
    //    if (IsDead) return;
    //    IsDead = true;

    //    OnEnemyDeath?.Invoke(this);
    //    Destroy(gameObject);

    //}
}
