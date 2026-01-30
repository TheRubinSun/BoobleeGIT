
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
                    GlobalData.Player.AddTypeExp(typeExp, exp_full); //Если было посаженно то фарм
                }
                else
                {
                    GlobalData.Player.AddTypeExp(typeExp, exp_full); //Если найденно то коллект
                }
                StartCoroutine(PlaySoundFullBroken());
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

        if (IsUpper) return;

        float PosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt(((PosY - 1.6f) - PlayerPosY - 2) * -5);
    }
}
