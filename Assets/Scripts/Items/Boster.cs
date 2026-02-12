using System;
using UnityEngine;

public class Boster : Item, IUsable
{
    public AllStats boosterType;
    public int countBoost;
    public int soundID;
    public Boster(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, AllStats bosterType, int countBoost, int idSound = 0, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
    {
        this.boosterType = bosterType;
        this.countBoost = countBoost;
        soundID = idSound;
    }

    public virtual int GetSoundID()
    {
        return soundID;
    }
    public TypeSound GetTypeSound()
    {
        return TypeSound.Booster;
    }
    public virtual bool Use()
    {

        if (GlobalData.Player != null)
        {
            GlobalData.Player.AddAttribute(boosterType, countBoost);
            GlobalData.EqupmentPlayer.UpdateAllWeaponsStats();
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
