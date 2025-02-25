using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour 
{
    //UI
    [SerializeField]
    private Transform hp_bar;
    private Image cur_hp_image;
    private TextMeshProUGUI hp_text;
    private RectTransform hpRect;

    [SerializeField]
    private Transform exp_bar;
    private Image cur_exp_image;
    private TextMeshProUGUI exp_text;
    private RectTransform expRect;

    [SerializeField]
    private Transform player_info_panel;
    private TextMeshProUGUI text_player_info;


    public void FindUI(Transform GameUI)
    {
        hp_bar = GameUI.Find("HP_MaxHealth");
    }
    public void SetComponentUI()
    {
        cur_hp_image = hp_bar.GetChild(1).GetComponent<Image>();
        hp_text = hp_bar.GetChild(2).GetComponent<TextMeshProUGUI>();
        hpRect = hp_bar.GetComponent<RectTransform>();
        cur_exp_image = exp_bar.GetChild(1).GetComponent<Image>();
        exp_text = exp_bar.GetChild(2).GetComponent<TextMeshProUGUI>();
        expRect = exp_bar.GetComponent<RectTransform>();
        text_player_info = player_info_panel.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void UpdateHpBar(PlayerStats pl_stats)
    {                
        cur_hp_image.fillAmount = (float)pl_stats.Cur_Hp / pl_stats.Max_Hp;
        hp_text.text = $"{pl_stats.Cur_Hp} / {pl_stats.Max_Hp}";
    }
    public void UpdateExpBar(PlayerStats pl_stats)
    {
        cur_exp_image.fillAmount = (float)pl_stats.cur_exp / pl_stats.nextLvl_exp;
        exp_text.text = $"{pl_stats.cur_exp} / {pl_stats.nextLvl_exp}";
    }
    public void UpdateSizeHpBar(PlayerStats pl_stats)
    {
        if (pl_stats.Max_Hp <= 1000)
        {
            hpRect.sizeDelta = new Vector2(30f + pl_stats.Max_Hp / 3, 20f);
            hpRect.anchoredPosition = new Vector2(25.5f + pl_stats.Max_Hp / 6, -40f);
            return;
        }
    }
    public void UpdateAllInfo(PlayerStats pl_stats)
    {
        UpdateHpBar(pl_stats);
        UpdateSizeHpBar(pl_stats);
        UpdateExpBar(pl_stats);
    }
    public void LvlUpUI(string lvlup_text)
    {
        text_player_info.text = lvlup_text;
    }
}
