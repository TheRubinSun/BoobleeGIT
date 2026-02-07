using UnityEngine;

public class LoadingSlime: MonoBehaviour, IItemMove
{
    [SerializeField] Transform main_obj;
    [SerializeField] Transform item_one;
    [SerializeField] Transform item_two;
    [SerializeField] Transform item_three;

    protected SpriteRenderer sr_item_one;
    protected SpriteRenderer sr_item_two;
    protected SpriteRenderer sr_item_three;

    protected SpriteRenderer spr_ren;
    protected Animator animator_main;

    int face_dir = 1;
    private void Awake()
    {
        spr_ren = main_obj.GetComponent<SpriteRenderer>();
        animator_main = main_obj.GetComponent<Animator>();
        animator_main.SetBool("Move", true);
    }
    private void Start()
    {
        DropItemEnemy[] nameKeysItem = ItemDropEnemy.enemyAndHisDropItems["slime_enem"];

        sr_item_one = item_one.GetComponent<SpriteRenderer>();
        sr_item_two = item_two.GetComponent<SpriteRenderer>();
        sr_item_three = item_three.GetComponent<SpriteRenderer>();

        sr_item_one.sprite = GetRandomItem(nameKeysItem).Sprite;
        sr_item_two.sprite = GetRandomItem(nameKeysItem).Sprite;
        sr_item_three.sprite = GetRandomItem(nameKeysItem).Sprite;

        sr_item_one.sortingOrder = spr_ren.sortingOrder - 1;
        sr_item_two.sortingOrder = spr_ren.sortingOrder - 1;
        sr_item_three.sortingOrder = spr_ren.sortingOrder - 1;
    }
    private Item GetRandomItem(DropItemEnemy[] nameKeysItem)
    {
        string nameItem = nameKeysItem[Random.Range(0, nameKeysItem.Length)].item_key;
        return ItemsList.GetItemForName(nameItem);
    }
    public void SetItemsPosIdle(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.1f * face_dir, 0.01f);
                item_two.localPosition = new Vector2(-0.06f * face_dir, -0.03f);
                item_three.localPosition = new Vector2(-0.004f * face_dir, 0.07f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.1f * face_dir, 0.03f);
                item_two.localPosition = new Vector2(-0.06f * face_dir, -0.01f);
                item_three.localPosition = new Vector2(-0.004f * face_dir, 0.09f);
                break;
        }
    }
    public void SetItemsPosMove(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.18f * face_dir, 0.08f);
                item_two.localPosition = new Vector2(0.016f * face_dir, -0.06f);
                item_three.localPosition = new Vector2(0.05f * face_dir, 0.08f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.12f * face_dir, -0.05f);
                item_two.localPosition = new Vector2(-0.03f * face_dir, -0.05f);
                item_three.localPosition = new Vector2(0.17f * face_dir, 0.04f);
                break;
            case 2:
                item_one.localPosition = new Vector2(0.05f * face_dir, -0.06f);
                item_two.localPosition = new Vector2(-0.07f * face_dir, -0.05f);
                item_three.localPosition = new Vector2(0.18f * face_dir, -0.02f);
                break;
            case 3:
                item_one.localPosition = new Vector2(0.01f * face_dir, -0.06f);
                item_two.localPosition = new Vector2(-0.025f * face_dir, 0.05f);
                item_three.localPosition = new Vector2(0.17f * face_dir, -0.06f);
                break;
            case 4:
                item_one.localPosition = new Vector2(0.04f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(0.1f * face_dir, -0.04f);
                item_three.localPosition = new Vector2(0.1f * face_dir, 0.06f);
                break;
        }


    }
    public void SetItemsPosShoot(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.11f * face_dir, 0.016f);
                item_two.localPosition = new Vector2(-0.1f * face_dir, -0.04f);
                item_three.localPosition = new Vector2(0.01f * face_dir, 0.05f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.14f * face_dir, -0.06f);
                item_two.localPosition = new Vector2(-0.12f * face_dir, -0.05f);
                item_three.localPosition = new Vector2(0.1f * face_dir, -0.02f);
                break;
            case 2:
                item_one.localPosition = new Vector2(0.12f * face_dir, 0);
                item_two.localPosition = new Vector2(-0.12f * face_dir, 0);
                item_three.localPosition = new Vector2(0 * face_dir, -0.05f);
                break;
            case 3:
                item_one.localPosition = new Vector2(0.05f * face_dir, 0);
                item_two.localPosition = new Vector2(-0.06f * face_dir, 0);
                item_three.localPosition = new Vector2(0 * face_dir, 0.1f);
                break;
            default:
                Debug.LogWarning("ПРоблема с анимацией");
                break;
        }
    }
    public void SetItemsPosMeleAttack(int frame)
    {
        switch (frame)
        {
            case 0:
                item_one.localPosition = new Vector2(0.08f * face_dir, -0.04f);
                item_two.localPosition = new Vector2(-0.1f * face_dir, -0.02f);
                item_three.localPosition = new Vector2(0.06f * face_dir, 0.06f);
                break;
            case 1:
                item_one.localPosition = new Vector2(0.12f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(-0.1f * face_dir, 0.03f);
                item_three.localPosition = new Vector2(0.01f * face_dir, 0.22f);
                break;
            case 2:
                item_one.localPosition = new Vector2(0.11f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(-0.11f * face_dir, 0.06f);
                item_three.localPosition = new Vector2(-0.13f * face_dir, 0.1f);
                break;
            case 3:
                item_one.localPosition = new Vector2(0.1f * face_dir, -0.05f);
                item_two.localPosition = new Vector2(-0.08f * face_dir, -0.04f);
                item_three.localPosition = new Vector2(0.19f * face_dir, 0.07f);
                break;
            case 4:
                item_one.localPosition = new Vector2(0.1f * face_dir, -0.01f);
                item_two.localPosition = new Vector2(-0.07f * face_dir, 0);
                item_three.localPosition = new Vector2(0.18f * face_dir, -0.08f);
                break;
            default:
                Debug.LogWarning("ПРоблема с анимацией");
                break;
        }
    }
}
