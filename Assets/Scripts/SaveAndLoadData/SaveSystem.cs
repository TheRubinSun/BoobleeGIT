using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    private string savePath;

    public SaveSystem()
    {
        savePath = Path.Combine(Application.persistentDataPath, "weapon_save.json");
    }
    public void SaveWeapon(List<int> weaponIds)
    {
        List<WeaponSaveData> saveDataList = new List<WeaponSaveData>();

        // Преобразуем все ID оружия в объекты WeaponSaveData
        foreach (int id in weaponIds)
        {
            saveDataList.Add(new WeaponSaveData { weaponId = id });
        }
        string json = JsonUtility.ToJson(new WeaponSaveList { weapons = saveDataList}, true);

        File.WriteAllText(savePath, json);
        Debug.Log("Сохранено: " + json);
    }
}
[System.Serializable]
public class WeaponSaveData
{
    public int weaponId;
}
// Для сериализации списка
[System.Serializable]
public class WeaponSaveList
{
    public List<WeaponSaveData> weapons;
}