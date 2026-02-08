using System.Collections;
using UnityEngine;

public class DummyLogic : BaseEnemyLogic
{
    public override void Move() { }
    public override void Flipface() { }
    public override Vector2 ToPlayer => new Vector2();
    public override void DetectDirection() { }
    protected override RaycastHit2D BuildRayCast(Vector2 start, Vector2 end, float avoidDist, int combinedLayerMask) => new RaycastHit2D();
    protected override IEnumerator AvoidTarget(RaycastHit2D target) { yield return null; }
    protected override void PlayerDetected(Vector2 toPlayer, float distanceToPlayer) { }
    public override void RotateTowardsMovementDirection(Vector2 direction) { }
    public override void Attack(float distanceToPlayer) { }
    public override void TakeDamage(int damage, damageT typeAttack, bool canEvade, EffectData effect = null)
    {
        animator_main.SetTrigger("TakeDamage");
        base.TakeDamage(damage, typeAttack, canEvade, effect);
    }
}
