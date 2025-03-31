using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GlobalPrefabs
{

    public static Dictionary<string, GameObject> PrefabDict = new Dictionary<string, GameObject>();
    public static GameObject ItemDropPref { get; private set; }
    public static GameObject ListSlotPref { get; private set; }
    public static GameObject SlotPref { get; private set; }
    public static GameObject SlotMaterial { get; private set; }

    public static async Task<bool> LoadPrefabs()
    {
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(
            "InventoryPrefabs",
            obj => { PrefabDict[obj.name] = obj;}
            );
        await handle.Task;

        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            ItemDropPref = GetPrefab("ItemDrop");
            ListSlotPref = GetPrefab("ListSlot");
            SlotPref = GetPrefab("Slot");
            SlotMaterial = GetPrefab("SlotMaterial");

            if (ItemDropPref == null) Debug.LogError("ItemDropPref is NULL!");
            if (ListSlotPref == null) Debug.LogError("ListSlotPref is NULL!");
            if (SlotPref == null) Debug.LogError("Slot is NULL!");
            if (SlotMaterial == null) Debug.LogError("SlotMaterial is NULL!");

            return true;
        }
        return false;
    }
    private static GameObject GetPrefab(string name)
    {
        return PrefabDict.TryGetValue(name, out var prefab) ? prefab : null;
    }
}
