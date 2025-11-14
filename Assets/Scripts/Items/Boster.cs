using UnityEngine;

public class Boster : Item, IUsable
{
    protected BoosterType boosterType;
    protected int countBoost;
    private static int soundID = 3;
    public Boster(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, BoosterType bosterType, int countBoost, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
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
        Player.Instance.AddAttribute(boosterType, countBoost);
        return true;
    }
}
public enum BoosterType
{
    Strength,
    Agillity,
    Intelligence
}
