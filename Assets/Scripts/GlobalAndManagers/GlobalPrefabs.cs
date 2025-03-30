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
    public static GameObject BuySlotPref { get; private set; }
    public static GameObject CraftSlotPref { get; private set; }
    public static GameObject ISlotPref { get; private set; }
    public static GameObject ListSlotPref { get; private set; }
    public static GameObject MaterialSlotPref { get; private set; }
    public static GameObject SellSlotPref { get; private set; }
    public static GameObject ShopSlotPref { get; private set; }
    public static GameObject TempSlotPref { get; private set; }

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
            BuySlotPref = GetPrefab("BuySlot");
            CraftSlotPref = GetPrefab("CraftSlot");
            ISlotPref = GetPrefab("ISlot"); ;
            ListSlotPref = GetPrefab("ListSlot");
            MaterialSlotPref = GetPrefab("MaterialSlot");
            SellSlotPref = GetPrefab("SellSlot");
            ShopSlotPref = GetPrefab("ShopSlot");
            TempSlotPref = GetPrefab("TempSlot");

            if (ItemDropPref == null) Debug.LogError("ItemDropPref is NULL!");
            if (BuySlotPref == null) Debug.LogError("BuySlotPref is NULL!");
            if (CraftSlotPref == null) Debug.LogError("CraftSlotPref is NULL!");
            if (ISlotPref == null) Debug.LogError("ISlotPref is NULL!");
            if (ListSlotPref == null) Debug.LogError("ListSlotPref is NULL!");
            if (MaterialSlotPref == null) Debug.LogError("MaterialSlotPref is NULL!");
            if (SellSlotPref == null) Debug.LogError("SellSlotPref is NULL!");
            if (ShopSlotPref == null) Debug.LogError("ShopSlotPref is NULL!");
            if (TempSlotPref == null) Debug.LogError("TempSlotPref is NULL!");

            return true;
        }
        return false;
    }
    private static GameObject GetPrefab(string name)
    {
        return PrefabDict.TryGetValue(name, out var prefab) ? prefab : null;
    }
}
