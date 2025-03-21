using UnityEngine;

public class Trap : Item, IUsable
{
    public int id_trap_prefab;
    protected GameObject prefab_trap;
    public Trap(int id, string name, int maxCount, int spriteID, Quality quality, int cost, string description, int _id_trap_prefab) : base(id, name, maxCount, spriteID, quality, cost, description, TypeItem.Trap, true)
    {

    }
    public virtual bool Use()
    {
        return false;
    }
    public virtual void LoadParamToObj()
    {
        prefab_trap = ResourcesData.GetTrapPrefab(id_trap_prefab);
    }
    public virtual int GetSoundID() => 0;
}
public class Mine: Trap
{
    public int damageMine;
    public damageT damageT;
    public float radiusExp;
    public float delayTime;
    private static int soundID = 3;

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
        if (prefab_trap == null) LoadParamToObj();
        if (prefab_trap == null) //Если не изменилось
        {
            Debug.Log("Ошибка, отсутствует префаб");
            return false;
        }
        GameObject mine = Player.Instance.CreateTrap(prefab_trap);
        mine.GetComponent<MineLogic>().SetParameters(damageMine, damageT, radiusExp, delayTime);
        return true;

    }
    public override int GetSoundID()
    {
        return soundID;
    }
}
