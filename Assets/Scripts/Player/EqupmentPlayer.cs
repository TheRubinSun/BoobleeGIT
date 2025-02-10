using TMPro;
using UnityEngine;

public class EqupmentPlayer : MonoBehaviour
{
    public static EqupmentPlayer Instance { get; private set; }
    public Slot slotWeaponOne { get; set; }
    public Slot slotWeaponTwo { get; set; }
    public Slot slotWeaponThree { get; set; }
    public Slot slotWeaponFour { get; set; }
    public Slot slotArrmorOne { get; set; }
    public Slot[] slotsEqup {  get; set; }

    [SerializeField] GameObject [] slotsObjEquip;
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
    }
    private void Start()
    {
        Item item = ItemsList.Instance.GetNoneItem();
        slotWeaponOne = new Slot(item, slotsObjEquip[0], TypeItem.Weapon);
        slotWeaponTwo = new Slot(item, slotsObjEquip[1], TypeItem.Weapon);
        slotWeaponThree = new Slot(item, slotsObjEquip[2], TypeItem.Weapon);
        slotWeaponFour = new Slot(item, slotsObjEquip[3], TypeItem.Weapon);
        slotArrmorOne = new Slot(item, slotsObjEquip[4], TypeItem.Arrmor);
        slotsEqup = new Slot[] { slotWeaponOne, slotWeaponTwo, slotWeaponThree, slotWeaponFour, slotArrmorOne };
        for (int i = 0; i < slotsEqup.Length; i++)
        {
            Inventory.Instance.UpdateSlotUI(slotsEqup[i]);
        }
        //Загрузить сохранение если есть
        LoadSave();
    }
    public Slot GetSlot(int numbSlot)
    {
        //Debug.Log($"Слот: {numbSlot} {slotsEqup.Length}");
        return slotsEqup[numbSlot];
    }
    private void LoadSave()
    {

    }
}
