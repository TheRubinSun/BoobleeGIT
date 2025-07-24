using System.Collections.Generic;
using UnityEngine;

public class ResourcesData : MonoBehaviour 
{
    public static Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();
    public static Dictionary<int, GameObject> projectiles = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> mobsProjectiles = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> minions = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> traps = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> portals = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> flowers_seed = new Dictionary<int, GameObject>();
    public static Dictionary<int, EffectData> effects = new Dictionary<int, EffectData>();
    public static void LoadWeapons()
    {
        weapons["sword_gods_slayer"] = Resources.Load<GameObject>("Weapons/Sword_God_Pref");
        weapons["axe_woodcutter"] = Resources.Load<GameObject>("Weapons/Axe_woodcut_Pref");
        weapons["soldier_spear"] = Resources.Load<GameObject>("Weapons/Soldier's spear");
        weapons["simple_knife"] = Resources.Load<GameObject>("Weapons/Simple_knife");
        weapons["pickaxe_simple"] = Resources.Load<GameObject>("Weapons/Pickaxe_simple_pref");
        weapons["bur_t0k6"] = Resources.Load<GameObject>("Weapons/Bur_simple_pref");

        weapons["gun_makarov"] = Resources.Load<GameObject>("Weapons/Pistol_Mark_Pref");
        weapons["bow_simple"] = Resources.Load<GameObject>("Weapons/Bow_simple");
        weapons["shotgun_pump"] = Resources.Load<GameObject>("Weapons/ShotGun_pump");
        weapons["staff_forest"] = Resources.Load<GameObject>("Weapons/Staff_forest");
        weapons["lazergun_tra"] = Resources.Load<GameObject>("Weapons/LazerGun");
        weapons["thunder_gun"] = Resources.Load<GameObject>("Weapons/ThunderGun");

        projectiles[0] = Resources.Load<GameObject>("Projectiles/Pistol_Bullet");
        projectiles[1] = Resources.Load<GameObject>("Projectiles/Shotgun_Bullet");
        projectiles[2] = Resources.Load<GameObject>("Projectiles/Arrow");
        projectiles[3] = Resources.Load<GameObject>("Projectiles/Forest_mag");
        projectiles[4] = Resources.Load<GameObject>("Projectiles/GunMinion");
        projectiles[5] = Resources.Load<GameObject>("Projectiles/HealMin");
        
        mobsProjectiles[0] = Resources.Load<GameObject>("Mob_Projectiles/BulletMobOne");
        mobsProjectiles[1] = Resources.Load<GameObject>("Mob_Projectiles/Slime_arrow");

        minions[0] = Resources.Load<GameObject>("Minions/Minion_Tech");
        minions[1] = Resources.Load<GameObject>("Minions/Minion_Mage");
        //minions[2] = Resources.Load<GameObject>("Minions/Minion_Mixed");
        minions[2] = Resources.Load<GameObject>("Minions/Minion_GunTech");
        minions[3] = Resources.Load<GameObject>("Minions/Minion_HealMag");
        
        traps[0] = Resources.Load<GameObject>("Traps/Mine");
        traps[1] = Resources.Load<GameObject>("Traps/FootTrap");

        portals[0] = Resources.Load<GameObject>("Portals/Portal_Tech");
        portals[1] = Resources.Load<GameObject>("Portals/Portal_Mages");

        flowers_seed[0] = Resources.Load<GameObject>("Objects/Flowers/Sunflower_seed");
        flowers_seed[1] = Resources.Load<GameObject>("Objects/Flowers/Tolania_seed");
        flowers_seed[2] = Resources.Load<GameObject>("Objects/Flowers/Tall_seed");

        effects[0] = Resources.Load<EffectData>("Effects/Heal");
        effects[1] = Resources.Load<EffectData>("Effects/Posion_Pistol");
        effects[2] = Resources.Load<EffectData>("Effects/SpeedUp");
        effects[3] = Resources.Load<EffectData>("Effects/Posion_SmallSlime");
        effects[4] = Resources.Load<EffectData>("Effects/Posion_BossSlime");
        effects[5] = Resources.Load<EffectData>("Effects/Trapped");
        effects[6] = Resources.Load<EffectData>("Effects/TrappedMin");


        if (weapons["sword_gods_slayer"] == null)
            Debug.LogError("Не удалось загрузить префаб Sword_God_Pref!");
        if (weapons["gun_makarov"] == null)
            Debug.LogError("Не удалось загрузить префаб Pistol_Mark_Pref!");
    }

    public static GameObject GetWeaponPrefab(string nameKey)
    {
        return weapons.ContainsKey(nameKey) ? weapons[nameKey] : null;
    }
    public static GameObject GetProjectilesPrefab(int id)
    {
        return projectiles.ContainsKey(id) ? projectiles[id] : null;
    }
    public static GameObject GetMobProjectilesPrefab(int id)
    {
        return mobsProjectiles.ContainsKey(id) ? mobsProjectiles[id] : null;
    }
    public static GameObject GetMinionPrefab(int id)
    {
        return minions.ContainsKey(id) ? minions[id] : null;
    }
    public static GameObject GetTrapPrefab(int id)
    {
        return traps.ContainsKey(id) ? traps[id] : null;
    }
    public static GameObject GetPortalPrefab(int id)
    {
        return portals.ContainsKey(id) ? portals[id] : null;
    }
    public static GameObject GetFlowerSeedPrefab(int id)
    {
        return flowers_seed.ContainsKey(id) ? flowers_seed[id] : null;
    }
    public static EffectData GetEffectsPrefab(int id)
    {
        return effects.ContainsKey(id) ? effects[id] : null;
    }
}
