using System.IO;
using UnityEngine;

public class LoadSystem
{
    //private string savePath;

    //public LoadSystem()
    //{
    //    savePath = Path.Combine(Application.persistentDataPath, "weapon_save.json");
    //    ResourcesData.LoadWeapons(); // Загружаем префабы при старте
    //}
    //public GameObject LoadWeapon()
    //{
    //    if (!File.Exists(savePath))
    //    {
    //        Debug.LogWarning("Файл сохранения не найден!");
    //        return null;
    //    }
    //    string json = File.ReadAllText(savePath);
    //    WeaponSaveData loadedData = JsonUtility.FromJson<WeaponSaveData>(json);

    //    return ResourcesData.GetWeaponPrefab(loadedData.weaponId);
    //}
}
