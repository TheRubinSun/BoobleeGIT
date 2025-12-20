using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AspectName
{
    Agillity,
    Strength,
    Intelligence,
    Tech_Point,
    Mage_Point,
    Exp_Bust,
    Speed,
    Hp,
    Gold,
    AttackSpeed,
    Range,
    Projectile_speed,
    Damage,
    Mana,
    ManaRegen
}
public class LvlUpLogic : MonoBehaviour
{
    public static LvlUpLogic Instance { get; set; }
    [SerializeField] private GameObject pref_Aspect;
    [SerializeField] private Transform parent_aspects;
    [SerializeField] private List<AspectData> aspectDatas = new List<AspectData>();
    private List<TempAspect> tempAspects = new List<TempAspect>();

    [SerializeField] private TextMeshProUGUI nameWindow;

    private string word_nameWindow;
    private string word_acceptButton;
    private string word_Agillity;
    private string word_Strength;
    private string word_Intelligence;
    private string word_Tech_Point;
    private string word_Mage_Point;
    private string word_Exp_Bust;
    private string word_Speed;
    private string word_Hp;
    private string word_Gold;
    private string word_AttackSpeed;
    private string word_Range;
    private string word_Projectile_speed;
    private string word_Damage;
    private string word_Mana;
    private string word_Mana_Regen;


    private static readonly HashSet<AspectName> percentageAspects = new HashSet<AspectName>
    { AspectName.Exp_Bust}; //те что в процентах
    private static System.Random random;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void LocalizationText()
    {
        if(GlobalData.LocalizationManager != null)
        {
            Dictionary<string, string> localized_player_stats_name = GlobalData.LocalizationManager.GetLocalizedValue("ui_text", "aspects_name");
            if(localized_player_stats_name != null)
            {
                word_nameWindow = localized_player_stats_name["word_nameWindow"];
                word_acceptButton = localized_player_stats_name["word_acceptButton"];
                word_Agillity = localized_player_stats_name["word_Agillity"];
                word_Strength = localized_player_stats_name["word_Strength"];
                word_Intelligence = localized_player_stats_name["word_Intelligence"];
                word_Tech_Point = localized_player_stats_name["word_Tech_Point"];
                word_Mage_Point = localized_player_stats_name["word_Mage_Point"];
                word_Exp_Bust = localized_player_stats_name["word_Exp_Bust"];
                word_Speed = localized_player_stats_name["word_Speed"];
                word_Hp = localized_player_stats_name["word_Hp"];
                word_Gold = localized_player_stats_name["word_Gold"];
                word_AttackSpeed = localized_player_stats_name["word_AttackSpeed"];
                word_Range = localized_player_stats_name["word_Range"];
                word_Projectile_speed = localized_player_stats_name["word_Projectile_speed"];
                word_Damage = localized_player_stats_name["word_Damage"];
                word_Mana = localized_player_stats_name["word_Mana"];
                word_Mana_Regen = localized_player_stats_name["word_Mana_Regen"];
                nameWindow.text = word_nameWindow;
            }

        }

    }

    public void RemoveObj()
    {
        foreach(TempAspect tempAsp in tempAspects)
        {
            Destroy(tempAsp.objectAspect);
        }
        tempAspects.Clear();
    }
    public void GenAspects()
    {
        
        //Debug.Log(GlobalData.cur_seed + " " + GlobalData.cur_seed + (Player.GetLevel() - Player.GetFreeSkillPoint()));
        random = new System.Random(GlobalData.cur_seed + (GlobalData.Player.GetLevel() - GlobalData.Player.GetFreeSkillPoint()));
        if (tempAspects.Count > 0)
        {
            RemoveObj();
        }

        List<AspectName> aspects = GetRandomAspects();
        int id = 0;
        foreach (AspectName aspect in aspects)
        {
            
            foreach(AspectData aspectData in aspectDatas)
            {
                if(aspectData.name_asp == aspect)
                {
                    float value;
                    if (aspectData.min_value % 1 == 0 && aspectData.max_value % 1 == 0)
                        value = random.Next((int)aspectData.min_value, (int)aspectData.max_value + 1);
                    else value = Mathf.Round((float)(random.NextDouble() * (aspectData.max_value - aspectData.min_value) + aspectData.min_value) * 100) / 100f;
                    

                    GameObject AspectObg = Instantiate(pref_Aspect, parent_aspects);
                    AspectObg.name = id.ToString();
                    AspectObg.GetComponent<Image>().color = aspectData.BG_Color;

                    AspectObg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        percentageAspects.Contains(aspect)
                        ? $"+{value * 100}% {TranslateWord(aspect)}"
                        : $"+{value} {TranslateWord(aspect)}";

                    AspectObg.transform.GetChild(1).GetComponent<Image>().color = aspectData.BG_Color;
                    AspectObg.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = word_acceptButton;
                    Button btnAccept = AspectObg.transform.GetChild(1).GetComponent<Button>();
                    int currentId = id;
                    btnAccept.onClick.AddListener(() => AcceptAspect(currentId));

                    AspectObg.transform.GetChild(2).GetComponent<Image>().color = aspectData.icon_color;
                    AspectObg.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = aspectData.sprite;
                    tempAspects.Add(new TempAspect(aspect, value, AspectObg));
                    id++;
                    
                    break;
                }
            }


            //Debug.Log(aspect);
        }
    }
    public void AcceptAspect(int id)
    {
        GlobalData.Player.SetAspect(tempAspects[id].aspectName, tempAspects[id].value);
        RemoveObj();

        if(GlobalData.Player.GetFreeSkillPoint() > 0)
        {
            GlobalData.UIControl.OpenLvlUPWindow(false);
            StartCoroutine(StartNewAspectChoose());
        }
        else
        {
            GlobalData.UIControl.CloseWindowLvlUP();
            GlobalData.UIControl.ShowHideLvlUP(false);
        }
        GlobalData.SoundsManager.PlayAcceptAspect();

        GlobalData.DisplayInfo.UpdateInfoStatus();
        GlobalData.EqupmentPlayer.UpdateAllWeaponsStats();
    }
    private string TranslateWord(AspectName aspect)
    {
        switch (aspect)
        {
            case AspectName.Agillity:
                return word_Agillity;
            case AspectName.Strength:
                return word_Strength;
            case AspectName.Intelligence:
                return word_Intelligence;
            case AspectName.Tech_Point:
                return word_Tech_Point;
            case AspectName.Mage_Point:
                return word_Mage_Point;
            case AspectName.Exp_Bust:
                return word_Exp_Bust;
            case AspectName.Speed:
                return word_Speed;
            case AspectName.Hp:
                return word_Hp;
            case AspectName.Gold:
                return word_Gold;
            case AspectName.AttackSpeed:
                return word_AttackSpeed;
            case AspectName.Range:
                return word_Range;
            case AspectName.Projectile_speed:
                return word_Projectile_speed;
            case AspectName.Damage:
                return word_Damage;
            case AspectName.Mana:
                return word_Mana;
            case AspectName.ManaRegen:
                return word_Mana_Regen;
            default:
                return "Unknown Aspect"; // На случай, если что-то пойдет не так
        }
    }
    private IEnumerator StartNewAspectChoose()
    {
        yield return new WaitForSeconds(0.6f);

        GlobalData.UIControl.ShowHideLvlUP(true);
        GenAspects();
        GlobalData.UIControl.OpenLvlUPWindow(true);
    }
    public static List<AspectName> GetRandomAspects()
    {
        List<AspectName> coreAttributes = new List<AspectName>
        {
            AspectName.Agillity, AspectName.Strength, AspectName.Intelligence,
        };

        List<AspectName> allAspects = new List<AspectName>
        {
            AspectName.Agillity, AspectName.Strength, AspectName.Intelligence,
            AspectName.Tech_Point, AspectName.Mage_Point, AspectName.Exp_Bust,
            AspectName.Speed, AspectName.Hp,AspectName.Range, AspectName.Projectile_speed,
            AspectName.Gold, AspectName.AttackSpeed, AspectName.Damage, AspectName.Mana, AspectName.ManaRegen
        };
        //Выбираем случайный гарантированный атрибут 
        AspectName guaranteedAttribute = coreAttributes[random.Next(0, coreAttributes.Count)];


        //Удаляем его из общего списка, чтобы избежать дублирования
        allAspects.Remove(guaranteedAttribute);

        //Перемешиваем
        for (int i = allAspects.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (allAspects[i], allAspects[j]) = (allAspects[j], allAspects[i]);
        }
        List<AspectName> choosenAspects = new List<AspectName>() { guaranteedAttribute };
        choosenAspects.Add(allAspects[0]);
        choosenAspects.Add(allAspects[1]);
        choosenAspects.Add(allAspects[2]);

        return choosenAspects;
    }
}

public class TempAspect
{
    public AspectName aspectName;
    public float value;
    public GameObject objectAspect;

    public TempAspect(AspectName aspectName, float value, GameObject objectAspect)
    {
        this.aspectName = aspectName;
        this.value = value;
        this.objectAspect = objectAspect;
    }
}



