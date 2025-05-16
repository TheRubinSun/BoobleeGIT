using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

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
        CullingManager.Instance.RegisterObject(this);
    }
    public virtual void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float corpsePosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

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
    public void PlayDieSoind(AudioClip dieSound)
    {
        Debug.Log(dieSound.name + " Played");
        audioSource.Stop();
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(dieSound);
    }
    private List<Slot> ChanceAllDrop()
    {
        string[] nameKeysItem = ItemDropEnemy.enemyAndHisDrop[NameKey];
        float dropChance;
        List<Slot> dropItems = new List<Slot>();
        foreach (string nameItem in nameKeysItem)
        {
            Item item = ItemsList.GetItemForName(nameItem);
            dropChance = CalculatingItemDrop(item) / 100f;
            //Debug.Log($"{item.NameKey}:  Шанс:{dropChance*100}%");
            if (Random.value < dropChance)
            {
                dropItems.Add(new Slot(item, 1));
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
                    totalChance = MultiplyChanceRared(Player.Instance.GetPlayerStats().TechniquePoints, item.quality);
                    break;
                }
            case TypeMob.Magic:
                {
                    totalChance = MultiplyChanceRared(Player.Instance.GetPlayerStats().MagicPoints, item.quality);
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

    public void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main);
    }
    private void OnDisable()
    {
        if (CullingManager.Instance != null)
            CullingManager.Instance.UnregisterObject(this);

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
