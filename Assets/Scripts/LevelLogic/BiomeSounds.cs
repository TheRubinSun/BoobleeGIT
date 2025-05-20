using UnityEngine;

public class BiomeSounds : MonoBehaviour 
{
    [SerializeField] private AudioClip[] biomeSounds;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(biomeSounds.Length>0)
        {
            audioSource.clip = biomeSounds[Random.Range(0, biomeSounds.Length)];
            audioSource.Play();
        }

    }
}
