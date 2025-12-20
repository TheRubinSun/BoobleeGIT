using System.Collections;
using UnityEngine;

public class DaizenLogic : BaseEnemyLogic
{
    private SpriteRenderer spr_ren_ch { get; set; }

    //Объекты
    [SerializeField]
    private Transform child_Obj; //Дочерний объект
    private Collider2D child_col;
    protected override void Start()
    {
        spr_ren_ch = child_Obj.GetComponent<SpriteRenderer>();//Берем доч спрайт моба, если есть
        child_col = child_Obj.GetComponent<Collider2D>();

        base.Start();
    }
    public override void SetTrapped(float time)
    {
        selfCollider.isTrigger = true;
        IsTrapped = true;
        child_col.isTrigger = true;
        StartCoroutine(OffPhysics(time));
    }
    protected override IEnumerator OffPhysics(float time)
    {
        yield return new WaitForSeconds(time);
        selfCollider.isTrigger = false;
        IsTrapped = false;
        child_col.isTrigger = false;
    }
    public override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        float mobPosY = transform.position.y;
        float PlayerPosY = GlobalData.GameManager.PlayerPosY;

        spr_ren.sortingOrder = Mathf.RoundToInt((mobPosY - PlayerPosY - 2) * -5);

        if (spr_ren_ch != null) spr_ren_ch.sortingOrder = spr_ren.sortingOrder - 5;
    }

    public override IEnumerator FlashColor(Color32 color, float time)
    {
        if (spr_ren != null)
        {
            spr_ren.color = color;
            if (spr_ren_ch != null) spr_ren_ch.color = color;


            yield return new WaitForSeconds(time);

            spr_ren.color = original_color;
            if (spr_ren_ch != null) spr_ren_ch.color = original_color;
        }
    }


    public override void Flipface() //Разворачиваем моба 
    {
        if (player == null) return; // Проверка на null

        bool shouldFaceLeft = player.position.x < transform.position.x; // Игрок слева?

        if (spr_ren.flipX != shouldFaceLeft) // Если нужно сменить направление
        {
            spr_ren.flipX = shouldFaceLeft;
            if (spr_ren_ch != null)
                spr_ren_ch.flipX = shouldFaceLeft;
        }
    }
    public override void MeleeAttack()
    {
        if (attack_sounds != null)
        {
            //audioSource.volume = attack_volume;
            audioSource.Stop();
            audioSource.PlayOneShot(attack_sounds[UnityEngine.Random.Range(0, attack_sounds.Length)]); //Звук выстрела
        }


        GlobalData.Player.TakeDamage(enum_stat.Att_Damage, damageT.Physical, true);
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { spr_ren_ch }, new Animator[] { child_Obj.GetComponent<Animator>() });
    }
}
