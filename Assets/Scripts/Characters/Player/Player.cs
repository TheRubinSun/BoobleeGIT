using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, ITakeDamage
{
    public static Player Instance { get; set; }
    public bool GodMode { get; private set; }
    public bool PlayerStay {  get; set; }
    [SerializeField] private Transform spawnPoint;
    private PlayerStats pl_stats;
    private BuffsStats bf_stats;
    [SerializeField] 
    private EquipStats equip_Stats;

    private PlayerUI pl_ui;

    //GameObjects
    private readonly Dictionary<int, WeaponControl> WeaponsObj = new();
    private readonly Dictionary<int, MinionControl> MinionsObj = new();
    private PlayerControl playerControl;

    [SerializeField] 
    private GameObject PlayerModel;
    //[SerializeField]
    //private GameObject[] PlayerLegs;

    [SerializeField] 
    public Toggle[] TooglesWeapon = new Toggle[4];

    SpriteRenderer player_sprite;

    //Abillity
    private bool canForce = true;
    private float cooldownForce = 0.3f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pl_stats = new PlayerStats();
        bf_stats = new BuffsStats();
        pl_ui = GetComponent<PlayerUI>();
        equip_Stats = GetComponent<EquipStats>();
        playerControl = transform.GetChild(0).GetComponent<PlayerControl>();

        pl_ui.SetComponentUI();

        player_sprite = PlayerModel.GetComponent<SpriteRenderer>();

        StartCoroutine(AddManaForRegen());
    }
    public void LoadOrCreateNew(PlayerStats playerSaveData)
    {
        if (playerSaveData != null && playerSaveData.Base_Max_Hp > 0)
        {
            pl_stats.LoadStats(playerSaveData);
            LoadWeaponToggles();
            pl_stats.UpdateTotalStats();
            if (GetFreeSkillPoint() > 0) UIControl.Instance.ShowHideLvlUP(true);
        }
        else
        {
            pl_stats.SetBaseStats();
            pl_stats.UpdateTotalStats();
            pl_stats.FillHp();
            pl_stats.FillMana();
            ResetWeaponToggles();
        }
        if (spawnPoint != null)
        {
            foreach(Transform child in gameObject.transform)
            {
                child.position = spawnPoint.transform.position;
            }
            //PlayerModel.transform.position = spawnPoint.transform.position;
            //foreach (GameObject leg in PlayerLegs)
            //{
            //    leg.transform.position = spawnPoint.transform.position;
            //}
        }

        pl_ui.UpdateAllInfo(pl_stats);
        ChangeToggleWeapons();
    }
    public Vector2 GetPosPlayer() => playerControl.transform.position;
    public void UpdateHP()
    {
        pl_ui.UpdateHpBar(pl_stats);
    }
    public void UpdateMANA()
    {
        pl_ui.UpdateManaBar(pl_stats);
    }
    public void UpdateRegenMANA()
    {
        pl_ui.UpdateRegenMana(pl_stats);
    }
    public void UpdateNeededStat(AllParametrs parametrs, int multiply)
    {
        pl_stats.ApplyStat(parametrs, multiply);
    }
    public IEnumerator AddManaForRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            UpdateMANA();
            pl_stats.RegenMana();
        }
    }
    public PlayerStats GetPlayerStats() => pl_stats;
    public BuffsStats GetBuffStatsPlayer() => bf_stats;
    public EquipStats GetEquipStats() => equip_Stats;
    public int GetGold() => pl_stats.Gold;
    public int GetSkillsTrader() => pl_stats.TraderSkill;
    public int GetLevel() => pl_stats.level;
    public int GetFreeSkillPoint() => pl_stats.freeSkillPoints;
    public Dictionary<int, WeaponControl> GetDictWeaponAndArms() => WeaponsObj;
    public Dictionary<int, MinionControl> GetDictMinions() => MinionsObj;
    public int PayGold(int cost)
    {
        return pl_stats.Gold += cost;
    }

    private void UpdateSlotsInPlayerControl()
    {
        playerControl.UpdateSlots(WeaponsObj, MinionsObj);
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
    public void UpdateAllStats()
    {
        pl_stats.UpdateTotalStats();
    }
    public void SetAspect(AspectName aspectName, float value)
    {
        pl_stats.freeSkillPoints--;
        switch (aspectName)
        {
            case AspectName.Agillity:
                pl_stats.Base_Agility += (int)value;
                break;

            case AspectName.Strength:
                pl_stats.Base_Strength += (int)value;
                break;

            case AspectName.Intelligence:
                pl_stats.Base_Intelligence += (int)value;
                break;

            case AspectName.Tech_Point:
                pl_stats.TechniquePoints += (int)value;
                break;

            case AspectName.Mage_Point:
                pl_stats.MagicPoints += (int)value;
                break;

            case AspectName.Exp_Bust:
                pl_stats.Base_ExpBust += value;
                break;

            case AspectName.Speed:
                pl_stats.Base_Mov_Speed += value;
                break;

            case AspectName.Hp:
                AddMaxHP((int)value);
                break;

            case AspectName.Gold:
                pl_stats.Gold += ((int)value);
                break;

            case AspectName.AttackSpeed:
                pl_stats.Base_Att_Speed += (int)value;
                break;
            case AspectName.Range:
                pl_stats.Base_Att_Range += value; // Увеличение дальности атаки
                break;

            case AspectName.Projectile_speed:
                pl_stats.Base_Proj_Speed += value; // Увеличение скорости снаряда
                break;
            case AspectName.Damage:
                pl_stats.Base_Att_Damage += (int)value;
                break;
            case AspectName.Mana:
                AddMaxMana((int)value);
                break;
            case AspectName.ManaRegen:
                pl_stats.Base_Regen_Mana += value;
                break;
            default:
                Debug.LogWarning("Неизвестный аспект: " + aspectName);
                break;
        }

        UpdateAllUiInfo();
    }

    private void IsDeath()
    {
        if (pl_stats.Cur_Hp < 1)
        {
            PlayerModel.SetActive(false);
        }
    }
    public void ChangeToggleWeapon(int id)
    {
        pl_stats.DirectionOrVectorWeapon[id] = TooglesWeapon[id].isOn;
        if (WeaponsObj.ContainsKey(id))
        {
            WeaponsObj[id].AttackDirectionOrVector = TooglesWeapon[id].isOn;

        }
    }
    public void ChangeToggleWeapons()
    {
        for (int i = 0; i < pl_stats.DirectionOrVectorWeapon.Length; i++)
        {
            pl_stats.DirectionOrVectorWeapon[i] = TooglesWeapon[i].isOn;
            if (WeaponsObj.ContainsKey(i))
            {
                WeaponsObj[i].AttackDirectionOrVector = TooglesWeapon[i].isOn;
            }
        }
    }
    private void LoadWeaponToggles()
    {
        for (int i = 0; i < pl_stats.DirectionOrVectorWeapon.Length; i++)
        {
            TooglesWeapon[i].isOn = pl_stats.DirectionOrVectorWeapon[i];
        }
    }
    private void ResetWeaponToggles()
    {
        for (int i = 0; i < pl_stats.DirectionOrVectorWeapon.Length; i++)
        {
            pl_stats.DirectionOrVectorWeapon[i] = false;
            TooglesWeapon[i].isOn = pl_stats.DirectionOrVectorWeapon[i];
        }
    }
    public void AddAttribute(BoosterType boosterType, int count)
    {
        pl_stats.AddAttribute(boosterType, count);
        UpdateAllUiInfo();
    }
    public bool ForcePlayer(float force)
    {
        if (!canForce) return false;

        canForce = false;
        playerControl.Jump(force);
        StartCoroutine(ColdDown(cooldownForce, value => canForce = value));

        return true;
    }
    private IEnumerator ColdDown(float cooldown, System.Action<bool> setValue)
    {
        yield return new WaitForSeconds(cooldown);
        setValue(true);
    }
    public GameObject CreateTrap(GameObject trap)
    {
        GameObject trapObj = Instantiate(trap, null);
        trapObj.transform.position = PlayerModel.transform.position;
        return trapObj;
    }
    public void UpdateAllUiInfo()
    {
        UpdateAllStats();
        UpdateHP();
        UpdateMANA();
        UpdateRegenMANA();

        pl_ui.UpdateInfoPlayerStatus();
    }

    ///Player stats and player UI
    public void AddMaxHP(int addMaxHp)
    {
        pl_stats.AddMaxHPBaseStat(addMaxHp);
        pl_ui.UpdateHpBar(pl_stats);
        pl_ui.UpdateSizeHpBar(pl_stats);
    }
    public void AddMaxMana(int addMaxMana)
    {
        pl_stats.AddMaxManaBaseStat(addMaxMana);
        pl_ui.UpdateManaBar(pl_stats);
        pl_ui.UpdateSizeManaBar(pl_stats);
    }
    public void TakeDamage(int damage,  damageT typeAttack, bool canEvade, EffectData effect = null)
    {
        if(canEvade)
        {
            if (pl_stats.isEvasion())
            {
                Debug.Log("Игрок уклонился от удара");
                return;
            }
        }
        switch (typeAttack)
        {
            case damageT.Physical:
                {
                    pl_stats.TakePhysicalDamageStat(damage);
                    break;
                }
            case damageT.Magic:
                {
                    pl_stats.TakeMagicDamageStat(damage);
                    break;
                }
            case damageT.Technical:
                {
                    pl_stats.TakeTechDamageStat(damage);
                    break;
                }
            case damageT.Posion:
                {
                    pl_stats.TakePosionDamageStat(damage);
                    break;
                }
            default: goto case damageT.Physical;
        }
        if (effect != null)
        {
            GetComponent<EffectsManager>().ApplyEffect(effect);
        }
        pl_ui.UpdateHpBar(pl_stats);
        StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
        IsDeath();
    }
    public bool HaveMana(int spendMana) => pl_stats.HaveMana(spendMana);
    public void SpendMana(int spendMana)
    {
        pl_stats.SpendMana(spendMana);
        pl_ui.UpdateManaBar(pl_stats);
    }
    public bool TakeHeal(int heal)
    {
        if (pl_stats.PlayerHealStat(heal))
        {
            SoundsManager.Instance.PlayItemSoundsWithRandomPitch(0.7f, 1.2f, 4);
            pl_ui.UpdateHpBar(pl_stats);
            StartCoroutine(FlashColor(new Color32(110, 255, 93, 255), 0.1f));
            return true;
        }
        return false;
    }
    public bool TakeHealMana(int manaHeal)
    {
        if (pl_stats.PlayerManaHealStat(manaHeal))
        {
            SoundsManager.Instance.PlayItemSounds(4);
            pl_ui.UpdateManaBar(pl_stats);
            StartCoroutine(FlashColor(new Color32(84, 160, 210, 255), 0.1f));
            return true;
        }
        return false;
    }    
    private IEnumerator FlashColor(Color32 color, float time)
    {
        if (player_sprite == null) yield break;

        player_sprite.color = color;
        float elapsed = 0;

        while (elapsed < time)
        {
            if (Time.timeScale > 0)
            {
                elapsed += Time.deltaTime;
            }

            yield return null; // Ждет следующий кадр
        }

        player_sprite.color = new Color32(255, 255, 255, 255);
    }

    public bool IsFullHP()
    {
        if (pl_stats.Cur_Hp == pl_stats.Max_Hp) return true;
        else return false;
    }
    public bool IsFullMana()
    {
        if (pl_stats.Cur_Mana == pl_stats.Max_Mana) return true;
        else return false;
    }
    public int GetHp()
    {
        return GetPlayerStats().Cur_Hp;
    }
    public float GetMana()
    {
        return GetPlayerStats().Cur_Mana;
    }
    public void AddExp(int add_exp)
    {
        pl_stats.AddExpStat(add_exp);
        pl_ui.UpdateExpBar(pl_stats);
    }
    public void LvlUp()
    {
        //pl_ui.LvlUIUpdate(pl_stats);
        //pl_ui.UpdateHpBar(pl_stats);
        //pl_ui.UpdateSizeHpBar(pl_stats);
        //pl_ui.UpdateManaBar(pl_stats);
        //pl_ui.UpdateSizeManaBar(pl_stats);
        pl_ui.UpdateAllInfo(pl_stats);

        SoundsManager.Instance.PlayLevelUP();
        StartCoroutine(ShowLevelUPWithDelay());
    }
    public void TradeAddExp(int add_exp)
    {
        pl_stats.AddTradeExp(add_exp);
    }
    public void TradeLvlUp()
    {
        SoundsManager.Instance.PlayLevelUP();
    }
    private IEnumerator ShowLevelUPWithDelay()
    {
        yield return new WaitForSeconds(1f);
        
        UIControl.Instance.ShowHideLvlUP(true);
        UIControl.Instance.OpenLvlUPWindow(true);
    }
    public void SetGodMode()
    {
        GodMode = true;
    }
    public void SetSurvaveMode()
    {
        GodMode = false;
    }
}
