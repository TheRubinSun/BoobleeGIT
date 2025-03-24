using UnityEngine;

public class Tree : MonoBehaviour, ICullableObject
{
    private SpriteRenderer spr_ren;
    private SpriteRenderer spr_Child_ren;
    private Animator anim;

    protected CullingObject culling;
    protected bool isVisibleNow = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr_ren = GetComponent<SpriteRenderer>(); //Берем спрайт дерева
        spr_Child_ren = transform.GetChild(0).GetComponent<SpriteRenderer>();

        anim.speed = Random.Range(0.9f, 1.1f);
        spr_ren.sortingOrder = Mathf.RoundToInt((transform.position.y - 10) * -10);
        spr_Child_ren.sortingOrder = spr_ren.sortingOrder - 1;
    }
    private void Start()
    {
        CreateCulling();
        UpdateCulling(false);
        CullingManager.Instance.RegisterObject(this);
    }

    public void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim, new SpriteRenderer[] { spr_Child_ren });
    }
    public Transform GetTransform() => transform;
    private void OnDisable()
    {
        if (CullingManager.Instance != null)
            CullingManager.Instance.UnregisterObject(this);
    }
    public void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }


}
