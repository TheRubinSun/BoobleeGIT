using System;
using UnityEngine;

public class Boster : Item, IUsable
{
    public AllStats boosterType;
    public int countBoost;
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

        if (GlobalData.Player != null)
        {
            GlobalData.Player.AddAttribute(boosterType, countBoost);
            Debug.Log("Работает: " + countBoost);
            return true;
        }
        else
        {
            Debug.LogWarning("Не найден игрок");
            return false;
        }

    }
}
//public enum BoosterType
//{
//    Strength,
//    Agillity,
//    Intelligence
//}
