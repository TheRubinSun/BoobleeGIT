using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface ICullableObject
{
    Vector2 GetPosition();
    void CreateCulling();
    void UpdateCulling(bool shouldBeVisible);

    void UpdateSortingOrder();
}
public class CullingManager : MonoBehaviour
{
    public static CullingManager Instance;
    public Transform target;

    public float activationRadiusX = 14f; // горизонталь
    public float activationRadiusY = 8f;  // вертикаль

    public bool allVisible = false;
    private HashSet<ICullableObject> objects_visibles = new HashSet<ICullableObject>();

    private float cullCheckInterval = 0.25f; // 4 раза в секунду (вместо каждого кадра)
    private float cullCheckTimer = 0;
    private void Awake()
    {
        Instance = this;
        if(target == null)
        {
            if(GameManager.Instance.PlayerModel != null) target = GameManager.Instance.PlayerModel;
            else
            {
                target = GameObject.Find("PlayerModel").transform;
                GameManager.Instance.PlayerModel = target;
            }
        }
    }
    private void Start()
    {
        Camera cam = Camera.main;
        if(cam.orthographic)
        {
            float vert = cam.orthographicSize;
            float horiz = vert * cam.aspect;

            activationRadiusY = vert + 2f;
            activationRadiusX = horiz + 2f;
        }
    }
    public void RegisterObject(ICullableObject object_Visible)
    {
        objects_visibles.Add(object_Visible);
    }
    public void UnregisterObject(ICullableObject object_Visible)
    {
        objects_visibles.Remove(object_Visible);
    }
    private void Update()
    {
        cullCheckTimer += Time.deltaTime;
        if (cullCheckTimer < cullCheckInterval) return;

        cullCheckTimer = 0;

        foreach (ICullableObject object_Visible in objects_visibles)
        {
            if (object_Visible == null) continue;

            Vector2 objectPos = object_Visible.GetPosition();

            bool inBounds = Mathf.Abs(objectPos.x - target.position.x) <= activationRadiusX &&
                            Mathf.Abs(objectPos.y - target.position.y) <= activationRadiusY;

            bool shouldBeVisible = allVisible || inBounds;

            if (!allVisible)
            {
                object_Visible.UpdateCulling(shouldBeVisible);
            }
            else
            {
                object_Visible.UpdateCulling(true);
            }
            
            if(shouldBeVisible)
            {
                object_Visible.UpdateSortingOrder();
            }
        }
    }
}
public class CullingObject
{
    private SpriteRenderer sprite_ren;
    private SpriteRenderer[] sprites_ren_childs;
    private Animator animator_main;
    private Animator[] animator_main_childs;

    public CullingObject(SpriteRenderer ren, Animator anim = null, SpriteRenderer[] _sprites_ren_childs = null, Animator[] _animator_main_childs = null)
    {
        sprite_ren = ren;
        sprites_ren_childs = _sprites_ren_childs;
        animator_main = anim;
        animator_main_childs = _animator_main_childs;
    }
    public void SetVisible(bool isVisible)
    {
        if (sprite_ren != null && sprite_ren.enabled != isVisible)
            sprite_ren.enabled = isVisible;

        if (animator_main != null && animator_main.enabled != isVisible)
            animator_main.enabled = isVisible;


        if (sprites_ren_childs != null && sprites_ren_childs.Length > 0)
        {
            foreach (var sr_child in sprites_ren_childs)
                sr_child.enabled = isVisible;

        }
        if (animator_main_childs != null && animator_main_childs.Length > 0)
        {
            foreach (var anim_child in animator_main_childs)
                anim_child.enabled = isVisible;

        }
    }
}
