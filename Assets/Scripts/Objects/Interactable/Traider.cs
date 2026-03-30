using UnityEngine;

public class Traider : DrawOutline, IInteractable
{
    public string nameTrader;
    private bool IsOpen;
    [SerializeField] private Animator door_anim;
    [SerializeField] private Animator door_table_anim;
    public void Interact()
    {
        GlobalData.UIControl.OpenShopSurv(nameTrader);

    }
    public override void DrawOutlineObj()
    {
        base.DrawOutlineObj();
        door_anim.SetBool("IsOpen", true);
        door_table_anim.SetBool("IsOpen", true);
    }
    public override void EarseOutlineObj()
    {
        base.EarseOutlineObj();
        door_anim.SetBool("IsOpen", false);
        door_table_anim.SetBool("IsOpen", false);
    }
}
