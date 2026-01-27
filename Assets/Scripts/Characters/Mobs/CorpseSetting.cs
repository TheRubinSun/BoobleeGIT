using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class CorpseSetting : MonoBehaviour, ICullableObject
{
    public bool isBusy { get; private set; }
    public string NameKey { get; set; }
    public AudioSource audioSource;
    

    private SpriteRenderer spr_ren;
    private Animator animator_main;

    private CullingObject culling;
    private bool isVisibleNow = true;

    private Vector2 startPosition;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spr_ren = GetComponent<SpriteRenderer>();
        animator_main = GetComponent<Animator>();
    }
    public void Start()
    {
        isBusy = false;

        startPosition = transform.position;
        CreateCulling();
        UpdateCulling(false);
        GlobalData.CullingManager.RegisterObject(this);
    }
    public virtual void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float corpsePosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = (Mathf.RoundToInt((corpsePosY - PlayerPosY) * -10));
    }
    public void Busy()
    {
        isBusy = true;
    }
    public List<Slot> GetDrop()
    {
        //Debug.Log($"труп: {NameKey}");
        return ChanceAllDrop();
    }
    private List<Slot> ChanceAllDrop()
    {
        DropItemEnemy[] nameKeysItem = ItemDropEnemy.enemyAndHisDropItems[NameKey];
        float dropChance;
        List<Slot> dropItems = new List<Slot>();
        foreach (DropItemEnemy dropItem in nameKeysItem)
        {
            Item tempItem = ItemsList.GetItemForName(dropItem.item_key);
            dropChance = CalculatingItemDrop(tempItem) / 100f;
            //Debug.Log($"{item.NameKey}:  Шанс:{dropChance*100}%");
            if (Random.value < dropChance)
            {
                dropItems.Add(new Slot(tempItem, Random.Range(dropItem.countMin, dropItem.countMax+1)));
                //Debug.Log($"Выпал предмет: {item.NameKey}");
            }
            
        }
        return dropItems;
    }

    private float CalculatingItemDrop(Item item)
    {
        float totalChance = 0;
        //Debug.LogWarning($"TypeMob: {EnemyList.Instance.GetTypeMob(NameKey)} mob:{NameKey}");
        switch (EnemyList.GetTypeMob(NameKey))
        {
            case TypeMob.Technology:
                {
                    totalChance = MultiplyChanceRared(GlobalData.Player.GetPlayerStats().TechniquePoints, item.quality);
                    break;
                }
            case TypeMob.Magic:
                {
                    totalChance = MultiplyChanceRared(GlobalData.Player.GetPlayerStats().MagicPoints, item.quality);
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
                    multiplyChance = 56 * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Uncommon:
                {
                    multiplyChance = 16 * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Rare:
                {
                    multiplyChance = 4f * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Mystical:
                {
                    multiplyChance = 0.8f * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Legendary:
                {
                    multiplyChance = 0.15f * MultiplyChancePlayerPoint(countTypePoint);
                    break;
                }
            case Quality.Interverse:
                {
                    multiplyChance = 0.025f * MultiplyChancePlayerPoint(countTypePoint);
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
            chance = ((float)countTypePoint * countTypePoint * 0.07f);
        }
        return chance;
    }

    public void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main);
    }
    private void OnDisable()
    {
        if (GlobalData.CullingManager != null)
            GlobalData.CullingManager.UnregisterObject(this);

    }
    public Vector2 GetPosition() => startPosition;
    public void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }
}
