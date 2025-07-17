using UnityEngine;

public class PlayerHealProj : PlayerProjectile
{
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerManager.touchObjectsLayer)
        {
            ObjectLBroken objectL = collider.gameObject.GetComponent<ObjectLBroken>();
            if (objectL != null)
            {
                objectL.Break(canBeWeapon);
                Destroy(gameObject);
            }
            return;
        }
        else if (collider.gameObject.layer == LayerManager.playerLayer)
        {
            Player player = Player.Instance;
            player.TakeHeal(damage);
            Destroy(gameObject);
            return;
            //Debug.Log(collider.GetComponent<BaseEnemyLogic>().enum_stat.Cur_Hp+" "+ collider.GetComponent<BaseEnemyLogic>().enum_stat.Max_Hp);
        }
        else if (collider.gameObject.layer == LayerManager.obstaclesLayer)
        {
            Destroy(gameObject);
            return;
        }
    }
}
