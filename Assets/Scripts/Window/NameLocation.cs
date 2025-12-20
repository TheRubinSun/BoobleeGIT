using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class NameLocation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLocationName;
    [SerializeField] private float TimeToStart;
    [SerializeField] private float TimeToEnd;
    [SerializeField] private float TimeDisplayText;
    [SerializeField] private CanvasGroup wholeObject;
    [SerializeField] private string nameLoc;
    [SerializeField] private AudioClip[] fade_sound;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GlobalData.NAME_NEW_LOCATION_TEXT = GlobalData.LocalizationManager.GetLocalizedValue("ui_text", "name_location")[GlobalData.NAME_NEW_LOCATION];
        StartCoroutine(ShowAndHideName(TimeToStart, TimeToEnd, TimeDisplayText, GlobalData.NAME_NEW_LOCATION_TEXT));
    }
    private IEnumerator ShowAndHideName(float timeToStart, float timeToEnd, float timeLoad, string name)
    {
        nameLocationName.text = name;

        yield return new WaitForSeconds(timeToStart);

        //audioSource.volume = GlobalData.VOLUME_SOUNDS;

        audioSource.PlayOneShot(fade_sound[Random.Range(0, fade_sound.Length)]);
        float elapsedTime = 0f;

        while(elapsedTime < timeLoad)
        {
            elapsedTime += Time.deltaTime;
            wholeObject.alpha = Mathf.Clamp01(elapsedTime / timeLoad);
            yield return null;
        }

        yield return new WaitForSeconds(timeToEnd);
        elapsedTime = 0f;

        while (elapsedTime < timeLoad)
        {
            elapsedTime += Time.deltaTime;
            wholeObject.alpha = Mathf.Clamp01(1f - (elapsedTime / timeLoad));
            yield return null;
        }
        wholeObject.alpha = 0f;
        Destroy(this.gameObject);
    }
}
