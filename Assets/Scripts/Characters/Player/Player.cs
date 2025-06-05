using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, ITakeDamage
{
    public static Player Instance { get; set; }
    public bool godMode { get; private set; }
    public bool playerStay {  get; set; }
    [SerializeField] private Transform spawnPoint;
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
        equip_Stats = GetComponent<EquipStats>();

        pl_ui.SetComponentUI();

        player_sprite = PlayerModel.GetComponent<SpriteRenderer>();
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
            ResetWeaponToggles();
        }
        if (spawnPoint != null) transform.position = spawnPoint.transform.position;

        pl_ui.UpdateAllInfo(pl_stats);
        ChangeToggleWeapons();
    }
    public void UpdateHP()
    {
        pl_ui.UpdateHpBar(pl_stats);
    }
    public PlayerStats GetPlayerStats() => pl_stats;
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
            default:
                Debug.LogWarning("Неизвестный аспект: " + aspectName);
                break;
        }
        UpdateAllStats();
        UpdateHP();
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
    public GameObject CreateTrap(GameObject trap)
    {
        GameObject trapObj = Instantiate(trap, null);
        trapObj.transform.position = PlayerModel.transform.position;
        return trapObj;
    }


    ///Player stats and player UI
    public void AddMaxHP(int addMaxHp)
    {
        pl_stats.AddMaxHPBaseStat(addMaxHp);
        pl_ui.UpdateHpBar(pl_stats);
        pl_ui.UpdateSizeHpBar(pl_stats);
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
    public bool TakeHeal(int heal)
    {
        if (pl_stats.PlayerHealStat(heal))
        {
            SoundsManager.Instance.PlayItemSounds(4);
            pl_ui.UpdateHpBar(pl_stats);
            StartCoroutine(FlashColor(new Color32(110, 255, 93, 255), 0.1f));
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
    public int GetHp()
    {
        return GetPlayerStats().Cur_Hp;
    }
    public void AddExp(int add_exp)
    {
        pl_stats.AddExpStat(add_exp);
        pl_ui.UpdateExpBar(pl_stats);
    }
    public void LvlUp()
    {
        pl_ui.LvlUIUpdate(pl_stats);
        pl_ui.UpdateHpBar(pl_stats);
        pl_ui.UpdateSizeHpBar(pl_stats);

        SoundsManager.Instance.PlayLevelUP();
        StartCoroutine(ShowLevelUPWithDelay());
    }
    private IEnumerator ShowLevelUPWithDelay()
    {
        yield return new WaitForSeconds(1f);
        
        UIControl.Instance.ShowHideLvlUP(true);
        UIControl.Instance.OpenLvlUPWindow(true);
    }
    public void SetGodMode()
    {
        godMode = true;
    }
    public void SetSurvaveMode()
    {
        godMode = false;
    }
}
