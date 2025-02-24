using UnityEngine;

public class Trap : Item, IUsable
{
    public int id_trap_prefab;
    public Trap(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _id_trap_prefab) : base(id, name, maxCount, spriteID, quality, cost, description, TypeItem.Trap, true)
    {

    }
    public virtual bool Use()
    {
        return false;
    }
}
public class Mine: Trap
{
    public int damageMine;
    public damageT damageT;
    public float radiusExp;
    public float delayTime;
    
    private GameObject pref_mine;
    public Mine(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _id_trap_prefab, int _damageMine, damageT _damageT, float _radiusExp, float _delayTime) : base(id, name, maxCount, spriteID, quality, cost, description, _id_trap_prefab)
    {
        damageMine = _damageMine;
        damageT = _damageT;
        radiusExp = _radiusExp;
        delayTime = _delayTime;
        LoadParamToObj();
    }
    public override bool Use()
    {
        if(pref_mine != null)
        { 
            GameObject mine = Player.Instance.CreateTrap(pref_mine);
            mine.GetComponent<MineLogic>().SetParameters(damageMine, damageT, radiusExp, delayTime);
            return true;
        }
        return false;
    }
    public void LoadParamToObj()
    {
        pref_mine = WeaponDatabase.GetTrapPrefab(id_trap_prefab);
    }
}
