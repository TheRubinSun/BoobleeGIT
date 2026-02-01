using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
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

    //[SerializeField] private AudioClip[] items_sounds;
    //[SerializeField] private AudioClip[] craftItems;
    [SerializeField] private List<SoundsType> sounds_types;
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
        audioSource.Stop();
        audioSource.clip = LvlUP;
        audioSource.Play();
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
    //public void PlayItemSounds(int id)
    //{
    //    audioSource.pitch = 1f;
    //    audioSource.PlayOneShot(items_sounds[id]);
    //}
    //public void PlayItemSoundsWithRandomPitch(float min, float max, int id)
    //{
    //    audioSource.pitch = Random.Range(min, max);
    //    audioSource.PlayOneShot(items_sounds[id]);
    //}
    //public void PlayCraftItemSounds(int id)
    //{
    //    audioSource.pitch = Random.Range(0.8f, 1.2f);
    //    audioSource.PlayOneShot(craftItems[id]);
    //}
    public void PlaySwitchItemSounds()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(switchItem);
    }
    public void PlayItemSounds(TypeSound typeSound, int id)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(sounds_types[GetIdForTypeSound(typeSound)].sounds[id]);
        //Debug.Log($"{typeSound}: {id}");
    }
    public void PlayItemSoundsWithRandomPitch(TypeSound typeSound, int id, float min, float max)
    {
        audioSource.pitch = Random.Range(min, max);
        audioSource.PlayOneShot(sounds_types[GetIdForTypeSound(typeSound)].sounds[id]);
    }
    private int GetIdForTypeSound(TypeSound typeSound)
    {
        for (int i = 0; i < sounds_types.Count; i++)
        {
            if (sounds_types[i].typeSounds == typeSound)
                return i;
        }
        return 0;
    }
}
[System.Serializable]
public class SoundsType
{
    public TypeSound typeSounds;
    public AudioClip[] sounds;
}
public enum TypeSound
{
    None,
    Booster,
    Potions,
    Traps,
    Effects,
    CraftItems,
    Others
}


