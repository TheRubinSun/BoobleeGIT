using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class SoundLimitSettings
    {
        public AudioClip clip;
        public int maxSounds = 3;
        public float cooldown = 0.1f;
    }
    [SerializeField] private List<SoundLimitSettings> soundsSetting;

    [Header("Дефолтные настройки (если звука нет в списке выше)")]
    [SerializeField] private int defaultMaxSounds = 4;
    [SerializeField] private float defaultCooldown = 0.1f;

    private Dictionary<AudioClip, int> activeSoundsCount = new Dictionary<AudioClip, int>();
    private Dictionary<AudioClip, float> lastPlayedTimes = new Dictionary<AudioClip, float>();
    private Dictionary<AudioClip, SoundLimitSettings> settingCache = new Dictionary<AudioClip, SoundLimitSettings>();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Кэшируем настройки для быстрого доступа
        foreach (var setting in soundsSetting)
        {
            if (setting.clip != null) settingCache[setting.clip] = setting;
        }
    }
    public bool CanPlaySound(AudioClip clip)
    {
        if(clip == null) return false;


        // Шаг 1: Инициализируем значения дефолтными параметрами
        int allowedMaxSounds = defaultMaxSounds;
        float allowedCooldown = defaultCooldown;

        // Шаг 2: Если для звука есть персональная настройка — перезаписываем дефолтные значения
        if (settingCache.TryGetValue(clip, out SoundLimitSettings settings))
        {
            allowedMaxSounds = settings.maxSounds;
            allowedCooldown = settings.cooldown;
        }

        float currentTime = Time.time;

        // Шаг 3: Проверка на Cooldown (ограничение спама в один кадр)
        if (lastPlayedTimes.TryGetValue(clip, out float lastTime))
        {
            if (currentTime - lastTime < allowedCooldown)
                return false; //рано
        }
        // Шаг 4: Проверка на лимит одновременно играющих потоков звука
        if (activeSoundsCount.TryGetValue(clip, out int count))
        {
            if(count >= allowedMaxSounds)
                return false; //Лимит звуков 
        }

        //Если все проверки пройдены
        lastPlayedTimes[clip] = currentTime;
        return true;
    }
    public void RegisterSoundStart(AudioClip clip)
    {
        if (clip == null) return;

        activeSoundsCount.TryGetValue(clip, out int count);
        activeSoundsCount[clip] = count + 1; // Если ключа не было, count будет 0, и запишется 1
    }
    public void RegisterSoundEnd(AudioClip clip)
    {
        if(clip == null) return;
        if(activeSoundsCount.ContainsKey(clip) && activeSoundsCount[clip] > 0)
            activeSoundsCount[clip]--;  
    }

}
