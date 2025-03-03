using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static Dictionary<int, GameObject> weapons = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> projectiles = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> mobsProjectiles = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> minions = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> traps = new Dictionary<int, GameObject>();
    public static void LoadWeapons()
    {
        weapons[0] = Resources.Load<GameObject>("Weapons/Sword_God_Pref");
        weapons[1] = Resources.Load<GameObject>("Weapons/Pistol_Mark_Pref");
        weapons[2] = Resources.Load<GameObject>("Weapons/Bow_simple");
        weapons[3] = Resources.Load<GameObject>("Weapons/ShotGun_pump");
        weapons[4] = Resources.Load<GameObject>("Weapons/Soldier's spear");
        weapons[5] = Resources.Load<GameObject>("Weapons/Simple_knife");

        
        projectiles[0] = Resources.Load<GameObject>("Projectiles/Pistol_Bullet");
        projectiles[1] = Resources.Load<GameObject>("Projectiles/Shotgun_Bullet");
        projectiles[2] = Resources.Load<GameObject>("Projectiles/Arrow");

        mobsProjectiles[0] = Resources.Load<GameObject>("Mob_Projectiles/BulletMobOne");
        mobsProjectiles[1] = Resources.Load<GameObject>("Mob_Projectiles/Slime_arrow");

        minions[0] = Resources.Load<GameObject>("Minions/Minion_Tech");
        minions[1] = Resources.Load<GameObject>("Minions/Minion_Mage");
        minions[2] = Resources.Load<GameObject>("Minions/Minion_Mixed");

        traps[0] = Resources.Load<GameObject>("Traps/Mine");

        if (weapons[0] == null)
            Debug.LogError("Не удалось загрузить префаб Sword_God_Pref!");
        if (weapons[1] == null)
            Debug.LogError("Не удалось загрузить префаб Pistol_Mark_Pref!");
    }

    public static GameObject GetWeaponPrefab(int id)
    {
        return weapons.ContainsKey(id) ? weapons[id] : null;
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
}
