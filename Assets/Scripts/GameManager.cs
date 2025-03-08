
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;


public class GameManager: MonoBehaviour 
{
    public static GameManager Instance;
    [SerializeField] GameObject CorpsePref; 
    bool BuildingMode;

    public int KillsEnemy;
    public int totalSecondsPlayed;

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
    }
    private void Start()
    {
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
            ItemsList.Instance.LoadOrCreateItemList(GameDataHolder.ItemsData.item_List_data);
            Classes.Instance.LoadOrCreateClasses(GameDataHolder.RoleClassesData.role_Classes_data);

            Player.Instance.LoadOrCreateNew(GameDataHolder.PlayerData.player_data);
            Inventory.Instance.LoadOrCreateInventory(GameDataHolder.PlayerData.inventory_items_data);
            EqupmentPlayer.Instance.LoadOrCreateEquipment(GameDataHolder.PlayerData.equip_item_data);

            EnemyList.Instance.LoadOrCreateMobsList(GameDataHolder.EnemyData.mob_list_data);
            ItemDropEnemy.LoadOrCreate(GameDataHolder.ItemsDropOnEnemy.namesKeys);

            UIControl.Instance.LocalizationTranslate();

            SaveGameInfo dataInfo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
            KillsEnemy = dataInfo.enemy_kills;
            totalSecondsPlayed = dataInfo.timeHasPassed;

            Debug.Log("Игра загружена.");
        }
        else
        {
            Debug.LogError("Ошибка: данные из GameDataHolder не были загружены!");
        }


        InvokeRepeating(nameof(UpdatePlayTime), 1f, 1f);
    }
    private void UpdatePlayTime()
    {
        totalSecondsPlayed++;
    }
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
        Debug.Log($"Убит {enemy.Name} {enemy.max_Hp}");
        Player.Instance.AddExp(enemy.GiveExp);

        int chanceSpawnCorpse = UnityEngine.Random.Range(1, 10);
        if(chanceSpawnCorpse < 4) SpawnCorpse(enemy.gameObject.transform);



        if (InfoReaminingEnemy != null)
        {
            InfoReaminingEnemy.text = $"Убито врагов {KillsEnemy} из {enemisRemaining}";
        }
    }
    public void SpawnCorpse(Transform enemy)
    {
        GameObject corpseEnemy = Instantiate(CorpsePref, enemy.parent); //Создаем труп
        corpseEnemy.transform.position = enemy.transform.position;      //Назначаем позицию

        if (enemy.gameObject.GetComponent<BaseEnemyLogic>().typeMob == TypeMob.Technology) 
            corpseEnemy.tag = "Corpse_Tech";
        else 
            corpseEnemy.tag = "Corpse_Mag";

        corpseEnemy.GetComponent<CorpseSetting>().NameKey = enemy.GetComponent<BaseEnemyLogic>().Name;
        corpseEnemy.GetComponent<SpriteRenderer>().flipX = enemy.GetComponent<SpriteRenderer>().flipX;

        Animator corpseAnim = corpseEnemy.GetComponent<Animator>();    //Коприруем аниматор
        Animator enemyAnim = enemy.gameObject.GetComponent<BaseEnemyLogic>().GetAnimator(); //Коприруем аниматор
        
        corpseAnim.runtimeAnimatorController = enemyAnim.runtimeAnimatorController;        //Коприруем аниматор
        corpseAnim.fireEvents = false;  // Выключает все Animation Events
        corpseAnim.SetTrigger("Death");


    }
    public async void SaveDataGame()
    {
        
        List<SlotTypeSave> inventory_slots_list = new List<SlotTypeSave>();
        List<SlotTypeSave> equipment_item_list = new List<SlotTypeSave>();

        foreach (Slot slot in Inventory.Instance.slots)
        {
            inventory_slots_list.Add(new SlotTypeSave(slot.IdSlotInv,slot.Item.NameKey, slot.Count));
        }

        foreach (Slot slot in EqupmentPlayer.Instance.slotsEqup)
        {
            equipment_item_list.Add(new SlotTypeSave(slot.Item.NameKey, slot.Count));
        }

        ItemsData items_Data = new ItemsData(ItemsList.Instance.items);
        await SaveSystem.SaveDataAsync(items_Data, "items.json");

        RoleClassesData role_classes_data = new RoleClassesData(Classes.Instance.GetClasses());
        await SaveSystem.SaveDataAsync(role_classes_data, "role_classes_data.json");

        PlayerData player_Data = new PlayerData(Player.Instance.GetPlayerStats() , inventory_slots_list, equipment_item_list);
        await SaveSystem.SaveDataAsync(player_Data, savePath + "player.json");

        EnemyData enemy_Data = new EnemyData(EnemyList.Instance.mobs);
        await SaveSystem.SaveDataAsync(enemy_Data, "enemies.json");

        ItemsDropOnEnemy item_drop = new ItemsDropOnEnemy(ItemDropEnemy.enemyAndHisDrop);
        await SaveSystem.SaveDataAsync(item_drop, "item_drop.json");

        SaveGameInfo saveGameIngo = GenInfoSaves.saveGameFiles[GlobalData.SaveInt];
        saveGameIngo.timeHasPassed = totalSecondsPlayed;
        saveGameIngo.enemy_kills = KillsEnemy;
        saveGameIngo.level = Player.Instance.GetLevel();
        saveGameIngo.isStarted = true;
        SavesDataInfo savesDataInfo = new SavesDataInfo(GenInfoSaves.saveGameFiles, GlobalData.SaveInt, GlobalData.cur_language);
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

