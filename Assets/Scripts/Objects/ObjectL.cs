using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class ObjectL : MonoBehaviour, ICullableObject
{
    protected SpriteRenderer spr_ren;
    protected CullingObject culling;
    protected bool isVisibleNow = true;
    protected Vector2 startPos;
    [SerializeField] protected Vector2 ToDropPos;
    public abstract void CreateCulling();
    public abstract Vector2 GetPosition();
    public abstract void UpdateCulling(bool shouldBeVisible);
    public abstract void UpdateSortingOrder();

    protected void OnDisable()
    {
        if (CullingManager.Instance != null)
            CullingManager.Instance.UnregisterObject(this);
    }
}
public abstract class ObjectLBroken : ObjectL
{
    protected Animator anim;
    [SerializeField] protected int remainsHits = 4;
    [SerializeField] protected int toNextStageAnim = 1;

    [SerializeField] protected AudioClip[] soundsBroken;
    [SerializeField] protected AudioClip fullBroken;

    [SerializeField] protected List<ItemDropData> itemsDrop = new List<ItemDropData>();

    protected AudioSource audioS;
    protected int brokenStage;

    public override Vector2 GetPosition() => startPos;
    public virtual float GetPosX() => startPos.x;
    public virtual float GetPosY() => startPos.y;
    public abstract void Break(CanBeWeapon canBeWeapon);
    protected virtual void Awake()
    {
        spr_ren = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioS = GetComponent<AudioSource>();
    }
    protected virtual void Start()
    {
        startPos = transform.position;
        //audioS.volume = GlobalData.VOLUME_SOUNDS;

        AddDropItem();
        CreateCulling();
        UpdateCulling(false);
        CullingManager.Instance.RegisterObject(this);
    }
    protected virtual void AddDropItem()
    {
        foreach (ItemDropData itemDrop in itemsDrop)
        {
            itemDrop.Item = ItemsList.GetItemForNameKey(itemDrop.item_key);
        }
    }

    protected virtual IEnumerator PlayeSoundFullBroken()
    {
        HideBeforeDestroy();

        DropItems();

        if (fullBroken == null)
        {
            Debug.LogWarning("Нет звуков");
        }
        else
        {
            yield return PlayFullBroken();
        }
        
        DestroyObject();
    }
    protected virtual void HideBeforeDestroy()
    {
        spr_ren.enabled = false;
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.enabled = false;
    }
    protected virtual IEnumerator PlayFullBroken()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;
        audioS.PlayOneShot(fullBroken);
        yield return new WaitForSeconds(fullBroken.length);
    }
    protected virtual void PlayeSoundBroken()
    {
        if(soundsBroken.Length == 0)
        {
            Debug.LogWarning("Нет звуков");
            return;
        }
        AudioClip audioClip = soundsBroken[Random.Range(0, soundsBroken.Length)];
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;
        audioS.PlayOneShot(audioClip);
    }
    protected virtual void DestroyObject()
    {
        Destroy(gameObject);
    }
    protected virtual void DropItems()
    {
        foreach (ItemDropData item in itemsDrop)
        {
            int countItem = 0;
            if (item.max > 0)
            {
                for(int i = 0; i < item.max; i++)
                {
                    int chance = Random.Range(0, 10000); //Рандомный шанс до 100.00
                    if (chance >= item.chance * 100f) break; 
                    countItem++;
                }
            }
            if (countItem < 1 && item.min < 1) continue;
            if (countItem < 1) countItem = item.min;

            GameObject dropItem = Instantiate(GlobalPrefabs.ItemDropPref, GameManager.Instance.dropParent);

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

