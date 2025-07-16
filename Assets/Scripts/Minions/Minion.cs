using UnityEngine;

public class Minion : Item
{
    public float radius_search;
    public float time_red;
    public float move_speed;
    public TypeMob typeMob;
    public Minion(int id, string name, int maxCount, int spriteID, Quality quality,int cost, string decription, TypeItem typeItem, float radius,float timeRed , float speed, TypeMob type) : base(id, name, maxCount, spriteID, quality, cost, decription, typeItem)
    {
        radius_search = radius;
        time_red = timeRed;
        move_speed = speed;
        typeMob = type;
    }
}
public class GunMinion: Minion
{
    public int bulletId;
    public int effectId;
    public int damage;
    public float speedProj;

    public GunMinion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string decription, TypeItem typeItem, float radius, float timeRed, float speed, TypeMob type, int idBullet, int effectId, int damage, float speedProj) : base(id, name, maxCount, spriteID, quality, cost, decription, typeItem, radius, timeRed, speed, type)
    {
        bulletId = idBullet;
        this.effectId = effectId;
        this.damage = damage;
        this.speedProj = speedProj;
    }
}
