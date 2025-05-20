using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lamp : JustObject
{
    private Light2D light2d;
    protected override void Awake()
    {
        base.Awake();
        light2d = GetComponentInChildren<Light2D>();
    }

    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim, new SpriteRenderer[] {}, null, light2d);
    }

    public override void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }
}
