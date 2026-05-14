using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeExp
{
    None,
    Trade,
    Farm,
    Collect,
    MainExp
}
public abstract class ObjectL : MonoBehaviour, ICullableObject
{
    protected SpriteRenderer spr_ren;
    protected CullingObject culling;
    protected bool isVisibleNow = true;
    protected Vector2 startPos;
    [SerializeField] protected Vector2 ToDropPos;
    [SerializeField] protected bool IsUpper;
    [SerializeField] protected SpriteRenderer[] sr_childs;
    [SerializeField] protected Animator[] anims_childs;
    [SerializeField] protected TypeParticle customPartical;
    public abstract void CreateCulling();
    public abstract Vector2 GetPosition();
    public abstract void UpdateCulling(bool shouldBeVisible);
    protected float valueLayer = GlobalData.valueLayerObj;

    public virtual void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        if (IsUpper) return;

        float PosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        if (spr_ren != null)
        {
            spr_ren.sortingOrder = Mathf.RoundToInt(((PosY - valueLayer) - PlayerPosY - 2) * -5);
            foreach (SpriteRenderer s in sr_childs)
            {
                if (s != null)
                    s.sortingOrder = spr_ren.sortingOrder + 1;
            }

        }
    }

    protected void OnDisable()
    {
        if (GlobalData.CullingManager != null)
            GlobalData.CullingManager.UnregisterObject(this);
    }
}

public abstract class ObjectLBroken : ObjectL
{
    protected Animator anim;
    [SerializeField] protected int remainsHits = 4;
    [SerializeField] protected int toNextStageAnim = 1;

    [SerializeField] protected AudioClip[] soundsBroken;
    [SerializeField] protected AudioClip[] fullBroken;
    [SerializeField] protected Color32 particalColor;

    [SerializeField] protected List<ItemDropData> itemsDrop = new List<ItemDropData>();

    protected Color32 original_color; //Цвет
    [SerializeField] protected Color32 damage_color;

    protected AudioSource audioS;
    protected int brokenStage;
    protected Coroutine flashCol;
    protected Collider2D myCollider;

    [SerializeField] protected int exp;
    [SerializeField] protected int exp_full;
    public TypeExp typeExp;

    [SerializeField] protected int exp_damage;

    public override Vector2 GetPosition() => startPos;
    public virtual float GetPosX() => startPos.x;
    public virtual float GetPosY() => startPos.y;

    public abstract void Break(CanBeWeapon canBeWeapon, int count = 1);
    protected virtual void Awake()
    {
        spr_ren = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioS = GetComponent<AudioSource>();
        myCollider = GetComponent<Collider2D>();
    }
    protected virtual void Start()
    {
        original_color = spr_ren.color;
        startPos = transform.position;
        //audioS.volume = GlobalData.VOLUME_SOUNDS;

        AddDropItem();
        CreateCulling();
        UpdateCulling(false);
        GlobalData.CullingManager.RegisterObject(this);
    }
    protected virtual void AddDropItem()
    {
        foreach (ItemDropData itemDrop in itemsDrop)
        {
            itemDrop.Item = ItemsList.GetItemForNameKey(itemDrop.item_key);
        }
    }

    protected virtual IEnumerator BreakAndDestroy()
    {
        CreateParticale();
        HideBeforeDestroy();
        DropItems();

        if (fullBroken == null)
        {
            Debug.LogWarning("Нет звуков");
        }
        else
        {
            yield return CompletelyBreak();
        }
        
        DestroyObject();
    }
    protected virtual void PartiallyBreak()
    {
        CreateParticale();
        if (damage_color.a > 0)
        {
            flashCol = StartCoroutine(FlashColor(damage_color, 0.1f));
        }

        if (soundsBroken.Length == 0)
        {
            Debug.LogWarning("Нет звуков");
            return;
        }
        AudioClip audioClip = soundsBroken[Random.Range(0, soundsBroken.Length)];
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;
        audioS.PlayOneShot(audioClip);
    }
    protected virtual void CreateParticale()
    {
        GameObject prefab = null;

        if (customPartical != TypeParticle.None)
        {
            prefab = ResourcesData.GetParticalPrefab(customPartical);
        }
        else if (particalColor.a > 0)
        {
            prefab = ResourcesData.GetParticalPrefab(TypeParticle.Broken_Particle);
        }

        if (prefab == null) return;

        GameObject particleObj = Instantiate(prefab, transform.position, Quaternion.identity);
        if(particleObj.TryGetComponent<ParticleSystem>(out ParticleSystem particle))
        {
            var main = particle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(particalColor);
        }

    }
    protected virtual IEnumerator CompletelyBreak()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;

        AudioClip useAudio = fullBroken[Random.Range(0, fullBroken.Length)];
        audioS.PlayOneShot(useAudio);
        yield return new WaitForSeconds(useAudio.length);
    }
    protected virtual void HideBeforeDestroy()
    {
        spr_ren.enabled = false;
        myCollider.enabled = false;
    }
    protected virtual void DestroyObject()
    {
        GridNodes.NotifyWalkableObject(transform.position, myCollider.bounds.size);
        Destroy(gameObject);
    }
    protected virtual void DropItems()
    {
        int addForLevelType = 0;
        switch(typeExp) //За уровень увеличенный дроп
        {
            case TypeExp.None:
                break;
            case TypeExp.Farm:
                addForLevelType += GlobalData.Player.GetPlayerStats().farm_level;
                break;
            case TypeExp.Collect:
                addForLevelType += GlobalData.Player.GetPlayerStats().collect_level;
                break;
        }
        foreach (ItemDropData item in itemsDrop)
        {
            int countItem = 0;
            if (item.max + addForLevelType > 0)
            {
                for(int i = 0; i < item.max; i++)
                {
                    int chance = Random.Range(0, 10000); //Рандомный шанс до 100.00
                    if (chance >= (item.chance + (addForLevelType * 2)) * 100f) break; 
                    countItem++;
                }
            }
            if (countItem < 1 && item.min < 1) continue;
            if (countItem < 1) countItem = item.min;

            GameObject dropItem = Instantiate(GlobalPrefabs.ItemDropPref, GlobalData.GameManager.dropParent);

            Vector2 dropPos;
            if (ToDropPos.x != 0)
            {
                dropPos = new Vector2(GetPosX() + (int)(Random.Range(-ToDropPos.x, ToDropPos.x) * 10)/10f, GetPosY() + ToDropPos.y);
            }
            else
            {
                dropPos = new Vector2(GetPosX() + Random.Range(-1f, 1f), GetPosY() + ToDropPos.y);
            }
            dropItem.transform.position = dropPos;

            //Debug.Log($"[Drop] {item.Item.NameKey}: шанс {item.chance}%, выпало {countItem}");
            ItemDrop ItemD = dropItem.GetComponent<ItemDrop>();

            Item tempItem = item.Item;

            ItemD.sprite = tempItem.GetSprite();
            ItemD.item = tempItem;
            ItemD.count = countItem;
            dropItem.name = $"{tempItem.NameKey} ({ItemD.count})";

            dropItem.GetComponent<SpriteRenderer>().sprite = ItemD.sprite;
            dropItem.GetComponentInChildren<TextMeshPro>().text = $"{ItemD.item.Name} ({ItemD.count})";
        }
    }
    public virtual IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;

            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
        }
    }
}
public class PositionArgs : System.EventArgs
{
    public Vector2 pos;
    public Vector2 sizeObj;
    public PositionArgs(Vector2 worldPos, Vector2 size)
    {
        pos = worldPos;
        sizeObj = size;
    }
}
[System.Serializable]
public class ItemDropData
{
    public string item_key;
    public Item Item { get; set; }
    public float chance;
    public int min;
    public int max;

    public ItemDropData(string item_name,  int min, int max, float chance)
    {
        this.item_key = item_name;
        this.min = min;
        this.max = max;
        this.chance = chance;
    }
}

