using UnityEngine;

public class Boster : Item, IUsable
{
    protected AllStats boosterType;
    protected int countBoost;
    private static int soundID = 3;
    public Boster(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, AllStats bosterType, int countBoost, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
    {
        this.boosterType = bosterType;
        this.countBoost = countBoost;
    }

    public virtual int GetSoundID()
    {
        return soundID;
    }

    public virtual bool Use()
    {
        GlobalData.Player.AddAttribute(boosterType, countBoost);
        if (GlobalData.Player != null) Debug.Log("Работает: " + countBoost);
        return true;
    }
}
//public enum BoosterType
//{
//    Strength,
//    Agillity,
//    Intelligence
//}
