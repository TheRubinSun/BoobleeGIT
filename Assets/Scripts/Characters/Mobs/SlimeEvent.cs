using UnityEngine;

public class SlimeEvent : AttackEvent
{
    private IItemMove item_move;

    protected override void Awake()
    {
        base.Awake();
        item_move = GetComponentInParent<IItemMove>();
    }
    public void SetItemsPosIdle(int frame)
    {
        item_move.SetItemsPosIdle(frame);
    }
    public void SetItemsPosMove(int frame)
    {
        item_move.SetItemsPosMove(frame);
    }
    public void SetItemsPosShoot(int frame)
    {
        item_move.SetItemsPosShoot(frame);
    }
    public void SetItemsPosMeleAttack(int frame)
    {
        item_move.SetItemsPosMeleAttack((int)frame);
    }
}