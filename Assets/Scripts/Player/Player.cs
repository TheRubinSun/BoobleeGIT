using System.Collections;
using System.Collections.Generic;
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
    public int Att_Speed;
    public int Proj_Speed;

    public int level;
    private int freeSkillPoints;
    private int cur_exp;
    private int nextLvl_exp;


    private Dictionary<int, WeaponControl> WeaponsObj = new Dictionary<int, WeaponControl>();
    private bool[] DirectionOrVectorWeapon = new bool[4];
    [SerializeField] public Toggle[] TooglesWeapon = new Toggle[4];
    //Компоненты игрока
    private RoleClass classPlayer;
    [SerializeField] 
    private GameObject PlayerModel;
    
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
        nextLvl_exp = 10;
        level = 0;

        //LoadSave Потом
        for (int i = 0; i < DirectionOrVectorWeapon.Length; i++)
        {
            DirectionOrVectorWeapon[i] = false;
            TooglesWeapon[i].isOn = DirectionOrVectorWeapon[i];
        }
        ChangeToggleWeapon();

        cur_hp_image = hp_bar.GetChild(1).GetComponent<Image>();
        hp_text = hp_bar.GetChild(2).GetComponent<TextMeshProUGUI>();
        hpRect = hp_bar.GetComponent<RectTransform>();

        cur_exp_image = exp_bar.GetChild(1).GetComponent<Image>();
        exp_text = exp_bar.GetChild(2).GetComponent<TextMeshProUGUI>();
        expRect = exp_bar.GetComponent<RectTransform>();

        text_player_info = player_info_panel.GetChild(0).GetComponent<TextMeshProUGUI>();

        player_sprite = PlayerModel.GetComponent<SpriteRenderer>();


        RoleClass rc = Classes.Instance.GetRoleClass("Shooter");
        Mov_Speed = rc.BonusSpeedMove;
        Max_Hp = rc.BonusHp;

        Att_Speed = rc.BonusAttackSpeed;


        Cur_Hp = Max_Hp;
        UpdateHpBar();
        UpdateSizeHpBar();

        UpdateExpBar();
    }
    public Dictionary<int, WeaponControl> GetDictWeaponAndArms()
    {
        return WeaponsObj;
    }
    public void SetWeaponsObj(int i, WeaponControl weaponObj)
    {
        WeaponsObj[i] = weaponObj;
    }
    public void UpdateHpBar()
    {
        cur_hp_image.fillAmount = (float)Cur_Hp / Max_Hp;
        hp_text.text = $"{Cur_Hp} / {Max_Hp}";
    }
    public void UpdateExpBar()
    {
        cur_exp_image.fillAmount = (float)cur_exp / nextLvl_exp;
        exp_text.text = $"{cur_exp} / {nextLvl_exp}";
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
        Cur_Hp += addMaxHp;
        UpdateHpBar();
        UpdateSizeHpBar();
    }
    public void UpdateSizeHpBar()
    {
        if (Max_Hp <= 1000)
        {
            hpRect.sizeDelta = new Vector2(30f + Max_Hp / 3, 20f);
            hpRect.anchoredPosition = new Vector2(25.5f + Max_Hp / 6, -40f);
            return;
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
    public void AddExp(int add_exp)
    {
        cur_exp += add_exp;
        if (isNewLevel())
            LvlUp();

        UpdateExpBar();
    }
    private bool isNewLevel()
    {
        if(cur_exp >= nextLvl_exp)
        {
            cur_exp -= nextLvl_exp;
            nextLvl_exp = (int)((nextLvl_exp + 10) * 1.4f);
            return true;
        }
        return false;
    }
    private void LvlUp()
    {
        level++;
        freeSkillPoints++;
        text_player_info.text = $"{level} lvl";
        AddMaxHP(2);
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
}
