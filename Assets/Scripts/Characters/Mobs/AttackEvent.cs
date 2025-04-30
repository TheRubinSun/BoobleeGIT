using UnityEngine;

public class AttackEvent : MonoBehaviour
{
    private IAttack attack;

    protected virtual void Awake()
    {
        attack = GetComponentInParent<IAttack>();
    }
    public void RangeAttack()
    {
        attack.RangeAttack();
    }
    public void MeleeAttack()
    {
        attack.MeleeAttack();
    }
}

