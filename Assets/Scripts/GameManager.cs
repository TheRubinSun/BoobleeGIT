
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.GraphicsBuffer;


public class GameManager: MonoBehaviour 
{
    public static GameManager Instance;
    [SerializeField] GameObject CorpsePref;
    [SerializeField] GameObject MusicManager;
    [SerializeField] Transform Corpse_parent;
    [SerializeField] AudioClip[] musics;
    [SerializeField] private GameObject clouds;
    public GameObject GetClouds => clouds;

    public Transform mobsLayer;
    public Transform corpseLayer;
    private AudioSource music_source;

    public Transform dropParent;
    public Transform PlayerModel;
    public int countBosses;

    public float PlayerPosY;

    public int KillsEnemy;

    private int totalSecondsPlayed;
    private float sessionStartTime;

    public int enemisRemaining;
    public TextMeshProUGUI InfoReaminingEnemy;

    private string savePath;

    private bool isPaused = false;

    private bool playedBossMusic;
    private EffectsManager playerEffects;
    private Coroutine musicRoutine;

    private Dictionary<string, List<CorpsePool>> corpsePool = new Dictionary<string, List<CorpsePool>>();
    private void Awake()
    {
        GlobalData.LoadedGame = false;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dropParent == null) dropParent = GameObject.Find("DropItems").transform;
        if (PlayerModel == null) PlayerModel = GameObject.Find("PlayerModel").transform;
    }
    private IEnumerator Start()
    {
        yield return null;
        if (DisplayInfo.Instance != null) DisplayInfo.Instance.LoadDisplayInfo();
        if (GlobalData.Player != null)
        {
            GlobalData.Player.LoadPlayerLogic();
        }

        playerEffects = GlobalData.Player.GetComponent<EffectsManager>();

        music_source = MusicManager.GetComponent<AudioSource>();
        music_source.volume = GlobalData.VOLUME_MUSICS;
        music_source.loop = true;
        music_source.clip = musics[Random.Range(0, musics.Length)];
        music_source.Play();

        savePath = GlobalData.SavePath;
        if (savePath == null) savePath = "";

        if (LocalizationManager.Instance != null)
        {
            GlobalData.UIControl.LocalizationTranslate();
            Debug.Log("Локализация применена.");
        }
        else
        {
            Debug.LogError("Ошибка: Локализация не была загружена.");
        }
        if (GameDataHolder.PlayerData != null)
        {
            if(clouds != null)
            {
                if (GlobalData.OnClouds)
                {
                    clouds.SetActive(true);
                    Clouds.Instance.StartCloudsLogic();
                }  
                else
                {
                    Clouds.Instance.StopCloudsLogic();
                }
            }
            
            GlobalData.Artifacts.LoadOrNew(GameDataHolder.ArtifactsData.artifacts);
            GlobalWorld.LoadData(GameDataHolder.WorldData.numbTotalPoints, GameDataHolder.WorldData.farmPoints);
            GlobalData.Player.LoadOrCreateNew(GameDataHolder.PlayerData.player_data);
            GlobalData.Inventory.LoadOrCreateInventory(GameDataHolder.PlayerData.inventory_items_data);
            GlobalData.EqupmentPlayer.LoadOrCreateEquipment(GameDataHolder.PlayerData.equip_item_data);
            GlobalData.UIControl.LocalizationTranslate();
            LoadActiveEffect();

            SaveGameInfo dataInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
            KillsEnemy = 0;
            sessionStartTime = Time.realtimeSinceStartup; //Сохраняем настоящее время входа в игру
            if (dataInfo.godMode == true) GlobalData.Player.SetGodMode();
            else GlobalData.Player.SetSurvaveMode();

            GlobalData.UIControl.LoadButtons();
            Debug.Log("Игра загружена.");
        }
        else
        {
            Debug.LogError("Ошибка: данные из GameDataHolder не были загружены!");
        }

        yield return StartCoroutine(ChunkGenerator.Instance.GenerateChunks());
        if (GridNodes.Instance != null) GridNodes.Instance.CreateGrid();
        if (CullingManager.Instance != null) CullingManager.Instance.StartCulling();
        if (level_logic.Instance != null) level_logic.Instance.StartLevelLogic();
        ChunkGenerator.Instance.DeactivateAllChunks();
        if (UIControl.Instance != null) UIControl.Instance.LoadUI();
        if (ShopLogic.Instance != null) ShopLogic.Instance.LoadShopsData();
        if (GardenManager.instance != null) GardenManager.instance.CreateLoadFarmPoints();

        SaveGameInfo saveGameInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
        Debug.LogWarning($"Passed time: {saveGameInfo.timeHasPassed}");
        GlobalData.Player.GiveStartKit();

        GlobalData.LoadedGame = true;
        GlobalData.NeedLoadFile = false;

        yield return null;
    }
    private async void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // Игра уходит в фон
        {
            if (!isPaused)
            {
                await SaveOnlyPlayTime();
                isPaused = true;
            }
        }
        else // Игра возвращается из фона
        {
            sessionStartTime = Time.realtimeSinceStartup;
            isPaused = false;
        }
    }
    private async void OnApplicationQuit()
    {
        // Сохранение при выходе, если игра не была в паузе
        if (!isPaused)
        {
            await SaveOnlyPlayTime();
        }
    }
    //private void UpdatePlayTime()
    //{
    //    totalSecondsPlayed++;
    //}
    private void OnEnable()
    {
        BaseEnemyLogic.OnEnemyDeath += HandleEnemyDeath;
        BossLogic.OnBossDie += HandleBossDie;
        BossLogic.OnBossAdd += HandleBossAdd;
    }
    private void OnDisable()
    {
        BaseEnemyLogic.OnEnemyDeath -= HandleEnemyDeath;
        BossLogic.OnBossDie -= HandleBossDie;
        BossLogic.OnBossAdd -= HandleBossAdd;
    }
    private void HandleEnemyDeath(BaseEnemyLogic enemy)
    {
        KillsEnemy++;
        enemisRemaining--;
        //Debug.Log($"Убит {enemy.Name} {enemy.enum_stat.Max_Hp}");
        if(GlobalData.Player.GetHp() > 0)
        {
            GlobalData.Player.AddExp(enemy.enum_stat.GiveExp);
        }
        
        int chanceSpawnCorpse = UnityEngine.Random.Range(0, 100);
        if(chanceSpawnCorpse < 30) StartCoroutine(SpawnCorpse(enemy.mob_object.transform, enemy, false));
        else StartCoroutine(SpawnCorpse(enemy.mob_object.transform, enemy, true));

        if (InfoReaminingEnemy != null)
        {
            InfoReaminingEnemy.text = $"Убито врагов {KillsEnemy} из {enemisRemaining}";
        }
    }
    private IEnumerator FadeAndPlay(AudioClip newClip, float startTime)
    {
        while(music_source.volume > 0.05f)
        {
            music_source.volume -= Time.deltaTime;
            yield return null;
        }
        music_source.clip = newClip;
        music_source.time = startTime;
        music_source.Play();

        while(music_source.volume < 1f)
        {
            music_source.volume += Time.deltaTime;
            yield return null;
        }
    }
    private void PlayMusic(AudioClip clip, float startTime)
    {
        if(musicRoutine != null)
            StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(FadeAndPlay(clip, startTime));
    }

    private void HandleBossDie(BossLogic bossLogic)
    {
        countBosses--;
        if(countBosses <= 0)
        {
            PlayMusic(musics[Random.Range(0, musics.Length)], 0);
            playedBossMusic = false;
        }
    }
    private void HandleBossAdd(BossLogic bossLogic)
    {
        countBosses++;
        if (!playedBossMusic)
        {
            PlayMusic(bossLogic.GetBossMusic(), 7f);
            playedBossMusic = true;
        }

    }
    private CorpsePool GetCorpse(GameObject corpse_prefab)
    {
        GameObject corpse;

        string nameCorpse = corpse_prefab.name;
        if (!corpsePool.ContainsKey(nameCorpse))
            corpsePool[nameCorpse] = new List<CorpsePool>();

        List<CorpsePool> pool = corpsePool[nameCorpse];
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].Obj.activeInHierarchy)
            {
                CorpsePool corpseD = (CorpsePool)pool[i];
                corpse = corpseD.Obj;

                if (corpseD.Transform == null)
                    corpseD.NewTrans(corpse.transform);

                //NewPos(corpseD.Transform, newPos);
                return corpseD;
            }
        }
        corpse = Instantiate(corpse_prefab, Corpse_parent);
        CorpsePool newCorpse = new CorpsePool(corpse);
        corpse.tag = CorpsePref.tag;
        //NewPos(newCorpse.Transform, newPos);
        pool.Add(newCorpse);

        return newCorpse;
    }
    private void NewPos(Transform corpsePos, Vector2 newPos)
    {
        corpsePos.position = newPos;
    }
    private IEnumerator SpawnCorpse(Transform enemy, BaseEnemyLogic mob_logic, bool destroyCorpse)
    {
        //Создаем труп
        //GameObject corpseEnemy = Instantiate(CorpsePref, mob_logic.transform.parent);
        CorpsePool corpseEnemy = GetCorpse(CorpsePref);
        corpseEnemy.Obj.SetActive(true);
        corpseEnemy.Logic.StartCorpse();

        corpseEnemy.Transform.localScale = new Vector3(enemy.localScale.x, enemy.localScale.y, enemy.localScale.z);
        //Назначаем позицию
        corpseEnemy.Transform.position = enemy.transform.position;

        AudioClip die_sound = null;
        //Звук
        if (mob_logic.die_sounds.Length > 0)
        {
            die_sound = mob_logic.die_sounds[Random.Range(0, mob_logic.die_sounds.Length)];
            if (corpseEnemy.AudioS == null)
                corpseEnemy.AudioS = corpseEnemy.Obj.GetComponent<AudioSource>();
            TryPlaySound(die_sound, corpseEnemy.AudioS); //Звук смерти моба
            //corpseEnemy.AudioS.PlayOneShot(die_sound); //Звук смерти моба
        }


        //Спрайт
        corpseEnemy.SpriteRenderer.flipX = enemy.GetComponent<SpriteRenderer>().flipX; //Отразить  как нужно


        //Анимация
        Animator corpseAnim = corpseEnemy.Animator;//Берем аниматор трупа
        Animator enemyAnim = mob_logic.GetAnimator(); //Берем аниматор моба
        CopyAnim(enemyAnim, corpseAnim);

        yield return null;


        // Удаляем моба только после этого


        //Уничтожение трупа или нет
        if (!destroyCorpse)
        {
            Debug.LogWarning("Не уничтожать труп");
            if (mob_logic.typeMob == TypeMob.Technology)
                corpseEnemy.Obj.tag = "Corpse_Tech";
            else
                corpseEnemy.Obj.tag = "Corpse_Mag";
            CorpseSetting corpseSetting = corpseEnemy.Logic;
            corpseSetting.NameKey = mob_logic.Name;
        }
        else
        {
            StartCoroutine(WaitToDie(corpseEnemy.Obj, die_sound.length + 0.4f, corpseEnemy.AudioS));
        }
        yield return null;

        //Destroy(mob_logic.gameObject);
        GlobalData.CullingManager.UnregisterObject(mob_logic);
        mob_logic.gameObject.SetActive(false);
    }
    private void CopyAnim(Animator from, Animator to)
    {
        to.runtimeAnimatorController = from.runtimeAnimatorController;//Коприруем анимации
        to.fireEvents = false;  // Выключает все Animation Events
        to.SetTrigger("Death"); // Включаем анимацию смерти
    }
    private IEnumerator WaitToDie(GameObject corpse, float time, AudioSource AudioS)
    {
        yield return new WaitForSeconds(time);
        OnDisableAudio(AudioS);
        corpse.SetActive(false);
        //Destroy(corpse);
    }
    protected void TryPlaySound(AudioClip clip, AudioSource audioSource)
    {
        if (clip == null || audioSource == null) return;

        if (AudioManager.Instance != null && AudioManager.Instance.CanPlaySound(clip))
            StartCoroutine(PlaySoundRoutine(clip, audioSource));
    }
    protected IEnumerator PlaySoundRoutine(AudioClip clip, AudioSource audioSource)
    {
        AudioManager.Instance.RegisterSoundStart(clip);
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);

        AudioManager.Instance.RegisterSoundEnd(clip);
    }
    protected void OnDisableAudio(AudioSource audioSource)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.RegisterSoundEnd(audioSource.clip);
            }
        }
    }
    public async Task SaveAllData()
    {
        SaveGameInfo saveGameInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
        WritePlayTime(saveGameInfo);
        await SaveDataGame(saveGameInfo);
    }
    public async Task SaveDataGame(SaveGameInfo saveGameInfo = null)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, savePath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        List<SlotTypeSave> inventory_slots_list = new List<SlotTypeSave>();
        List<SlotTypeSave> equipment_item_list = new List<SlotTypeSave>();

        foreach (Slot slot in GlobalData.Inventory.slots)
        {
            inventory_slots_list.Add(new SlotTypeSave(slot.IdSlot,slot.Item.NameKey, slot.Count, slot.artifact_id));
        }

        foreach (Slot slot in GlobalData.EqupmentPlayer.SlotsEqup)
        {
            equipment_item_list.Add(new SlotTypeSave(slot.Item.NameKey, slot.Count, slot.artifact_id));
        }

        ItemsData items_Data = new ItemsData(ItemsList.items);
        //await SaveSystem.SaveDataAsync(items_Data, "items.json");

        RoleClassesData role_classes_data = new RoleClassesData(Classes.GetClasses());
        //await SaveSystem.SaveDataAsync(role_classes_data, "role_classes_data.json");

        PlayerData player_Data = new PlayerData(GlobalData.Player.GetPlayerStats() , inventory_slots_list, equipment_item_list);
        //await SaveSystem.SaveDataAsync(player_Data, savePath + "player.json");

        SaveDataBinds saveBinds = new SaveDataBinds(GlobalData.PlayerInputHandler.keyBindings);
        //await SaveSystem.SaveDataAsync(saveBinds, "keyBinds.json");

        ArtifactsData artifacts_Data = new ArtifactsData(GlobalData.Artifacts.artifacts);
        //await SaveSystem.SaveDataAsync(artifacts_Data, savePath + "artifacts.json");

        WorldData world_data = new WorldData(GlobalWorld.numbTotalPoints, GlobalWorld.FarmsPoints);
        //await SaveSystem.SaveDataAsync(world_data, savePath + "world_data.json");

        EnemyData enemy_Data = new EnemyData(EnemyList.mobs);
        //await SaveSystem.SaveDataAsync(enemy_Data, "enemies.json");

        ItemsDropOnEnemy item_drop = new ItemsDropOnEnemy(ItemDropEnemy.enemyAndHisDropItems);
        //await SaveSystem.SaveDataAsync(item_drop, "item_drop.json");

        ActiveEffectsData activeEffectsData = new ActiveEffectsData(playerEffects.GetActiveEffects());

        if (saveGameInfo == null)
            saveGameInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];

        //float sessionsDuration = Time.realtimeSinceStartup - sessionStartTime;
        //saveGameInfo.timeHasPassed += (int)sessionsDuration;
        saveGameInfo.enemy_kills += KillsEnemy;
        saveGameInfo.level = GlobalData.Player.GetLevel();
        saveGameInfo.isStarted = true;
        saveGameInfo.seed = GlobalData.CurSeed;
        saveGameInfo.lvl_left = GlobalData.cur_lvl_left;

        //saveGameIngo.randomCalls = GlobalData.randomCalls;
        ScreenResolutions screen_resole = GlobalData.GetScreenResolutions();
        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole, GlobalData.IsBigUI, GlobalData.IsFarCamera, GlobalData.OnClouds);
        //await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");

        CraftsRecipesData savesDataRecipesCrafts = new CraftsRecipesData(RecipesCraft.recipesCraft);
        //await SaveSystem.SaveDataAsync(savesDataRecipesCrafts, "recipes_crafts_data.json");

        GameDataHolder.PlayerData = player_Data;
        GameDataHolder.WorldData = world_data;
        GameDataHolder.ArtifactsData = artifacts_Data;
        GameDataHolder.ActiveEffectsData = activeEffectsData;

        var tasks = new List<Task>
        {
            SaveSystem.SaveDataAsync(items_Data, "items.json"),
            SaveSystem.SaveDataAsync(role_classes_data, "role_classes_data.json"),
            SaveSystem.SaveDataAsync(player_Data, savePath + "player.json"),
            SaveSystem.SaveDataAsync(saveBinds, "keyBinds.json"),
            SaveSystem.SaveDataAsync(artifacts_Data, savePath + "artifacts.json"),
            SaveSystem.SaveDataAsync(world_data, savePath + "world_data.json"),
            SaveSystem.SaveDataAsync(enemy_Data, "enemies.json"),
            SaveSystem.SaveDataAsync(item_drop, "item_drop.json"),
            SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json"),
            SaveSystem.SaveDataAsync(savesDataRecipesCrafts, "recipes_crafts_data.json"),
            SaveSystem.SaveDataAsync(activeEffectsData, savePath + "activeEffects.json"),
        };
        await Task.WhenAll(tasks);
    }
    public void WritePlayTime(SaveGameInfo saveGameInfo = null)
    {
        if(saveGameInfo == null)
             saveGameInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];

        if (!saveGameInfo.isStarted) return;

        float sessionsDuration = Time.realtimeSinceStartup - sessionStartTime;
        saveGameInfo.timeHasPassed += (int)sessionsDuration;
        sessionStartTime = Time.realtimeSinceStartup;
    }
    public async Task SaveOnlyPlayTime()
    {
        WritePlayTime();
        ScreenResolutions screen_resole = GlobalData.GetScreenResolutions();
        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole, GlobalData.IsBigUI, GlobalData.IsFarCamera, GlobalData.OnClouds);
        await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");
    }
    private void LoadActiveEffect()
    {
        foreach (EffectDataSave effect in GameDataHolder.ActiveEffectsData.active_ef_data)
        {
            EffectData loadedEffect = ScriptableObject.CreateInstance<EffectData>();

            EffectData effectTemplate = null;
            if (System.Enum.TryParse(effect.EffectName, out TypeEffectName type))
                effectTemplate = ResourcesData.GetEffectsPrefab(type);

            //Debug.Log($"ищём {effect.EffectName}");
            if (effectTemplate != null)
            {
                //Debug.Log($"Эффект с именем {effect.EffectName} найден");
                loadedEffect.effectObj = effectTemplate.effectObj;
                loadedEffect.Sprite = effectTemplate.Sprite;
            }
            loadedEffect.EffectName = effect.EffectName;
            loadedEffect.effectType = effect.effectType;
            loadedEffect.value = effect.value;
            loadedEffect.valueTwo = effect.valueTwo;
            loadedEffect.idSprite = effect.idSprite;
            loadedEffect.duration = effect.time_remains;
            loadedEffect.cooldown = effect.cooldown;
            loadedEffect.getTempStat = effect.getTempStat;
            playerEffects.ApplyEffect(loadedEffect);
        }

        //string path_player_data = Path.Combine(Application.persistentDataPath, GlobalData.SavePath + "activeEffects.json");
        //File.Delete(path_player_data);
    }
    //public void SaveActiveEffects()
    //{
    //    ActiveEffectsData activeEffectsData = new ActiveEffectsData(playerEffects.GetActiveEffects());
    //    SaveSystem.SaveDataAsync(activeEffectsData, savePath + "activeEffects.json");
    //}
    //public async void LoadDataGame()
    //{
    //    // Загрузка предметов
    //    ItemsData itemsData = await SaveSystem.LoadDataAsync<ItemsData>("items.json");
    //    ItemsList.Instance.LoadOrCreateItemList(itemsData.item_List_data);

    //    RoleClassesData role_Classes_data = await SaveSystem.LoadDataAsync<RoleClassesData>("role_classes_data");
    //    Classes.Instance.LoadOrCreateClasses(role_Classes_data.role_Classes_data);

    //    PlayerData playerData = await SaveSystem.LoadDataAsync<PlayerData>(savePath + "player.json");
    //    Player.Instance.LoadOrCreateNew(playerData.player_data);
    //    Inventory.Instance.LoadOrCreateInventory(playerData.inventory_items_data);
    //    EqupmentPlayer.Instance.LoadOrCreateEquipment(playerData.equip_item_data);

    //    EnemyData enemy = await SaveSystem.LoadDataAsync<EnemyData>("enemies.json");
    //    EnemyList.Instance.LoadOrCreateMobsList(enemy.mob_list_data);

    //    ItemsDropOnEnemy item_drop = await SaveSystem.LoadDataAsync<ItemsDropOnEnemy>("item_drop.json");
    //    ItemDropEnemy.LoadOrCreate(item_drop.namesKeys);

    //    UIControl.Instance.LocalizationTranslate();
    //    Debug.Log("Игра загружена.");
    //}


    public void SpawnMobs()
    {

    }
}
public class CorpsePool : IPoolData
{
    public GameObject Obj { get; private set; }
    public Transform Transform { get; private set; }
    public CorpseSetting Logic { get; private set; }
    public AudioSource AudioS { get; set; }
    public Animator Animator { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public CorpsePool(GameObject obj)
    {
        Obj = obj;
        Transform = obj.transform;
        Logic = obj.GetComponent<CorpseSetting>();
        AudioS = obj.GetComponent<AudioSource>();
        Animator = obj.GetComponent<Animator>();
        SpriteRenderer = obj.GetComponent<SpriteRenderer>();
    }
    public void NewTrans(Transform enemyTrans)
    {
        Transform = enemyTrans;
    }
}

