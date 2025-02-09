using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    //Характеристики
    public int Cur_Hp;
    public int Max_Hp;
    public int Armor_Hp;

    public int Mov_Speed;

    public float Att_Range;
    public int Att_Damage;
    public int Att_Spaeed;
    public int Proj_Speed;

    //Компоненты игрока
    private RoleClass classPlayer;
    [SerializeField] GameObject PlayerModel;

    //UI
    [SerializeField] Transform hp_bar;
    Image cur_hp_image;
    TextMeshProUGUI hp_text;
    RectTransform hpRect;
    SpriteRenderer player_sprite;
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
        cur_hp_image = hp_bar.GetChild(1).GetComponent<Image>();
        hp_text = hp_bar.GetChild(2).GetComponent<TextMeshProUGUI>();
        hpRect = hp_bar.GetComponent<RectTransform>();
        player_sprite = PlayerModel.GetComponent<SpriteRenderer>();

        RoleClass rc = Classes.Instance.GetRoleClass("Shooter");
        Mov_Speed = rc.BonusSpeedMove;
        Max_Hp = rc.BonusHp;



        Cur_Hp = Max_Hp;
        UpdateHpBar();
        UpdateSizeHpBar();
    }
    public void UpdateHpBar()
    {
        cur_hp_image.fillAmount = (float)Cur_Hp / Max_Hp;
        hp_text.text = $"{Cur_Hp} / {Max_Hp}";
    }
    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashColor(new Color32(255, 108, 108, 255), 0.1f));
        Cur_Hp -= (int)(Mathf.Max(damage / (1 + Armor_Hp / 10f), 1));
        UpdateHpBar();
        IsDeath();
    }
    private IEnumerator FlashColor(Color32 color, float time) //Менять цвет на время
    {
        if(player_sprite != null)
        {
            player_sprite.color = color;
            yield return new WaitForSeconds(time);
            player_sprite.color = new Color32(255, 255, 255, 255);
        }
    }
    public void AddMaxHP(int addMaxHp)
    {
        Max_Hp += addMaxHp;
        UpdateSizeHpBar();
    }
    public void UpdateSizeHpBar()
    {
        if (Max_Hp < 1000)
        {
            hpRect.sizeDelta = new Vector2(30f + Max_Hp / 3, 20f);
            hpRect.anchoredPosition = new Vector2(30f + Max_Hp / 6, -20f);
        }
    }
    private void IsDeath()
    {
        if (Cur_Hp < 1)
        {
            Cur_Hp = 0;
            PlayerModel.SetActive(false);
        }
    }

}
