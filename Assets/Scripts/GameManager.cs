using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

public class GameManager: MonoBehaviour 
{
    public static GameManager Instance;
    [SerializeField] GameObject CorpsePref; 
    bool BuildingMode;

    public int KillsEnemy = 0;
    public int enemisRemaining = 0;
    public TextMeshProUGUI InfoReaminingEnemy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadDataGame();
    }
    private void OnEnable()
    {
        EnemySetting.OnEnemyDeath += HandleEnemyDeath;
    }
    private void OnDisable()
    {
        EnemySetting.OnEnemyDeath -= HandleEnemyDeath;
    }
    private void HandleEnemyDeath(EnemySetting enemy)
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

        corpseEnemy.GetComponent<CorpseSetting>().NameKey = enemy.GetComponent<EnemySetting>().Name;
        corpseEnemy.GetComponent<SpriteRenderer>().flipX = enemy.GetComponent<SpriteRenderer>().flipX;
        Animator corpseAnim = corpseEnemy.GetComponent<Animator>();    //Коприруем аниматор
        Animator enemyAnim = enemy.gameObject.GetComponent<EnemyControl>().GetAnimator(); //Коприруем аниматор
        corpseAnim.runtimeAnimatorController = enemyAnim.runtimeAnimatorController;        //Коприруем аниматор
        corpseAnim.SetTrigger("Death");
    }
    public async void SaveDataGame()
    {
        
        List<SlotTypeSave> inventory_slots_list = new List<SlotTypeSave>();
        List<SlotTypeSave> equipment_item_list = new List<SlotTypeSave>();

        foreach (Slot slot in Inventory.Instance.slots)
        {
            inventory_slots_list.Add(new SlotTypeSave(slot.Item.NameKey, slot.Count));
        }

        foreach (Slot slot in EqupmentPlayer.Instance.slotsEqup)
        {
            equipment_item_list.Add(new SlotTypeSave(slot.Item.NameKey, slot.Count));
        }

        ItemsData items_Data = new ItemsData(ItemsList.Instance.items);
        await SaveSystem.SaveDataAsync(items_Data, "items.json");

        PlayerData player_Data = new PlayerData(Classes.Instance.GetClasses(), Player.Instance , inventory_slots_list, equipment_item_list);
        await SaveSystem.SaveDataAsync(player_Data, "player.json");

        EnemyData enemy_Data = new EnemyData(EnemyList.Instance.mobs);
        await SaveSystem.SaveDataAsync(enemy_Data, "enemies.json");

        ItemsDropOnEnemy item_drop = new ItemsDropOnEnemy(ItemDropEnemy.enemyAndHisDrop);
        await SaveSystem.SaveDataAsync(item_drop, "item_drop.json");
    }
    public async void LoadDataGame()
    {
        // Загрузка предметов
        ItemsData itemsData = await SaveSystem.LoadDataAsync<ItemsData>("items.json");
        ItemsList.Instance.LoadOrCreateItemList(itemsData.item_List_data);

        PlayerData playerData = await SaveSystem.LoadDataAsync<PlayerData>("player.json");
        Classes.Instance.LoadOrCreateClasses(playerData.role_Classes_data);
        Player.Instance.LoadOrCreateNew(playerData.player_data);
        Inventory.Instance.LoadOrCreateInventory(playerData.inventory_items_data);
        EqupmentPlayer.Instance.LoadOrCreateEquipment(playerData.equip_item_data);

        EnemyData enemy = await SaveSystem.LoadDataAsync<EnemyData>("enemies.json");
        EnemyList.Instance.LoadOrCreateMobsList(enemy.mob_list_data);

        ItemsDropOnEnemy item_drop = await SaveSystem.LoadDataAsync<ItemsDropOnEnemy>("item_drop.json");
        ItemDropEnemy.LoadOrCreate(item_drop.namesKeys);

        Debug.Log("Игра загружена.");
    }


    public void SpawnMobs()
    {

    }
}

