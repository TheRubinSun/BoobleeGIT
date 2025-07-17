using UnityEngine;

public class HealMin : GunMinCon
{
    protected override void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        base.Start();
    }
    protected override Transform FindAim()
    {
        Transform player = GameManager.Instance.PlayerModel.transform;
        if (Vector2.Distance(transform.position, player.position) <= radiusVision)
        {
            Debug.Log("Объект в радиусе: " + player.name);
            CheckToFlip(player);
            return player;
        }
        return null;
    }
}
