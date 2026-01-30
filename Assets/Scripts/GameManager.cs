
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using System.IO;
using System.Collections;
using System.Threading.Tasks;


public class GameManager: MonoBehaviour 
{
    public static GameManager Instance;
    [SerializeField] GameObject CorpsePref;
    [SerializeField] GameObject AudioManager;
    [SerializeField] AudioClip[] musics;
    public Transform mobsLayer;
    private AudioSource music_source;

    public Transform dropParent;
    public Transform PlayerModel;

    public float PlayerPosY;

    public int KillsEnemy;

    private int totalSecondsPlayed;
    private float sessionStartTime;

    public int enemisRemaining;
    public TextMeshProUGUI InfoReaminingEnemy;

    private string savePath;

    private bool isPaused = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dropParent == null) dropParent = GameObject.Find("DropItems").transform;
        if (PlayerModel == null) PlayerModel = GameObject.Find("PlayerModel").transform;
    }
    private void Start()
    {
        music_source = AudioManager.GetComponent<AudioSource>();
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
            //ItemsList.LoadOrCreateItemList(GameDataHolder.ItemsData.item_List_data);
            GlobalData.Classes.LoadOrCreateClasses(GameDataHolder.RoleClassesData.role_Classes_data);


            GlobalData.Artifacts.LoadOrNew(GameDataHolder.ArtifactsData.artifacts);


            GlobalWorld.LoadData(GameDataHolder.WorldData.numbTotalPoints, GameDataHolder.WorldData.farmPoints);


            GlobalData.Player.LoadOrCreateNew(GameDataHolder.PlayerData.player_data);

            GlobalData.Inventory.LoadOrCreateInventory(GameDataHolder.PlayerData.inventory_items_data);
            GlobalData.EqupmentPlayer.LoadOrCreateEquipment(GameDataHolder.PlayerData.equip_item_data);

            //EnemyList.LoadOrCreateMobsList(GameDataHolder.EnemyData.mob_list_data);
            ItemDropEnemy.LoadOrCreate(GameDataHolder.ItemsDropOnEnemy.namesKeys);

            GlobalData.UIControl.LocalizationTranslate();

            SaveGameInfo dataInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
            KillsEnemy = 0;
            //totalSecondsPlayed = dataInfo.timeHasPassed;

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
        SaveGameInfo saveGameInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
        Debug.LogWarning($"Passed time: {saveGameInfo.timeHasPassed}");
        //StartCoroutine(TrackPlayTime(5));
        //InvokeRepeating(nameof(UpdatePlayTime), 1f, 1f);
    }
    //private IEnumerator TrackPlayTime(int timer)
    //{
    //    //float lastTime = Time.realtimeSinceStartup;
    //    while (true)
    //    {
    //        yield return new WaitForSecondsRealtime(timer);

    //        //float currentTime = Time.realtimeSinceStartup;
    //        //float delta = currentTime - lastTime;
    //        //lastTime = currentTime;
    //        totalSecondsPlayed += timer;
    //    }
    //}
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
    }
    private void OnDisable()
    {
        BaseEnemyLogic.OnEnemyDeath -= HandleEnemyDeath;
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
    private IEnumerator SpawnCorpse(Transform enemy, BaseEnemyLogic mob_logic, bool destroyCorpse)
    {
        //Создаем труп
        GameObject corpseEnemy = Instantiate(CorpsePref, mob_logic.transform.parent);
        corpseEnemy.transform.localScale = new Vector3(enemy.localScale.x, enemy.localScale.y, enemy.localScale.z);
        //Назначаем позицию
        corpseEnemy.transform.position = enemy.transform.position;

        AudioClip die_sound = null;
        //Звук
        if (mob_logic.die_sounds.Length > 0)
        {
            die_sound = mob_logic.die_sounds[Random.Range(0, mob_logic.die_sounds.Length)];
            corpseEnemy.GetComponent<AudioSource>().PlayOneShot(die_sound); //Звук смерти моба
        }


        //Спрайт
        corpseEnemy.GetComponent<SpriteRenderer>().flipX = enemy.GetComponent<SpriteRenderer>().flipX; //Отразить  как нужно


        //Анимация
        Animator corpseAnim = corpseEnemy.GetComponent<Animator>();//Берем аниматор трупа
        Animator enemyAnim = mob_logic.GetAnimator(); //Берем аниматор моба
        CopyAnim(enemyAnim, corpseAnim);

        yield return null;


        // Удаляем моба только после этого


        //Уничтожение трупа или нет
        if (!destroyCorpse)
        {
            Debug.LogWarning("Не уничтожать труп");
            if (mob_logic.typeMob == TypeMob.Technology)
                corpseEnemy.tag = "Corpse_Tech";
            else
                corpseEnemy.tag = "Corpse_Mag";
            CorpseSetting corpseSetting = corpseEnemy.GetComponent<CorpseSetting>();
            corpseSetting.NameKey = mob_logic.Name;
        }
        else
        {
            StartCoroutine(WaitToDie(corpseEnemy, die_sound.length + 0.4f));
        }
        yield return null;

        Destroy(mob_logic.gameObject);
    }
    private void CopyAnim(Animator from, Animator to)
    {
        to.runtimeAnimatorController = from.runtimeAnimatorController;//Коприруем анимации
        to.fireEvents = false;  // Выключает все Animation Events
        to.SetTrigger("Death"); // Включаем анимацию смерти
    }
    private IEnumerator WaitToDie(GameObject corpse, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(corpse);
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

        RoleClassesData role_classes_data = new RoleClassesData(GlobalData.Classes.GetClasses());
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


        if (saveGameInfo == null)
            saveGameInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];

        //float sessionsDuration = Time.realtimeSinceStartup - sessionStartTime;
        //saveGameInfo.timeHasPassed += (int)sessionsDuration;
        saveGameInfo.enemy_kills += KillsEnemy;
        saveGameInfo.level = GlobalData.Player.GetLevel();
        saveGameInfo.isStarted = true;
        saveGameInfo.seed = GlobalData.cur_seed;
        saveGameInfo.lvl_left = GlobalData.cur_lvl_left;

        //saveGameIngo.randomCalls = GlobalData.randomCalls;
        ScreenResolutions screen_resole = GlobalData.GetScreenResolutions();
        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole);
        //await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");

        CraftsRecipesData savesDataRecipesCrafts = new CraftsRecipesData(RecipesCraft.recipesCraft);
        //await SaveSystem.SaveDataAsync(savesDataRecipesCrafts, "recipes_crafts_data.json");

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
        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS, screen_resole);
        await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");
    }
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

