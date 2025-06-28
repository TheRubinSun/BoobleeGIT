using UnityEngine;

public abstract class TrapLogic : MonoBehaviour 
{
    protected AudioSource audioSource;

    protected Animator anim;

    protected bool isActivate = false;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
