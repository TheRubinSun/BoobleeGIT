using System.Collections;
using TMPro;
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

    [SerializeField] private TextMeshPro textDPS;

    private int damageThisSecond = 0;
    private int currentDPS = 0;
    private float timer = 0f;

    public override void TakeDamage(int damage, damageT typeAttack, bool canEvade, EffectData effect = null)
    {
        animator_main.SetTrigger("TakeDamage");
        finalTakeDamage = 0;
        base.TakeDamage(damage, typeAttack, canEvade, effect);
        damageThisSecond += finalTakeDamage;
    }
    protected override void Update() 
    {
        timer += Time.deltaTime;
        if(timer >= 1f)
        {
            currentDPS = damageThisSecond;
            damageThisSecond = 0;
            timer = 0f;

            DisplayDPS(currentDPS);
        }
    }
    private void DisplayDPS(int dps)
    {
        textDPS.text = $"DPS: {dps}";
    }

}
