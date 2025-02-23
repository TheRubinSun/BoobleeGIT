using UnityEngine;

public class Potion : Item, IUsable
{
    public Potion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description) : base(id,name,maxCount,spriteID,quality,cost,description,TypeItem.Potion,true)
    {

    }
    public virtual bool Use()
    {
        return false;
    }
}
public class HealPotion : Potion
{
    int countHeal;
    public HealPotion(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description,int _countHeal) : base(id, name, maxCount, spriteID, quality, cost, description)
    {
        countHeal = _countHeal;
    }
    public override bool Use()
    {
        if(Player.Instance.PlayerHeal(countHeal))
        {
            Debug.Log("Пытаюсь отхилить");
            return true;
        }
        else
        {
            Debug.Log("Немогу отхилить");
            return false;
        }
    }
}