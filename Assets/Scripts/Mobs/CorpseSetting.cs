using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class CorpseSetting : MonoBehaviour
{
    public bool isBusy { get; private set; }
    public string NameKey { get; set; }
    public void Start()
    {
        isBusy = false;
    }
    public void Busy()
    {
        isBusy = true;
    }
    public List<Slot> GetDrop()
    {
        Debug.Log($"труп: {NameKey}");
        return ChanceAllDrop();
    }
    private List<Slot> ChanceAllDrop()
    {
        string[] nameKeysItem = ItemDropEnemy.enemyAndHisDrop[NameKey];
        float dropChance;
        List<Slot> dropItems = new List<Slot>();
        foreach (string nameItem in nameKeysItem)
        {
            Item item = ItemsList.Instance.GetItemForName(nameItem);
            dropChance = CalculatingItemDrop(item) / 100f;
            Debug.Log($"{item.NameKey}:  Шанс:{dropChance*100}%");
            if (Random.value < dropChance)
            {
                dropItems.Add(new Slot(item, 1));
                Debug.Log($"Выпал предмет: {item.NameKey}");
            }
            
        }
        return dropItems;
    }
    private float CalculatingItemDrop(Item item)
    {
        float totalChance = 0;
        Debug.LogWarning($"TypeMob: {EnemyList.Instance.GetTypeMob(NameKey)} mob:{NameKey}");
        switch (EnemyList.Instance.GetTypeMob(NameKey))
        {
            case TypeMob.Technology:
                {
                    totalChance = MultiplyChanceRared(Player.Instance.TechniquePoints, item.quality);
                    break;
                }
            case TypeMob.Magic:
                {
                    totalChance = MultiplyChanceRared(Player.Instance.MagicPoints, item.quality);
                    break;
                }
            default:
                {
                    break;
                }

        }
        return totalChance;
    }
    //Player.Instance.TechniquePoints
    private float MultiplyChanceRared(int countTypePoint, Quality quality)
    {
        float multiplyChance = 0;
        switch(quality)
        {
            case Quality.Common:
                {
                    multiplyChance = 40 * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Uncommon:
                {
                    multiplyChance = 15 * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Rare:
                {
                    multiplyChance = 4 * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Mystical:
                {
                    multiplyChance = 1 * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Legendary:
                {
                    multiplyChance = 0.2f * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Interverse:
                {
                    multiplyChance = 0.05f * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
                default:
                {
                    break;
                }
        }
        return multiplyChance;
    }
    private float MultiplyChancePlayerPoint(int countTypePoint)
    {
        float chance = 0;
        if(countTypePoint > 0)
        {
            chance = 1 + ((float)countTypePoint * 0.1f);
        }
        return chance;
    }


}
