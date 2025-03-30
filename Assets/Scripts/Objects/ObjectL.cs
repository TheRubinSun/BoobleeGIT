using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ObjectL : MonoBehaviour, ICullableObject
{
    protected SpriteRenderer spr_ren;
    protected CullingObject culling;
    protected bool isVisibleNow = true;
    protected Vector2 startPos;

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

    protected Dictionary<string, MinMax> itemsDrop = new Dictionary<string, MinMax>();

    protected AudioSource audioS;
    protected int brokenStage;

    public override Vector2 GetPosition() => startPos;

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
        audioS.volume = SoundsManager.Instance.volume;

        AddDropItem();
        CreateCulling();
        UpdateCulling(false);
        CullingManager.Instance.RegisterObject(this);
    }
    protected abstract void AddDropItem();

    protected virtual IEnumerator PlayeSoundFullBroken()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        audioS.pitch = pitch;
        audioS.PlayOneShot(fullBroken);
        spr_ren.enabled = false;
        Collider2D collider2D = GetComponent<Collider2D>();
        collider2D.enabled = false;
        DropItems();

        yield return new WaitForSeconds(fullBroken.length);
        
        DestroyObject();
    }
    protected virtual void PlayeSoundBroken()
    {
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
        int id = 0;
        foreach (KeyValuePair<string, MinMax> item in itemsDrop)
        {
            int countItem = Random.Range(item.Value.min, (item.Value.max + 1));
            if (countItem == 0) return;

            GameObject dropItem = Instantiate(GlobalPrefabs.ItemDropPref, GameManager.Instance.dropParent);
            dropItem.transform.position = GetPosition();
            ItemDrop ItemD = dropItem.GetComponent<ItemDrop>();

            Item tempItem = ItemsList.Instance.GetItemForNameKey(item.Key);
            //Debug.Log($"tempItem.Name: {tempItem.Name} tempItem.Name {tempItem.NameKey} sprite {tempItem.Sprite.name}");
            ItemD.sprite = tempItem.GetSprite();
            ItemD.item = tempItem;
            ItemD.count = countItem;
            dropItem.name = $"{tempItem.NameKey} ({ItemD.count})";

            dropItem.GetComponent<SpriteRenderer>().sprite = ItemD.sprite;
            dropItem.GetComponentInChildren<TextMeshPro>().text = $"{ItemD.item.Name} ({ItemD.count})";
            id++;
        }
    }
}
public class MinMax
{
    public int min;
    public int max;

    public MinMax(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

