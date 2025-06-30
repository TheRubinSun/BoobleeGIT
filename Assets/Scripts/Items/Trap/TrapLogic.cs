using UnityEngine;

public abstract class TrapLogic : MonoBehaviour 
{
    protected AudioSource audioSource;

    protected Animator anim;
    protected SpriteRenderer sr;
    protected Collider2D cold;

    protected bool isActivate = false;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        cold = GetComponent<Collider2D>();
        //audioSource.volume = GlobalData.VOLUME_SOUNDS;
    }
    public virtual void Activate()
    {

    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision!= null)
        {
            if (collision.gameObject.layer == LayerManager.enemyLayer)
            {
                Activate();
            }
        }
    }
}
