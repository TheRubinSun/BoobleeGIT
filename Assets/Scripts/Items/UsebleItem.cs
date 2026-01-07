

public class UsebleItem : Item, IUsable
{

    public float value;
    public UsebleItem(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, float value, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, typeItem, isUse)
    {
        this.value = value;
    }

    public virtual int GetSoundID()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool Use()
    {
        throw new System.NotImplementedException();
    }
}
public class ForseItem : UsebleItem
{
    public static int soundID = 5;
    public ForseItem(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, float value, TypeItem typeItem = TypeItem.Other, bool isUse = false) : base(id, name, maxCount, spriteID, quality, cost, description, value, typeItem, isUse)
    {

    }

    public override int GetSoundID()
    {
        return soundID;
    }
    public override bool Use()
    {
        if (GlobalData.Player.ForcePlayer(value))
        {
            return true;
        }
        else
            return false;
        
    }
}
