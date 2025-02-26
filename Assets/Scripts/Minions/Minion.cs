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
