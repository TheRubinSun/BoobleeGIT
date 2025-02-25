using System.Collections.Generic;
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


    //Player
    //Характеристики
    private string words_status_info;
    private string words_Cur_Hp;
    private string words_Max_Hp;
    private string words_Armor_Hp;

    private string words_Mov_Speed;

    private string words_Att_Range;
    private string words_Att_Damage;
    private string words_Att_Speed;
    private string words_Proj_Speed;

    private string words_level;
    private string words_freeSkillPoints;
    private string words_cur_exp;
    private string words_nextLvl_exp;
    private string words_classPlayer;

    //Slots
    private string words_typeItem;
    private string words_damageType;
    private string words_damage;
    private string words_attacks_speed;
    private string words_attacks_intervals;
    private string words_range;
    private string words_description;
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
                words_status_info = localized_player_stats_name["words_status_info"];
                words_Cur_Hp = localized_player_stats_name["words_Cur_Hp"];
                words_Max_Hp = localized_player_stats_name["words_Max_Hp"];
                words_Armor_Hp = localized_player_stats_name["words_Armor_Hp"];

                words_Mov_Speed = localized_player_stats_name["words_Mov_Speed"];

                words_Att_Range = localized_player_stats_name["words_Att_Range"];
                words_Att_Damage = localized_player_stats_name["words_Att_Damage"];
                words_Att_Speed = localized_player_stats_name["words_Att_Speed"];
                words_Proj_Speed = localized_player_stats_name["words_Proj_Speed"];

                words_level = localized_player_stats_name["words_level"];
                words_freeSkillPoints = localized_player_stats_name["words_freeSkillPoints"];
                words_cur_exp = localized_player_stats_name["words_cur_exp"];
                words_nextLvl_exp = localized_player_stats_name["words_nextLvl_exp"];
                words_classPlayer = localized_player_stats_name["words_classPlayer"];


            }
            else Debug.LogWarning($"Локализация для ключа \"localized_player_stats_name\"  не найдена.");
            if(localized_slots_name != null)
            {
                words_typeItem = localized_slots_name["words_typeItem"];
                words_damageType = localized_slots_name["words_damageType"];
                words_damage = localized_slots_name["words_damage"];
                words_attacks_speed = localized_slots_name["words_attacks_speed"];
                words_attacks_intervals = localized_slots_name["words_attacks_intervals"];
                words_range = localized_slots_name["words_range"];
                words_description = localized_slots_name["words_description"];
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
        string info =  $"{words_level}: {Player.Instance.GetPlayerStats().level}\n";
        info += $"{words_Max_Hp}:  {Player.Instance.GetPlayerStats().Cur_Hp}/{Player.Instance.GetPlayerStats().Max_Hp}\n";
        info += $"{words_Armor_Hp}: {Player.Instance.GetPlayerStats().Armor_Hp}\n";
        info += $"{words_Mov_Speed}: {Player.Instance.GetPlayerStats().Mov_Speed}\n";
        info += $"{words_Att_Range}: {Player.Instance.GetPlayerStats().Att_Range}\n";
        info += $"{words_Att_Damage}: {Player.Instance.GetPlayerStats().Att_Damage}\n";
        info += $"{words_Att_Speed}: {Player.Instance.GetPlayerStats().Att_Speed}\n";
        info += $"{words_Proj_Speed}: {Player.Instance.GetPlayerStats().Proj_Speed}\n";
        info += $"{words_freeSkillPoints}: {Player.Instance.GetPlayerStats().freeSkillPoints}\n";

        Status_Info_Name_Text.text = words_status_info;
        Status_Info.text = info;

    }
    public void UpdateInfoItem(int numbSlot, string TypeSlot)
    {
        Item item;
        if (TypeSlot == "Inventory")
        {
            item = Inventory.Instance.GetSlot(numbSlot).Item;
        }
        else if(TypeSlot == "Equip")
        {
            item = EqupmentPlayer.Instance.GetSlot(numbSlot).Item;
        }
        else
        {
            item = null;
        }
        if (item == null) return;
        iconItem.sprite = item.Sprite;
        

        //Разметка и цвет - первый текст
        string colorName = "#" + ColorUtility.ToHtmlStringRGBA(item.GetColor());
        nameItem.text =  $"<size=8>{item.Name}</size>\n<size=7><color={colorName}>{item.quality}</color></size>";

        //Второй текст
        string info = $"{words_typeItem}: {item.TypeItem.ToString()}\n";
        if (item is Weapon weapon)
        {
            info += $"{words_damageType}: {weapon.typeDamage}\n";
            info += $"{words_damage}: {weapon.damage}\n";
            info += $"{words_attacks_speed}: {weapon.attackSpeed}\n";
            info += $"{words_attacks_intervals}: {(60f / (weapon.attackSpeed * Player.Instance.GetPlayerStats().Att_Speed)).ToString("F2")}\n";
            info += $"{words_range}: {weapon.range}\n";
        }

        info += $"{words_description}: {item.Description}\n";
        infoItem.text = info;
    }
    public void ClearInfo()
    {
        iconItem.sprite = null;
        nameItem.text = "";
        infoItem.text = "";
    }
}
