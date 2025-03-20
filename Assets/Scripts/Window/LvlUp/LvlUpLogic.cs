using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LvlUpLogic : MonoBehaviour
{
    public static LvlUpLogic Instance { get; set; }
    [SerializeField] private GameObject pref_Aspect;
    [SerializeField] private Transform parent_aspects;
    [SerializeField] private List<AspectData> aspectDatas = new List<AspectData>();
    private List<TempAspect> tempAspects = new List<TempAspect>();


    private static readonly HashSet<AspectName> percentageAspects = new HashSet<AspectName>
    { AspectName.Range, AspectName.Speed, AspectName.Projectile_speed, AspectName.Exp_Bust}; //те что в процентах
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
        Debug.Log(GlobalData.cur_seed + " " + GlobalData.cur_seed + (Player.Instance.GetLevel() - Player.Instance.GetFreeSkillPoint()));
        random = new System.Random(GlobalData.cur_seed + (Player.Instance.GetLevel() - Player.Instance.GetFreeSkillPoint()));
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
                        ? $"+{value * 100}% {aspectData.name_asp}"
                        : $"+{value} {aspectData.name_asp}";
                    AspectObg.transform.GetChild(1).GetComponent<Image>().color = aspectData.BG_Color;

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
        Player.Instance.SetAspect(tempAspects[id].aspectName, tempAspects[id].value);
        RemoveObj();

        if(Player.Instance.GetFreeSkillPoint() > 0)
        {
            UIControl.Instance.ShowHideLvlUP(true);
            GenAspects();
        }
        else
        {
            UIControl.Instance.CloseWindowLvlUP();
            UIControl.Instance.ShowHideLvlUP(false);
        }
    }
    private void NewLevel()
    {
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
            AspectName.Speed, AspectName.Hp,
            AspectName.Gold, AspectName.AttackSpeed
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
    Projectile_speed
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



