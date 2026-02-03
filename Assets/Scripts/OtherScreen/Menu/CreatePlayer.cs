using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayer : MonoBehaviour
{
    public static CreatePlayer Instance;

    //Создание игрока 
    private NewPlayer newPlayer;

    //Классы
    [SerializeField] private GameObject ClassBut;
    [SerializeField] private Transform classesParent;
    private ClassBut[] classesButs;

    [SerializeField] private Image hear_Image;
    [SerializeField] private Image head_Image;
    private int curSpriteID_hair;
    private int curSpriteID_head;
    private int selecetSave;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void ClearAndCreateRoleClasses(int saveID)
    {
        selecetSave = saveID;
        newPlayer = new NewPlayer(0, 0, 0, "c_warrior");
        RemoveAllButClasses();
        AddAllButClasses();
    }
    public void GetIdSpritesPlayer(out int idHair, out int idHead)
    {
        GlobalData.newPlayer = newPlayer;
        idHair = curSpriteID_hair;
        idHead = curSpriteID_head;
    }
    private void RemoveAllButClasses()
    {
        foreach (Transform t in classesParent.transform)
        {
            Destroy(t.gameObject);
        }
        classesButs = null;
    }
    private void AddAllButClasses()
    {
        classesButs = new ClassBut[Classes.GetClasses().Count];
        int i = 0;
        foreach (KeyValuePair<string, RoleClass> classR in Classes.GetClasses())
        {
            GameObject classBut = Instantiate(ClassBut, classesParent);

            Button classButLog = classBut.GetComponent<Button>();
            TextMeshProUGUI textBut = classBut.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Image onColor = classBut.transform.GetChild(1).GetChild(0).GetComponent<Image>();


            classesButs[i] = new ClassBut(classR.Key, textBut, onColor);
            textBut.text = classR.Value.name_language;
            //Debug.Log($"{classR.Key}: {classR.Value.name_language}");

            string classID = classR.Key;
            int index = i;

            classButLog.onClick.RemoveAllListeners();
            classButLog.onClick.AddListener(() => OnClassButtonClicked(index));

            i++;
        }
        AllClassButtonDeactivate();
    }
    private void OnClassButtonClicked(int id)
    {
        classesButs[id].onColor.color = GlobalColors.yesGreenColor;
        newPlayer.className = classesButs[id].className;
        AllClassButtonDeactivate();
    }
    private void AllClassButtonDeactivate()
    {
        foreach (ClassBut but in classesButs)
        {
            if (but.className == newPlayer.className)
            {
                but.onColor.color = GlobalColors.yesGreenColor;
            }
            else
            {
                but.onColor.color = GlobalColors.noRedColor;
            }

        }
    }
    public void HairChange(bool nextOrUndo)
    {
        if (nextOrUndo)
        {
            if (curSpriteID_hair < GameDataHolder.spritePlayerHairById.Count - 1)
            {
                curSpriteID_hair++;
            }
            else
            {
                curSpriteID_hair = 0;
            }
        }
        else
        {
            if (curSpriteID_hair == 0)
            {
                curSpriteID_hair = GameDataHolder.spritePlayerHairById.Count - 1;
            }
            else
            {
                curSpriteID_hair--;
            }
        }
        hear_Image.sprite = GameDataHolder.spritePlayerHairById[curSpriteID_hair];
    }
    public void HeadChange(bool nextOrUndo)
    {
        if (nextOrUndo)
        {
            if (curSpriteID_head < GameDataHolder.spritePlayerHeadById.Count - 1)
            {
                curSpriteID_head++;
            }
            else
            {
                curSpriteID_head = 0;
            }
        }
        else
        {
            if (curSpriteID_head == 0)
            {
                curSpriteID_head = GameDataHolder.spritePlayerHeadById.Count - 1;
            }
            else
            {
                curSpriteID_head--;
            }
        }
        head_Image.sprite = GameDataHolder.spritePlayerHeadById[curSpriteID_head];
    }
}
public struct NewPlayer
{
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Intelligence { get; set; }
    public string className { get; set; }
    public NewPlayer(int strength, int agility, int intelligence, string _className)
    {
        Strength = strength;
        Agility = agility;
        Intelligence = intelligence;
        className = _className;
    }
}
public class ClassBut
{
    public string className;
    public TextMeshProUGUI textClass;
    public Image onColor;
    public ClassBut(string name, TextMeshProUGUI _textClass, Image _onColor)
    {
        className = name;
        textClass = _textClass;
        onColor = _onColor;
    }
}
