using UnityEngine;

public class Hook : Item, IUsable
{
    public bool Spent { get; set; } = false;
    public float range;
    public float speedHook;
    public float speedMove;
    public int idPrefab;
    public Hook(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, float _range,float _speedHook,float _speedMove,int _idPrefab, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
    {
        range = _range;
        speedHook = _speedHook;
        speedMove = _speedMove;
        idPrefab = _idPrefab;
    }

    public int GetSoundID()
    {
        return 0;
    }

    public TypeSound GetTypeSound()
    {
        return TypeSound.Effects;
    }

    public bool Use()
    {
        GlobalData.PlayerControl.CheckHook(this, LayerManager.allToughObj);
        Debug.Log("Использую хук");
        return true;
    }
}
