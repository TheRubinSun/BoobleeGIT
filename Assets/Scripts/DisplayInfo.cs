using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInfo: MonoBehaviour
{
    public static DisplayInfo Instance { get; private set; }

    [SerializeField] Transform iconObj;
    private Image iconItem;
    [SerializeField] TextMeshProUGUI nameItem;
    [SerializeField] TextMeshProUGUI infoItem;

    [SerializeField] TextMeshProUGUI Status_Info_Name_Text;
    [SerializeField] TextMeshProUGUI Status_Info;

    [SerializeField] TextMeshProUGUI Strength_Text;
    [SerializeField] TextMeshProUGUI Strength_Bonus_Text;
    [SerializeField] TextMeshProUGUI Agility_Text;
    [SerializeField] TextMeshProUGUI Agility_Bonus_Text;
    [SerializeField] TextMeshProUGUI Intelligence_Text;
    [SerializeField] TextMeshProUGUI Intelligence_Bonus_Text;


    const string HashColorStrength = "#A62E22";
    const string HashColorAgility = "#22A64C";
    const string HashColorIntelligence = "#2273A6";
    const string HashColorBonus = "#E3B23F";
    const string HashColorAddInfo = "#236465";

    //Player
    //Характеристики
    private string word_status_info;
    private string word_Cur_Hp;
    private string word_Max_Hp;
    private string word_Armor_Hp;

    private string word_Mov_Speed;

    private string word_Att_Range;
    private string word_Att_Damage;
    private string word_Att_Speed;
    private string word_Proj_Speed;

    private string word_level;
    private string word_freeSkillPoints;
    private string word_cur_exp;
    private string word_nextLvl_exp;
    private string word_classPlayer;

    private string word_Damage;
    private string word_Armor;
    private string word_Speed;
    private string word_Evasion;
    private string word_AttSpeed;
    private string word_AttRange;
    private string word_ProjSpeed;
    private string word_eqipment;
    private string word_roleClass;
    private string word_strength;
    private string word_agility;
    private string word_intelligence;
    private string word_expBonus;
    private string word_base;
    private string word_for;
    private string word_TradeSkill;
    private string word_gold;
    private string word_technology_gold;
    private string word_mage_gold;
    private string word_count_proj;


    //Slots
    private string word_player;
    private string word_typeItem;
    private string word_damageType;
    private string word_damage;
    private string word_attacks_speed;
    private string word_attacks_intervals;
    private string word_range;
    private string word_description;


    private void Awake()
    {
        // Проверка на существование другого экземпляра
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        iconItem = iconObj.GetComponent<Image>();
    }
    public void LocalizationText()
    {
        if (LocalizationManager.Instance != null)
        {
            Dictionary<string, string> localized_player_stats_name = LocalizationManager.Instance.GetLocalizedValue("ui_text", "player_stats_name");
            Dictionary<string, string> localized_slots_name = LocalizationManager.Instance.GetLocalizedValue("ui_text", "slots_name");
            if (localized_player_stats_name != null)
            {
                word_status_info = localized_player_stats_name["word_status_info"];
                word_Cur_Hp = localized_player_stats_name["word_Cur_Hp"];
                word_Max_Hp = localized_player_stats_name["word_Max_Hp"];
                word_Armor_Hp = localized_player_stats_name["word_Armor_Hp"];

                word_Mov_Speed = localized_player_stats_name["word_Mov_Speed"];

                word_Att_Range = localized_player_stats_name["word_Att_Range"];
                word_Att_Damage = localized_player_stats_name["word_Att_Damage"];
                word_Att_Speed = localized_player_stats_name["word_Att_Speed"];
                word_Proj_Speed = localized_player_stats_name["word_Proj_Speed"];

                word_level = localized_player_stats_name["word_level"];
                word_freeSkillPoints = localized_player_stats_name["word_freeSkillPoints"];
                word_cur_exp = localized_player_stats_name["word_cur_exp"];
                word_nextLvl_exp = localized_player_stats_name["word_nextLvl_exp"];
                word_classPlayer = localized_player_stats_name["word_classPlayer"];

                word_Damage = localized_player_stats_name["word_Damage"];
                word_Armor = localized_player_stats_name["word_Armor"];
                word_Speed = localized_player_stats_name["word_Speed"];
                word_Evasion = localized_player_stats_name["word_Evasion"];
                word_AttSpeed = localized_player_stats_name["word_AttSpeed"];
                word_AttRange = localized_player_stats_name["word_AttRange"];
                word_ProjSpeed = localized_player_stats_name["word_ProjSpeed"];
                word_eqipment = localized_player_stats_name["word_eqipment"];
                word_roleClass = localized_player_stats_name["word_roleClass"];
                word_strength = localized_player_stats_name["word_strength"];
                word_agility = localized_player_stats_name["word_agility"];
                word_intelligence = localized_player_stats_name["word_intelligence"];
                word_expBonus = localized_player_stats_name["word_expBonus"];
                word_base = localized_player_stats_name["word_base"];
                word_for = localized_player_stats_name["word_for"];
                word_Evasion = localized_player_stats_name["word_Evasion"];
                word_TradeSkill = localized_player_stats_name["word_TradeSkill"];
                word_gold = localized_player_stats_name["word_gold"];
                word_technology_gold = localized_player_stats_name["word_technology_gold"];
                word_mage_gold = localized_player_stats_name["word_mage_gold"];
                word_count_proj = localized_player_stats_name["word_count_proj"];
            }
            else Debug.LogWarning($"Локализация для ключа \"localized_player_stats_name\"  не найдена.");
            if(localized_slots_name != null)
            {
                word_player = localized_slots_name["word_player"];
                word_typeItem = localized_slots_name["word_typeItem"];
                word_damageType = localized_slots_name["word_damageType"];
                word_damage = localized_slots_name["word_damage"];
                word_attacks_speed = localized_slots_name["word_attacks_speed"];
                word_attacks_intervals = localized_slots_name["word_attacks_intervals"];
                word_range = localized_slots_name["word_range"];
                word_description = localized_slots_name["word_description"];

            }
            else Debug.LogWarning($"Локализация для ключа \"localized_slots_name\"  не найдена.");
        }
        else
        {
            Debug.LogWarning("LocalizationManager нет на сцене.");
        }
    }


    public void SetActive(bool turn)
    {
        gameObject.SetActive(turn);
    }
    public void UpdateInfoStatus()
    {
        StringBuilder info = new StringBuilder();
        Debug.Log("Open");
        PlayerStats pl_stat = Player.Instance.GetPlayerStats();
        EquipStats eqip_stat = Player.Instance.GetEquipStats();



        Strength_Text.text = $"<color={HashColorStrength}>{pl_stat.Strength} {word_strength}</color>";
        Strength_Bonus_Text.text = $"<color={HashColorBonus}>" +
            $"HP + {pl_stat.Strength*2}\n" +
            $"{word_Damage} + {(pl_stat.Strength * 2/10).ToString("F2")}\n" +
            $"{word_Armor} + {(pl_stat.Strength/ 10).ToString("F2")}";
        Agility_Text.text = $"<color={HashColorAgility}>{pl_stat.Agility} {word_agility}</color>";
        Agility_Bonus_Text.text = $"<color={HashColorBonus}>" +
            $"{word_Speed} + {(pl_stat.Agility * 0.015f).ToString("F2")}\n" +
            $"{word_Evasion} + {(pl_stat.Agility)}\n" +
            $"{word_AttSpeed} + {(pl_stat.Agility * 2)}";
        Intelligence_Text.text = $"<color={HashColorIntelligence}>{pl_stat.Intelligence} {word_intelligence}</color>";
        Intelligence_Bonus_Text.text = $"<color={HashColorBonus}>" +
            $"{word_AttRange} + {(pl_stat.Intelligence * 0.01f).ToString("F2")}\n" +
            $"{word_ProjSpeed} + {(pl_stat.Intelligence * 0.01f).ToString("F2")}\n" +
            $"{word_Damage} + {(pl_stat.Intelligence * 2 / 10).ToString("F2")}";

        
        info.Append($"{word_level}: {pl_stat.level} | {word_expBonus}: {pl_stat.ExpBust}\n");
        info.Append($"{word_gold}: {pl_stat.Gold}\n");
        AppendStat(info, word_Max_Hp, pl_stat.Max_Hp, pl_stat.Base_Max_Hp, pl_stat.Strength * 2, pl_stat.classPlayer.Bonus_Class_Hp, eqip_stat.Bonus_Equip_Hp,
            ($"{word_strength} * 2"), word_roleClass, word_eqipment);
        AppendStat(info, word_Armor_Hp, pl_stat.Armor, pl_stat.Base_Armor, pl_stat.Strength / 10, pl_stat.classPlayer.Bonus_Class_Armor, eqip_stat.Bonus_Equip_Armor,
            ($"{word_strength} / 10"), word_roleClass, word_eqipment);
        AppendStat(info, word_Mov_Speed, pl_stat.Mov_Speed, pl_stat.Base_Mov_Speed, pl_stat.Agility * 0.015f, pl_stat.classPlayer.Bonus_Class_SpeedMove, eqip_stat.Bonus_Equip_Mov_Speed,
            ($"{word_agility} * 0.015"), word_roleClass, word_eqipment);
        AppendStat(info, word_Evasion, pl_stat.Evasion, pl_stat.Base_Evasion, pl_stat.Agility, eqip_stat.Bonus_Equip_Evasion, 0, 
            ($"{word_for + word_agility}"), word_roleClass, word_eqipment);
        AppendStat(info, word_Att_Speed, pl_stat.Att_Speed, pl_stat.Base_Att_Speed, pl_stat.Agility * 2, pl_stat.classPlayer.Bonus_Class_AttackSpeed, eqip_stat.Bonus_Equip_Att_Speed,
            ($"{word_agility} * 2"), word_roleClass, word_eqipment);
        AppendStat(info, word_Att_Range, pl_stat.Att_Range, pl_stat.Base_Att_Range, pl_stat.Intelligence * 0.01f, pl_stat.classPlayer.Bonus_Class_Range, eqip_stat.Bonus_Equip_Att_Range,
            ($"{word_intelligence} * 0.01"), word_roleClass, word_eqipment);
        AppendStat(info, word_Proj_Speed, pl_stat.Proj_Speed, pl_stat.Base_Proj_Speed, pl_stat.Intelligence * 0.01f, pl_stat.classPlayer.Bonus_Class_ProjectileSpeed, eqip_stat.Bonus_Equip_Proj_Speed, 
            ($"{word_intelligence} * 0.01"), word_roleClass, word_eqipment);
        AppendStat(info, word_Att_Damage, pl_stat.Att_Damage, pl_stat.Base_Att_Damage, (pl_stat.Strength * 2 + pl_stat.Intelligence * 2) / 10, pl_stat.classPlayer.Bonus_Class_Damage, eqip_stat.Bonus_Equip_Att_Damage,
            ($"{pl_stat.Strength * 2} ({word_strength} * 2) + {pl_stat.Intelligence * 2} ({word_intelligence} * 2) / 10"), word_roleClass, word_eqipment);

        info.AppendLine($"{word_freeSkillPoints}: {pl_stat.freeSkillPoints}");
        info.AppendLine($"{word_TradeSkill}: {pl_stat.TraderSkill}");
        info.AppendLine($"{word_technology_gold}: {pl_stat.TechniquePoints}");
        info.AppendLine($"{word_mage_gold}: {pl_stat.MagicPoints}");
        info.AppendLine($"{word_count_proj}: {pl_stat.count_Projectile}");

        Status_Info_Name_Text.text = word_status_info;
        Status_Info.text = info.ToString();
    }
    private void AppendStat(StringBuilder info, string statName, float totalStat, float baseStat, float statModifier, float classBonus, float equipBonus, string statNameMofifier, string roleClass, string equipment)
    {
        int sizeFont = 10;
        info.Append($"{statName}: {totalStat}");
        if (totalStat <= 0)
        {
            info.Append("\n");
            return;
        }    
        info.Append($"<size={sizeFont}><color={HashColorAddInfo}>    = ");
        info.Append($"{baseStat} ({word_base})");
        if (statModifier > 0) info.Append($" + {statModifier} ({statNameMofifier})");
        if (classBonus > 0) info.Append($" + {classBonus} ({word_for + roleClass})");
        if (equipBonus > 0) info.Append($" + {equipBonus} ({word_for + equipment})");
        info.Append($"\n</color></size>");
    }

    public void UpdateInfoItem(int numbSlot, string TypeSlot)
    {
        Item item;
        switch (TypeSlot)
        {
            case "Inventory":
                item = Inventory.Instance.GetSlot(numbSlot).Item;
                break;
            case "Equip":
                item = EqupmentPlayer.Instance.GetSlot(numbSlot).Item;
                break;
            case "Sell":
                item = ShopLogic.Instance.GetSlot(TypeSlot, numbSlot).Item;
                break;
            case "Buy":
                item = ShopLogic.Instance.GetSlot(TypeSlot, numbSlot).Item;
                break;
            case "Shop":
                item = ShopLogic.Instance.GetSlot(TypeSlot, numbSlot).Item;
                break;
            default:
                return;
        }
        
        iconItem.sprite = item.Sprite;

        //Разметка и цвет - первый текст
        string colorName = "#" + ColorUtility.ToHtmlStringRGBA(item.GetColor());
        nameItem.text =  $"<size=14>{item.Name}</size>\n<size=12><color={colorName}>{item.quality}</color></size>";

        PlayerStats pl_st = Player.Instance.GetPlayerStats();
        //Второй текст
        StringBuilder info = new StringBuilder();
        info.AppendLine($"{word_typeItem}: {item.TypeItem.ToString()}");
        if (item is Weapon weapon)
        {
            info.AppendLine($"{word_damageType}: {weapon.typeDamage}");
            info.AppendLine($"{word_damage}: {weapon.damage} {SetStyleLine(HashColorAddInfo, 10, $"+ {pl_st.Att_Damage} {word_Damage} {word_player}")}");
            info.AppendLine($"{word_attacks_speed}: {weapon.attackSpeed} {SetStyleLine(HashColorAddInfo, 10, $"* {pl_st.Att_Speed} {word_AttSpeed} {word_player}")}");
            info.AppendLine($"{word_attacks_intervals}: {(60f / (weapon.attackSpeed * pl_st.Att_Speed)).ToString("F2")} {SetStyleLine(HashColorAddInfo, 10, $"= 60 / {word_attacks_speed} * {pl_st.Att_Speed} {word_AttSpeed} {word_player}")}");
            info.AppendLine($"{word_range}: {weapon.range}  {SetStyleLine(HashColorAddInfo, 10, $"= {weapon.range/pl_st.Att_Range} * {pl_st.Att_Range} {word_AttRange} {word_player}")}");
            //attack_ran + (Player.Instance.GetPlayerStats().Att_Range/2)
        }
        info.AppendLine($"{word_description}: {item.Description}");
        infoItem.text = info.ToString();
    }
    private string SetStyleLine(string HashColor, int sizeFont, string line)
    {
        return $"<size={sizeFont}><color={HashColor}>{line}</color></size>";
    }
    public void ClearInfo()
    {
        iconItem.sprite = null;
        nameItem.text = "";
        infoItem.text = "";
    }
}
