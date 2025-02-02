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

    private void Awake()
    {
        // Проверка на существование другого экземпляра
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }
        iconItem = iconObj.GetComponent<Image>();
    }
    public void SetActive(bool turn)
    {
        this.SetActive(turn);
    }
    public void UpdateInfo(int numbSlot)
    {
        Item item = Inventory.Instance.GetSlot(numbSlot).Item;

        if (item == null || item.Id == 0) return;

        iconItem.sprite = item.Sprite;
        
        string colorName = "#" + ColorUtility.ToHtmlStringRGBA(item.GetColor());
        nameItem.text =  $"<size=8>{item.GetLocalizationName()}</size>\n<size=7><color={colorName}>{item.quality}</color></size>";

        string info = item.TypeItem.ToString();
        if(item is Weapon weapon)
        {
            info += "\nОружие";
            info += "\nТип урона: "+weapon.typeDamage;
            info += "\nУрон: " + weapon.damage;
            info += "\nСкорость атаки: " + weapon.attackSpeed;
            info += "\nДальность: " + weapon.range;
            
        }
        if(item.TypeItem == TypeItem.Food)
        {
            info += "\nEда";
        }
        info += "\nОписание: " + item.Description;
        infoItem.text = info;
    }
    public void ClearInfo()
    {
        iconItem.sprite = null;
        nameItem.text = "";
        infoItem.text = "";
    }
}
