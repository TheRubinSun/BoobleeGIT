using UnityEngine;

public class Tree : MonoBehaviour
{
    private SpriteRenderer spr_ren;
    private SpriteRenderer spr_Child_ren;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr_ren = GetComponent<SpriteRenderer>(); //Берем спрайт дерева
        spr_Child_ren = transform.GetChild(0).GetComponent<SpriteRenderer>();

        anim.speed = Random.Range(0.9f, 1.1f);
        spr_ren.sortingOrder = Mathf.RoundToInt((transform.position.y - 10) * -10);
        spr_Child_ren.sortingOrder = spr_ren.sortingOrder - 1;
    }

}
