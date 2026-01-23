using System.Text;
using TMPro;
using UnityEngine;

public class DisplayPlayerStats : MonoBehaviour
{
    DisplayInfo displayInfo = DisplayInfo.Instance;
    //public RectTransform content;

    [SerializeField] TextMeshProUGUI Status_Info_Name_Text;
    [SerializeField] TextMeshProUGUI Status_Info;

    [SerializeField] TextMeshProUGUI Strength_Text;
    [SerializeField] TextMeshProUGUI Strength_Bonus_Text;
    [SerializeField] TextMeshProUGUI Agility_Text;
    [SerializeField] TextMeshProUGUI Agility_Bonus_Text;
    [SerializeField] TextMeshProUGUI Intelligence_Text;
    [SerializeField] TextMeshProUGUI Intelligence_Bonus_Text;

    public void UpdateAttribute(string str, string str_b, string agil, string agil_b, string intel, string intel_b)
    {
        Strength_Text.text = str;
        Strength_Bonus_Text.text = str_b;
        Agility_Text.text = agil;
        Agility_Bonus_Text.text = agil_b;
        Intelligence_Text.text = intel;
        Intelligence_Bonus_Text.text = intel_b;
    }
    public void UpdateOtherInfo(string infoName, string OtherInfo)
    {
        Status_Info_Name_Text.text = infoName;
        Status_Info.text = OtherInfo;
    }
}
