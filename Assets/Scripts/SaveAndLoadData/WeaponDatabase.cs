using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static Dictionary<int, GameObject> weapons = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> projectiles = new Dictionary<int, GameObject>();
    public static void LoadWeapons()
    {
        weapons[0] = Resources.Load<GameObject>("Weapons/Sword_God_Pref");
        weapons[1] = Resources.Load<GameObject>("Weapons/Pistol_Mark_Pref");

        projectiles[0] = Resources.Load<GameObject>("Projectiles/Bullet");
        projectiles[1] = Resources.Load<GameObject>("Projectiles/BulletMobOne");

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
}
