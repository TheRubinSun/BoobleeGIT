using UnityEngine;

public class Minion : Item
{
    public float range_search;
    public Minion(int id, string name, int maxCount, int spriteID, Quality quality,int cost, string decription, TypeItem typeItem, float range) : base(id, name, maxCount, spriteID, quality, cost, decription, typeItem)
    {
        range_search = range;
    }
}
