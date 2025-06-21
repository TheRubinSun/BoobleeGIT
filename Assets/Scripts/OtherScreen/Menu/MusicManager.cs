using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip[] musics;
    private AudioSource music_source;
    void Start()
    {
        music_source = GetComponent<AudioSource>();
        music_source.volume = GlobalData.VOLUME_MUSICS;
        music_source.loop = true;
        music_source.clip = musics[Random.Range(0, musics.Length)];
        music_source.Play();
    }
}
