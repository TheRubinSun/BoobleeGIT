using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLogic : BaseEnemyLogic
{
    public static Action<BossLogic> OnBossDie;
    public static Action<BossLogic> OnBossAdd;

    [SerializeField] private AudioClip bossMusic;

    public AudioClip GetBossMusic() => bossMusic;
    protected override void Start()
    {
        base.Start();
        OnBossAdd?.Invoke(this);
    }
    public override void Attack(float distanceToPlayer)
    {
        base.Attack(distanceToPlayer);
    }
    protected override void LoadParametrs()
    {
        base.LoadParametrs();
    }
    protected override void Update()
    {
        base.Update();  
    }
    protected void BossDie()
    {
        OnBossDie?.Invoke(this);
    }

}
