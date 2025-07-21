
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
    private float lastRealTime;

    public int enemisRemaining;
    public TextMeshProUGUI InfoReaminingEnemy;

    private string savePath;
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
            UIControl.Instance.LocalizationTranslate();
            Debug.Log("Локализация применена.");
        }
        else
        {
            Debug.LogError("Ошибка: Локализация не была загружена.");
        }


        if (GameDataHolder.PlayerData != null)
        {
            //ItemsList.LoadOrCreateItemList(GameDataHolder.ItemsData.item_List_data);
            Classes.Instance.LoadOrCreateClasses(GameDataHolder.RoleClassesData.role_Classes_data);


            Artifacts.Instance.LoadOrNew(GameDataHolder.ArtifactsData.artifacts);


            GlobalWorld.LoadData(GameDataHolder.WorldData.numbTotalPoints, GameDataHolder.WorldData.farmPoints);


            Player.Instance.LoadOrCreateNew(GameDataHolder.PlayerData.player_data);

            Inventory.Instance.LoadOrCreateInventory(GameDataHolder.PlayerData.inventory_items_data);
            EqupmentPlayer.Instance.LoadOrCreateEquipment(GameDataHolder.PlayerData.equip_item_data);

            //EnemyList.LoadOrCreateMobsList(GameDataHolder.EnemyData.mob_list_data);
            ItemDropEnemy.LoadOrCreate(GameDataHolder.ItemsDropOnEnemy.namesKeys);

            UIControl.Instance.LocalizationTranslate();

            SaveGameInfo dataInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
            KillsEnemy = dataInfo.enemy_kills;
            totalSecondsPlayed = dataInfo.timeHasPassed;

            lastRealTime = Time.realtimeSinceStartup; //Сохраняем настоящее время входа в игру

            if (dataInfo.godMode == true) Player.Instance.SetGodMode();
            else Player.Instance.SetSurvaveMode();

            UIControl.Instance.LoadButtons();
            Debug.Log("Игра загружена.");
        }
        else
        {
            Debug.LogError("Ошибка: данные из GameDataHolder не были загружены!");
        }

        StartCoroutine(TrackPlayTime(5));
        //InvokeRepeating(nameof(UpdatePlayTime), 1f, 1f);
    }
    private IEnumerator TrackPlayTime(int timer)
    {
        //float lastTime = Time.realtimeSinceStartup;
        while (true)
        {
            yield return new WaitForSecondsRealtime(timer);

            //float currentTime = Time.realtimeSinceStartup;
            //float delta = currentTime - lastTime;
            //lastTime = currentTime;
            totalSecondsPlayed += timer;
        }
    }
    private async void OnApplicationQuit()
    {
        await SavePlayTime();
    }
    public async Task SavePlayTime()
    {
        SaveGameInfo saveGameIngo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
        if (!saveGameIngo.isStarted) return;

        if(totalSecondsPlayed > saveGameIngo.timeHasPassed) saveGameIngo.timeHasPassed = totalSecondsPlayed;

        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);
        await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");
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
        if(Player.Instance.GetHp() > 0)
        {
            Player.Instance.AddExp(enemy.enum_stat.GiveExp);
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
    public async Task SaveDataGame()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, savePath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        List<SlotTypeSave> inventory_slots_list = new List<SlotTypeSave>();
        List<SlotTypeSave> equipment_item_list = new List<SlotTypeSave>();

        foreach (Slot slot in Inventory.Instance.slots)
        {
            inventory_slots_list.Add(new SlotTypeSave(slot.IdSlot,slot.Item.NameKey, slot.Count, slot.artifact_id));
        }

        foreach (Slot slot in EqupmentPlayer.Instance.SlotsEqup)
        {
            equipment_item_list.Add(new SlotTypeSave(slot.Item.NameKey, slot.Count, slot.artifact_id));
        }

        ItemsData items_Data = new ItemsData(ItemsList.items);
        await SaveSystem.SaveDataAsync(items_Data, "items.json");

        RoleClassesData role_classes_data = new RoleClassesData(Classes.Instance.GetClasses());
        await SaveSystem.SaveDataAsync(role_classes_data, "role_classes_data.json");

        PlayerData player_Data = new PlayerData(Player.Instance.GetPlayerStats() , inventory_slots_list, equipment_item_list);
        await SaveSystem.SaveDataAsync(player_Data, savePath + "player.json");

        ArtifactsData artifacts_Data = new ArtifactsData(Artifacts.Instance.artifacts);
        await SaveSystem.SaveDataAsync(artifacts_Data, savePath + "artifacts.json");

        WorldData world_data = new WorldData(GlobalWorld.numbTotalPoints, GlobalWorld.FarmsPoints);
        await SaveSystem.SaveDataAsync(world_data, savePath + "world_data.json");

        EnemyData enemy_Data = new EnemyData(EnemyList.mobs);
        await SaveSystem.SaveDataAsync(enemy_Data, "enemies.json");

        ItemsDropOnEnemy item_drop = new ItemsDropOnEnemy(ItemDropEnemy.enemyAndHisDropItems);
        await SaveSystem.SaveDataAsync(item_drop, "item_drop.json");

        SaveGameInfo saveGameIngo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];

        saveGameIngo.timeHasPassed = totalSecondsPlayed;
        saveGameIngo.enemy_kills = KillsEnemy;
        saveGameIngo.level = Player.Instance.GetLevel();
        saveGameIngo.isStarted = true;
        saveGameIngo.seed = GlobalData.cur_seed;
        saveGameIngo.lvl_left = GlobalData.cur_lvl_left;
        //saveGameIngo.randomCalls = GlobalData.randomCalls;
        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language, GlobalData.VOLUME_SOUNDS, GlobalData.VOLUME_MUSICS);
        await SaveSystem.SaveDataAsync(savesDataInfo, "saves_info.json");

        CraftsRecipesData savesDataRecipesCrafts = new CraftsRecipesData(RecipesCraft.recipesCraft);
        await SaveSystem.SaveDataAsync(savesDataRecipesCrafts, "recipes_crafts_data.json");
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

