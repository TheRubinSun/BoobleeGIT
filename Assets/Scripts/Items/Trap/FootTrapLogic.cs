using System.Collections;
using UnityEngine;

public class FootTrapLogic : TrapLogic
{
    public int damageTrap { get; set; }
    public damageT damageT { get; set; }
    public float timeDuration { get; set; }
    [SerializeField] private AudioClip trapped_sound;

    //private bool isCaught = false;

    public void SetParameters(int _damageTrap, damageT _damageT, float _timeDuration)
    {
        damageTrap = _damageTrap;
        damageT = _damageT;
        timeDuration = _timeDuration;
    }
    public void Activate(BaseEnemyLogic baseEnemyLogic, EffectsManager eff_man)
    {
        if (baseEnemyLogic.IsTrapped || baseEnemyLogic.IsFly) return;

        //isCaught = true;
        sr.enabled = false;
        cold.enabled = false;



        baseEnemyLogic.SetTrapped(timeDuration);

        EffectData effectTemplate = Resources.Load<EffectData>("Effects/" + "Trapped");
        // Создаем КОПИЮ данных
        EffectData trapped = Instantiate(effectTemplate);

        trapped.EffectName = effectTemplate.EffectName;
        trapped.effectObj = effectTemplate.effectObj;
        trapped.effectType = EffectData.EffectType.SpeedSlow;
        trapped.value = 100;
        trapped.idSprite = effectTemplate.idSprite;
        trapped.duration = timeDuration;
        trapped.cooldown = 0;

        eff_man.ApplyEffect(trapped);

        baseEnemyLogic.TakeDamage(damageTrap, damageT, false);
        StartCoroutine(PlaySoundsAndDestroy());
        //isCaught = false;
        //baseEnemyLogic.SetSpeedCoof(coofMove);
        //baseEnemyLogic.TakeDamage(damageTrap, damageT, false);
        //StartCoroutine(DurationAndDestroy(baseEnemyLogic));
    }
    private IEnumerator PlaySoundsAndDestroy()
    {
        audioSource.PlayOneShot(trapped_sound);
        yield return new WaitForSeconds(trapped_sound.length);
        Destroy(gameObject);
    }
    //private IEnumerator DurationAndDestroy(BaseEnemyLogic baseEnemyLogic)
    //{
    //    yield return new WaitForSeconds(timeDuration);
    //    baseEnemyLogic.ToBaseSpeed();
    //    yield return null;
    //    Destroy(gameObject);
    //}
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        //if (isCaught) return;
        //Debug.Log("Trigger");
        if (collision != null)
        {
            if (collision.gameObject.layer == LayerManager.enemyLayer)
            {
                //Debug.Log("Enemy " + collision.name);

                BaseEnemyLogic baseEnemyLogic = collision.GetComponent<BaseEnemyLogic>();
                EffectsManager eff_man = collision.GetComponent<EffectsManager>();

                if (baseEnemyLogic == null) 
                    baseEnemyLogic = collision.GetComponentInParent<BaseEnemyLogic>();
                if (eff_man == null) 
                    eff_man = collision.GetComponentInParent<EffectsManager>();
                //if(baseEnemyLogic == null) baseEnemyLogic = collision.transform.parent.GetComponent<BaseEnemyLogic>();

                Activate(baseEnemyLogic, eff_man);
            }
        }
    }
}
