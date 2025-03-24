using System.Collections;
using UnityEngine;

public class DaizenLogic : BaseEnemyLogic
{
    private SpriteRenderer spr_ren_ch { get; set; }

    //Объекты
    [SerializeField]
    private Transform child_Obj; //Дочерний объект

    protected override void Start()
    {
        spr_ren_ch = child_Obj.GetComponent<SpriteRenderer>();//Берем доч спрайт моба, если есть

        base.Start();
    }

    protected override void UpdateSortingOrder()
    {
        if (!isVisibleNow) return;

        if (Time.time >= nextUpdateTime)
        {
            spr_ren.sortingOrder = Mathf.RoundToInt((transform.position.y - 10) * -10);

            if(spr_ren_ch != null ) spr_ren_ch.sortingOrder = spr_ren.sortingOrder - 1;

            nextUpdateTime = Time.time + updateRate;
        }

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
    private void MeleeAttackOne()
    {
        audioSource.volume = attack_volume;
        audioSource.Stop();
        audioSource.PlayOneShot(attack_sound); //Звук выстрела


        Player.Instance.TakeDamage(enum_stat.Att_Damage, true);
    }
    public override void CreateCulling()
    {
        culling = new CullingObject(spr_ren, animator_main, new SpriteRenderer[] { spr_ren_ch }, new Animator[] { child_Obj.GetComponent<Animator>() });
    }
}
