using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInfo: MonoBehaviour
{
    public static DisplayInfo Instance { get; private set; }

    [SerializeField] private GameObject InfoStatusObj;
    private DisplayPlayerStats InfoStatus;

    [SerializeField] private GameObject InfoItemsObj;
    private RectTransform sizeInfoItem;
    private DisplayItem InfoItem;

    private PlayerStats pl_stat;
    private EquipStats eqip_stat;

    private Vector2 mousePos;
    //private Vector2 mouseOffset;
    public bool moveInfo;
    //Player
    //Характеристики
    public string word_status_info { get; private set; }
    public string word_Cur_Hp {get; private set;}
    public string word_Max_Hp {get; private set;}
    public string word_Armor_Hp {get; private set;}

    public string word_Mov_Speed {get; private set;}

    public string word_Att_Range {get; private set;}
    public string word_Att_Damage {get; private set;}
    public string word_Att_Speed {get; private set;}
    public string word_Proj_Speed {get; private set;}

    public string word_level {get; private set;}
    public string word_freeSkillPoints {get; private set;}
    public string word_cur_exp {get; private set;}
    public string word_nextLvl_exp {get; private set;}
    public string word_classPlayer {get; private set;}

    public string word_curse_lvl { get; private set; }
    public string word_char_lvl { get; private set; }
    public string word_art_lvl { get; private set; }
    public string word_Damage {get; private set;}
    public string word_Armor {get; private set;}
    public string word_Speed {get; private set;}
    public string word_Evasion {get; private set;}
    public string word_AttSpeed {get; private set;}
    public string word_AttRange {get; private set;}
    public string word_ProjSpeed {get; private set;}
    public string word_eqipment {get; private set;}
    public string word_roleClass {get; private set;}
    public string word_strength {get; private set;}
    public string word_agility {get; private set;}
    public string word_intelligence {get; private set;}
    public string word_expBonus {get; private set;}
    public string word_base {get; private set;}
    public string word_for {get; private set;}
    public string word_TradeSkill {get; private set;}
    public string word_gold {get; private set;}
    public string word_technology_gold {get; private set;}
    public string word_mage_gold {get; private set;}
    public string word_count_proj {get; private set;}

    public string word_Tech_Resis {get; private set;}
    public string word_Magic_Resis {get; private set;}


    //Slots
    public string word_player {get; private set;}
    public string word_typeItem {get; private set;}
    public string word_damageType {get; private set;}
    public string word_damage {get; private set;}
    public string word_attacks_speed {get; private set;}
    public string word_attacks_intervals {get; private set;}
    public string word_range {get; private set;}
    public string word_description {get; private set;}


    private void Awake()
    {
        // Проверка на существование другого экземпляра
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InfoStatus = InfoStatusObj.GetComponent<DisplayPlayerStats>();
        InfoItem = InfoItemsObj.GetComponent<DisplayItem>();

        sizeInfoItem = InfoItemsObj.GetComponent<RectTransform>();
        //mouseOffset = new Vector2(sizeInfoItem.rect.width / 2, 0);
        //sizeInfoItem.pivot = new Vector2(1f, 1.15f); //Центр выше, смещение
    }
    private void Update()
    {
        if(moveInfo)
        {
            UpdateItemInfoPanel();
        }
    }
    public void SetActiveStatusInfo(bool isActive)
    {
        InfoStatusObj.SetActive(isActive);
    }
    public void SetActiveItemInfo(bool isActive)
    {
        InfoItemsObj.SetActive(isActive);
    }
    public void UpdateSizeWindowItem()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(InfoItem.GetComponent<RectTransform>());
        InfoItem.UpdateSizeWindow();
    }
    public void UpdateItemInfoPanel()
    {
        mousePos = Input.mousePosition;
        int newPivotX = mousePos.x > Screen.width * 0.66f ? 1 : 0;
        int newPivotY = mousePos.y > Screen.height / 3 ? 1 : 0;

        if (sizeInfoItem.pivot.x != newPivotX || sizeInfoItem.pivot.y != newPivotY)
        {
            sizeInfoItem.pivot = new Vector2(newPivotX, newPivotY);
        }
        Vector2 offset = new Vector2(20f * (newPivotX == 1 ? -1 : 1),  // Смещение по X влево или вправо
                                     20f * (newPivotY == 1 ? -1 : 1));  // Смещение по Y вверх или вниз

        sizeInfoItem.position = mousePos + offset; ;
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

                word_curse_lvl = localized_player_stats_name["word_curse_lvl"];
                word_char_lvl = localized_player_stats_name["word_char_lvl"];
                word_art_lvl = localized_player_stats_name["word_art_lvl"];
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
                word_Tech_Resis = localized_player_stats_name["word_Tech_Resis"];
                word_Magic_Resis = localized_player_stats_name["word_Magic_Resis"];
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

    public void UpdateInfoStatus()
    {
        StringBuilder info = new StringBuilder();

        pl_stat = Player.Instance.GetPlayerStats();
        eqip_stat = Player.Instance.GetEquipStats();

        string str = $"<color={GlobalColors.Hh_Str}>{pl_stat.Strength} {word_strength}</color>";
        string strB = $"<color={GlobalColors.Hh_Bonus}>" +
            $"HP + {pl_stat.Strength * 2}\n" +
            $"{word_Damage} + {(pl_stat.Strength * 2 / 10).ToString("F2")}\n" +
            $"{word_Armor} + {(pl_stat.Strength / 10).ToString("F2")}";

        string agil = $"<color={GlobalColors.Hh_Agi}>{pl_stat.Agility} {word_agility}</color>";
        string agilB = $"<color={GlobalColors.Hh_Bonus}>" +
            $"{word_Speed} + {(pl_stat.Agility * 0.015f).ToString("F2")}\n" +
            $"{word_Evasion} + {(pl_stat.Agility)}\n" +
            $"{word_AttSpeed} + {(pl_stat.Agility * 2)}";
        string intel = $"<color={GlobalColors.Hh_Int}>{pl_stat.Intelligence} {word_intelligence}</color>";
        string intelB = $"<color={GlobalColors.Hh_Bonus}>" +
            $"{word_AttRange} + {(pl_stat.Intelligence * 0.1f).ToString("F2")}\n" +
            $"{word_ProjSpeed} + {(pl_stat.Intelligence * 0.1f).ToString("F2")}\n" +
            $"{word_Damage} + {(pl_stat.Intelligence * 2 / 10).ToString("F2")}";

        InfoStatus.UpdateAttribute(str, strB, agil, agilB, intel, intelB);

        info.Append($"{word_level}: {pl_stat.level} | {word_expBonus}: {((pl_stat.ExpBust - 1) * 100).ToString("F1")}%\n");
        info.Append($"{word_gold}: {pl_stat.Gold}\n");
        AppendStat(info, word_Max_Hp, pl_stat.Max_Hp, false, false, pl_stat.Base_Max_Hp, pl_stat.Strength * 2, pl_stat.classPlayer.Bonus_Class_Hp, eqip_stat.Bonus_Equip_Hp,
            ($"{word_strength} * 2"), word_roleClass, word_eqipment);
        AppendStat(info, word_Armor_Hp, pl_stat.Armor, false, false, pl_stat.Base_Armor, pl_stat.Strength / 10, pl_stat.classPlayer.Bonus_Class_Armor, eqip_stat.Bonus_Equip_Armor,
            ($"{word_strength} / 10"), word_roleClass, word_eqipment);
        AppendStat(info, word_Mov_Speed, pl_stat.Mov_Speed, false, false, pl_stat.Base_Mov_Speed, pl_stat.Agility * 0.015f, pl_stat.classPlayer.Bonus_Class_SpeedMove, eqip_stat.Bonus_Equip_Mov_Speed,
            ($"{word_agility} * 0.015"), word_roleClass, word_eqipment);
        AppendStat(info, word_Evasion, pl_stat.Evasion, true, true, pl_stat.Base_Evasion, pl_stat.Agility, eqip_stat.Bonus_Equip_Evasion, 0,
            ($"{word_for + word_agility}"), word_roleClass, word_eqipment);
        AppendStat(info, word_Att_Speed, pl_stat.Att_Speed, false, false, pl_stat.Base_Att_Speed, pl_stat.Agility * 2, pl_stat.classPlayer.Bonus_Class_AttackSpeed, eqip_stat.Bonus_Equip_Att_Speed,
            ($"{word_agility}"), word_roleClass, word_eqipment);
        AppendStat(info, word_Att_Range, pl_stat.Att_Range, false, false, pl_stat.Base_Att_Range, pl_stat.Intelligence * 0.1f, pl_stat.classPlayer.Bonus_Class_Range, eqip_stat.Bonus_Equip_Att_Range,
            ($"{word_intelligence} * 0.1"), word_roleClass, word_eqipment);
        AppendStat(info, word_Proj_Speed, pl_stat.Proj_Speed, false, false, pl_stat.Base_Proj_Speed, pl_stat.Intelligence * 0.1f, pl_stat.classPlayer.Bonus_Class_ProjectileSpeed, eqip_stat.Bonus_Equip_Proj_Speed,
            ($"{word_intelligence} * 0.1"), word_roleClass, word_eqipment);
        AppendStat(info, word_Att_Damage, pl_stat.Att_Damage, false, false, pl_stat.Base_Att_Damage, (pl_stat.Strength * 2 + pl_stat.Intelligence * 2) / 10, pl_stat.classPlayer.Bonus_Class_Damage, eqip_stat.Bonus_Equip_Att_Damage,
            ($"{pl_stat.Strength * 2} ({word_strength} * 2) + {(pl_stat.Intelligence * 2)} ({word_intelligence} * 2) / 10"),
            word_roleClass, word_eqipment);
        // Добавление сопротивлений в статистику
        AppendStat(info, word_Tech_Resis, pl_stat.Tech_Resis, true, false, pl_stat.Base_Tech_Resis, pl_stat.Intelligence, pl_stat.classPlayer.Bonus_Tech_Resis, eqip_stat.Bonus_Tech_Resis,
            ($"{word_intelligence} / ({word_intelligence} + 100)"), word_roleClass, word_eqipment);

        AppendStat(info, word_Magic_Resis, pl_stat.Magic_Resis, true, false, pl_stat.Base_Magic_Resis, pl_stat.Intelligence, pl_stat.classPlayer.Bonus_Magic_Resis, eqip_stat.Bonus_Magic_Resis,
            ($"{word_intelligence} / ({word_intelligence} + 100)"), word_roleClass, word_eqipment);

        info.AppendLine($"{word_freeSkillPoints}: {pl_stat.freeSkillPoints}");
        info.AppendLine($"{word_TradeSkill}: {pl_stat.TraderSkill}");
        info.AppendLine($"{word_technology_gold}: {pl_stat.TechniquePoints}");
        info.AppendLine($"{word_mage_gold}: {pl_stat.MagicPoints}");
        info.AppendLine($"{word_count_proj}: {pl_stat.count_Projectile}");


        InfoStatus.UpdateOtherInfo(word_status_info, info.ToString());

    }
    private void AppendStat(StringBuilder info, string statName, float totalStat, bool IsProcent, bool isHundred, float baseStat, float statModifier, float classBonus, float equipBonus, string statNameMofifier, string roleClass, string equipment)
    {
        int sizeFont = 13;

        if (IsProcent && !isHundred)
        {
            totalStat *= 100;
            baseStat *= 100;
            statModifier *= 100;
            classBonus *= 100;
            equipBonus *= 100;
        }

        if (IsProcent)
            info.Append($"{statName}: {totalStat.ToString("F0")}%");
        else
        {
            if (totalStat % 1 == 0)
                info.Append($"{statName}: {totalStat.ToString("F0")}");
            else
                info.Append($"{statName}: {totalStat.ToString("F2")}");
        }


        if (totalStat <= 0)
        {
            info.Append("\n");
            return;
        }
        info.Append($"<size={sizeFont}><color={GlobalColors.Hh_AddInfo}>    = ");
        info.Append($"{baseStat} ({word_base})");
        if (statModifier > 0) info.Append($" + {statModifier} ({statNameMofifier})");
        if (classBonus > 0) info.Append($" + {classBonus} ({word_for + roleClass})");
        if (equipBonus > 0) info.Append($" + {equipBonus} ({word_for + equipment})");
        info.Append($"\n</color></size>");
    }
    public void UpdateInfoItem(int numbSlot, string TypeSlot)
    {
        Slot slot;
        switch (TypeSlot)
        {
            case "Inventory":
                {
                    slot = Inventory.Instance.GetSlot(new SlotRequest { index = numbSlot });
                    break;
                }
            case "Equip":
                {
                    slot = EqupmentPlayer.Instance.GetSlot(new SlotRequest { index = numbSlot });
                    break;
                }
            case "Sell":
                slot = ShopLogic.Instance.GetSlot(new SlotRequest { index = numbSlot, Type = TypeSlot });
                break;
            case "Buy":
                slot = ShopLogic.Instance.GetSlot(new SlotRequest { index = numbSlot, Type = TypeSlot });
                break;
            case "Shop":
                slot = ShopLogic.Instance.GetSlot(new SlotRequest { index = numbSlot, Type = TypeSlot });
                break;
            default:
                return;
        }
        Item item = slot.Item;
        if (item == null || item.Id == 0) return;



        pl_stat = Player.Instance.GetPlayerStats();
        moveInfo = true;


        //Первый текст
        string colorName = "#" + ColorUtility.ToHtmlStringRGBA(item.GetColor());
        string nameItem =  $"<size=14>{item.Name}</size>\n<size=12><color={colorName}>{item.quality}</color></size>";


        //Второй текст
        StringBuilder info = new StringBuilder();
        info.AppendLine($"{word_typeItem}: {item.TypeItem.ToString()}");
        if (item is Weapon weapon)
        {
            info.AppendLine($"{word_damageType}: {weapon.typeDamage}");
            info.AppendLine($"{word_damage}: {weapon.damage} {SetStyleLine(GlobalColors.Hh_AddInfo, 10, $"+ {pl_stat.Att_Damage} {word_Damage} {word_player}")}");

            int finally_AttackSpeed = weapon.addAttackSpeed + pl_stat.Att_Speed;
            info.AppendLine($"{word_attacks_speed}: {weapon.addAttackSpeed} {SetStyleLine(GlobalColors.Hh_AddInfo, 10, $"+ {pl_stat.Att_Speed} {word_AttSpeed} {word_player}")}");
            info.AppendLine($"{word_attacks_speed}: {weapon.attackSpeedCoof} {SetStyleLine(GlobalColors.Hh_AddInfo, 10, $"* {finally_AttackSpeed} {word_AttSpeed} {word_player}")}");

            info.AppendLine($"{word_attacks_intervals}: {(60f / ((finally_AttackSpeed) * weapon.attackSpeedCoof)).ToString("F2")} {SetStyleLine(GlobalColors.Hh_AddInfo, 10, $"= 60 / {word_attacks_speed} * {pl_stat.Att_Speed} {word_AttSpeed} {word_player}")}");
            info.AppendLine($"{word_range}: {weapon.range}  {SetStyleLine(GlobalColors.Hh_AddInfo, 10, $"= {weapon.range} + {pl_stat.Att_Range} {word_AttRange} {word_player}")}");
            //attack_ran + (Player.Instance.GetPlayerStats().Att_Range/2)
        }
        else if(item is ArtifactItem)
        {
            ArtifactObj artifactObj = Artifacts.Instance.GetArtifact(slot.artifact_id);

            FormatLevel(info, artifactObj.art_level, artifactObj.chars_level, artifactObj.curse_level);

            FormatStat(info, artifactObj.Artif_Strength, word_strength, GlobalColors.Hh_Str);
            FormatStat(info, artifactObj.Artif_Agility, word_agility, GlobalColors.Hh_Agi);
            FormatStat(info, artifactObj.Artif_Intelligence, word_intelligence, GlobalColors.Hh_Int);
            FormatStat(info, artifactObj.Artif_Hp, word_Max_Hp, GlobalColors.Hh_Hp);
            FormatStat(info, artifactObj.Artif_Armor, word_Armor, GlobalColors.Hh_Armor);
            FormatStat(info, artifactObj.Artif_Evasion, word_Evasion, GlobalColors.Hh_Evasion);
            FormatStat(info, artifactObj.Artif_Mov_Speed, word_Mov_Speed, GlobalColors.Hh_Mov_Speed);
            FormatStat(info, artifactObj.Artif_Att_Range, word_AttRange, GlobalColors.Hh_Att_Range);
            FormatStat(info, artifactObj.Artif_Att_Speed, word_AttSpeed, GlobalColors.Hh_Att_Speed);
            FormatStat(info, artifactObj.Artif_Proj_Speed, word_ProjSpeed, GlobalColors.Hh_Proj_Speed);
            FormatStat(info, artifactObj.Artif_ExpBust, word_expBonus, GlobalColors.Hh_ExpBust);
            FormatStat(info, artifactObj.Artif_Mage_Resis, word_Magic_Resis, GlobalColors.Hh_Mage_Resis);
            FormatStat(info, artifactObj.Artif_Tech_Resis, word_Tech_Resis, GlobalColors.Hh_Tech_Resis);
            FormatStat(info, artifactObj.Artif_Damage, word_Damage, GlobalColors.Hh_Damage);

        }
        info.AppendLine($"\n{word_description}: {item.Description}");

        InfoItem.LoadInfo(item.Sprite, nameItem, info.ToString());
    }
    private void FormatStat<T>(StringBuilder sb, T value, string word, string color)
    {
        if(value is float f && f != 0)
        {
            sb.AppendLine($"<color={color}>{word}: {(f > 0 ? "+" : "")}{f:F2}</color>");
        }
        else if(value is int i && i != 0)
        {
            sb.AppendLine($"<color={color}>{word}: {(i > 0 ? "+" : "")}{i}</color>");
        }
    }
    private void FormatLevel(StringBuilder sb, int lvlart, int lvlcharms, int curselvl)
    {
        int tempCharm = Mathf.Abs(lvlcharms);
        string tempColor = null;
        if (tempCharm < 1) tempColor = GlobalColors.Common;
        else if (tempCharm < 5) tempColor = GlobalColors.Uncommon;
        else if (tempCharm < 15) tempColor = GlobalColors.Rare;
        else if (tempCharm < 30) tempColor = GlobalColors.Mystical;
        else if (tempCharm < 50) tempColor = GlobalColors.Legendary;
        else tempColor = GlobalColors.Interverse;

        sb.AppendLine($"<color={tempColor}>{word_char_lvl}: <b>{lvlcharms}</b></color>");
        sb.AppendLine($"<color={GlobalColors.Curse}>{word_curse_lvl}: <b>{curselvl}</b></color>\n");
    }
    private string SetStyleLine(string HashColor, int sizeFont, string line)
    {
        return $"<size={sizeFont}><color={HashColor}>{line}</color></size>";
    }

}
