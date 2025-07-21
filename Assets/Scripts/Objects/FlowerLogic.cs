using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class FlowerLogic : ObjectLBroken , IPointFarm
{
    public int ID { get;  set; }
    public int IdDirtBed { get; set; }

    protected override void Awake()
    {
        base.Awake();

        anim.speed = Random.Range(0.9f, 1.1f);
    }
    public override void Break(CanBeWeapon canBeWeapon)
    {
        if (canBeWeapon.canBeCut == true)
        {
            remainsHits--;
            if (remainsHits == 0)
            {
                GardenManager gr = GardenManager.instance;
                if (gameObject.tag == "Planted")
                {
                    
                    GlobalWorld.RemoveFarmPoint(ID);
                    if (gr != null)
                    {
                        Debug.Log($"remove Flower in {ID}");
                        gr.RemoveSeed(IdDirtBed);
                    }
                        
                }
                StartCoroutine(PlayeSoundFullBroken());
            }
            else if (remainsHits % toNextStageAnim == 0)
            {
                PlayeSoundBroken();
                brokenStage++;
            }
        }
    }

    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, anim, null);
    }

    public override void UpdateCulling(bool shouldBeVisible)
    {
        if (isVisibleNow != shouldBeVisible)
        {
            isVisibleNow = shouldBeVisible;
            culling.SetVisible(shouldBeVisible);
        }
    }

    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float PosY = transform.position.y;
        float PlayerPosY = GameManager.Instance.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((PosY - 1.6f) - PlayerPosY - 2) * -5);
    }
}
