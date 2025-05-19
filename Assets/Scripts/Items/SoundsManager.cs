using UnityEngine;
using UnityEngine.Rendering;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance;

    public AudioSource audioSource;

    [SerializeField] private AudioClip takeItem;
    [SerializeField] private AudioClip putItem;
    [SerializeField] private AudioClip dropItem;
    [SerializeField] private AudioClip LvlUP;
    [SerializeField] private AudioClip acceptAspect;
    [SerializeField] private AudioClip openLvelUPWindow;
    [SerializeField] private AudioClip switchItem;

    [SerializeField] private AudioClip[] items_sounds;
    [SerializeField] private AudioClip[] craftItems;

    //public float volume;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //volume = GlobalData.VOLUME_SOUNDS;
        //audioSource.volume = volume;
        audioSource.ignoreListenerPause = true; // Звук будет играть даже в паузе!
    }
    public void PlayTakeItem ()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(takeItem);
    }
    public void PlayPutItem()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(putItem);
    }
    public void PlayTakeDropItem()
    {
        audioSource.pitch = Random.Range(1.2f, 1.5f);
        audioSource.PlayOneShot(dropItem);
    }
    public void PlayPutDropItem()
    {
        audioSource.pitch = Random.Range(0.5f, 0.8f);
        audioSource.PlayOneShot(dropItem);
    }
    public void PlayLevelUP()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(LvlUP);
    } 
    public void PlayOpenWindow()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(openLvelUPWindow);
    }
    public void PlayAcceptAspect()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(acceptAspect);
    }
    public void PlayItemSounds(int id)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(items_sounds[id]);
    }
    public void PlayCraftItemSounds(int id)
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(craftItems[id]);
    }
    public void PlaySwitchItemSounds()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(switchItem);
    }
}
