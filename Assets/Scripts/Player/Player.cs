using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; set; }

    public bool[] DirectionOrVectorWeapon = new bool[4];

    private PlayerStats pl_stats;
    
    [SerializeField] 
    private EquipStats equip_Stats;
    private PlayerUI pl_ui;

    //GameObjects
    private Dictionary<int, WeaponControl> WeaponsObj = new Dictionary<int, WeaponControl>();
    private Dictionary<int, MinionControl> MinionsObj = new Dictionary<int, MinionControl>();

    [SerializeField] 
    private GameObject PlayerModel;

    [SerializeField] 
    public Toggle[] TooglesWeapon = new Toggle[4];

    SpriteRenderer player_sprite;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pl_stats = new PlayerStats();
        pl_ui = GetComponent<PlayerUI>();

        pl_ui.SetComponentUI();

        player_sprite = PlayerModel.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {

    }
    public void LoadOrCreateNew(PlayerStats playerSaveData)
    {
        if(playerSaveData != null && playerSaveData.Max_Hp > 0)
        {
            pl_stats.LoadStats(playerSaveData);
        }
        else
        {
            pl_stats.SetBaseStats();
        }
        pl_stats.UpdateTotalStats();
        pl_stats.FillHp();

        pl_ui.UpdateAllInfo(pl_stats);
        ResetWeaponToggles();
        ChangeToggleWeapon();
    }
    public PlayerStats GetPlayerStats() => pl_stats;
    public EquipStats GetEquipStats() => equip_Stats;
    private void ResetWeaponToggles()
    {

        for (int i = 0; i < DirectionOrVectorWeapon.Length; i++)
        {
            DirectionOrVectorWeapon[i] = false;
            TooglesWeapon[i].isOn = DirectionOrVectorWeapon[i];
        }
    }

    public Dictionary<int, WeaponControl> GetDictWeaponAndArms()
    {
        return WeaponsObj;
    }
    public Dictionary<int, MinionControl> GetDictMinions()
    {
        return MinionsObj;
    }
    private void UpdateSlotsInPlayerControl()
    {
        transform.GetChild(0).GetComponent<PlayerControl>().UpdateSlots(WeaponsObj, MinionsObj);
    }
    public void SetWeaponsObj(int i, WeaponControl weaponObj)
    {
        WeaponsObj[i] = weaponObj;
        UpdateSlotsInPlayerControl();
    }
    public void SetMinionsObj(int i, MinionControl minionObj)
    {
        MinionsObj[i] = minionObj;
        UpdateSlotsInPlayerControl();
    }

    private async Task FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if (player_sprite == null) return;

        player_sprite.color = color;

        float elapsed = 0;
        while (elapsed < time)
        {
            if(Time.timeScale > 0)// Чтобы учитывать паузу
            {
                elapsed += Time.deltaTime;
            }

            await Task.Yield();// Ждать следующий кадр без блокировки поток
        }

        //await Task.Delay((int)(time * 1000));
        player_sprite.color = new Color32(255, 255, 255, 255);
    }
    private void IsDeath()
    {
        if (pl_stats.Cur_Hp < 1)
        {
            PlayerModel.SetActive(false);
        }
    }

    public void ChangeToggleWeapon()
    {
        for (int i = 0; i < DirectionOrVectorWeapon.Length; i++)
        {
            if(WeaponsObj.ContainsKey(i))
            {
                WeaponsObj[i].AttackDirectionOrVector = TooglesWeapon[i].isOn;
            }
            
        }
    }
    public GameObject CreateTrap(GameObject trap)
    {
        GameObject trapObj = Instantiate(trap, null);
        trapObj.transform.position = PlayerModel.transform.position;
        return trapObj;
    }


    ///Player stats and player UI
    public void AddMaxHP(int addMaxHp)
    {
        pl_stats.AddMaxHPStat(addMaxHp);
        pl_ui.UpdateHpBar(pl_stats);
        pl_ui.UpdateSizeHpBar(pl_stats);
    }
    public async void TakeDamage(int damage, bool canEvade)
    {
        if(pl_stats.TakeDamageStat(damage, canEvade))
        {
            pl_ui.UpdateHpBar(pl_stats);
            await FlashColor(new Color32(255, 108, 108, 255), 0.1f);
            IsDeath();
        }
        else
        {
            Debug.Log("Промах от врага");
        }
    }
    public bool PlayerHeal(int count_heal)
    {
        bool chech = pl_stats.PlayerHealStat(count_heal);
        pl_ui.UpdateHpBar(pl_stats);
        return chech;
    }
    public void AddExp(int add_exp)
    {
        pl_stats.AddExpStat(add_exp);
        pl_ui.UpdateExpBar(pl_stats);
    }
    public void LvlUp(string lvlup_text)
    {
        pl_ui.LvlUpUI(lvlup_text);
    }
}
