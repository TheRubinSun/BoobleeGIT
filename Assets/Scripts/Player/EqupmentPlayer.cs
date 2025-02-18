using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    [SerializeField] GameObject [] slotsObjEquip; //Массив слотов
    [SerializeField] Transform [] EquipSlotPrefab; //Префабы рук как родителя
    [SerializeField] Transform PlayerModel;

    Dictionary<int, GameObject> slots_Weapon = new Dictionary<int, GameObject>(); //В ячейки рук по порядку префабы оружия 
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
        slotArrmorOne = new Slot(item, slotsObjEquip[4], TypeItem.Armor);
        slotsEqup = new Slot[] { slotWeaponOne, slotWeaponTwo, slotWeaponThree, slotWeaponFour, slotArrmorOne };
        for (int i = 0; i < slotsEqup.Length; i++)
        {
            Inventory.Instance.UpdateSlotUI(slotsEqup[i]);
        }
        //Загрузить сохранение если есть
    }
    public void LoadOrCreateEquipment(List<SlotTypeSave> equipment_items)
    {
        if (IsLoadEquipment(equipment_items))
        {
            UpdateAllSlots();
        }
    }
    private bool IsLoadEquipment(List<SlotTypeSave> equipment_items)
    {
        if (equipment_items != null)
        {
            for(int i = 0; i < equipment_items.Count; i++)
            {
                slotsEqup[i].Item = ItemsList.Instance.GetItemForNameKey(equipment_items[i].NameKey);
                slotsEqup[i].Count = equipment_items[i].count;
                Inventory.Instance.UpdateSlotUI(slotsEqup[i]);
            }
            return true;
        }
        return false;
    }



    public Slot GetSlot(int numbSlot)
    {
        //Debug.Log($"Слот: {numbSlot} {slotsEqup.Length}");
        return slotsEqup[numbSlot];
    }

    public void UpdateAllSlots()
    {
        foreach(Slot slot in slotsEqup)
        {
            PutOnEquip(slot);
        }
    }
    public void PutOnEquip(Slot slot)
    {

        for (int i = 0; i < slotsEqup.Length; i++)
        {
            if(slot.SlotObj.CompareTag("SlotEquip"))
            {
                if (slotsEqup[i] == slot)
                {
                    DeleteEquipOnSlot(i, EquipSlotPrefab[i]);

                    if(slot.Item.NameKey != "item_none")
                    {
                        AddEquipOnSlot(i);
                    }
                    
                }
            }
        }
    }
    private void DeleteEquipOnSlot(int id,Transform deleteAllChild)
    {
        foreach (Transform child in deleteAllChild)
        {
            if (slots_Weapon.ContainsKey(id)) slots_Weapon.Remove(id);
            Destroy(child.gameObject);  // Удаляем дочерний объект
        }
    }
    private void AddEquipOnSlot(int id)
    {
        int idPref = ItemsList.Instance.GetIdWeaponForNum(slotsEqup[id].Item);  //Получаем номер оружия из списка всех предметов (нужен порядковый номер оржия чтобы создать подходящий префаб)
        
        if (WeaponDatabase.GetWeaponPrefab(idPref) != null && id < 4) // Проверяем, существует ли Prefab перед инстанциированием и id до 4, так как 0 1 2 3 это слоты для оружия
        {
            GameObject weaponObj = Instantiate(WeaponDatabase.GetWeaponPrefab(idPref), EquipSlotPrefab[id]);  //Создаем оружие в слот 
            LoadParametersWeapon(weaponObj, slotsEqup[id]); //Загружаем параметры с слолта в оружие
            Player.Instance.SetWeaponsObj(id, weaponObj.GetComponent<WeaponControl>()); //Передаем в словарь у игрока в список оружия
            slots_Weapon[id] = weaponObj; //Словарь в этом классе, пока не используется 
        }
        else
        {
            Debug.LogWarning($"Ошибка 400 {id}");
        }
    }
    private void LoadParametersWeapon(GameObject weaponObj, Slot slot)
    {


        if (slot.Item is Gun gun)
        {
            //Debug.Log($"Gun: {gun.NameKey}: {gun.projectileSpeed}");
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(gun.damage, gun.attackSpeed, gun.projectileSpeed, gun.rangeType, gun.range, gun.conut_Projectiles, gun.typeDamage, PlayerModel,
                WeaponDatabase.GetProjectilesPrefab(gun.idBulletPref), gun.projectileSpeedCoof);
        }
        else if (slot.Item is Sword sword)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(sword.damage, sword.attackSpeed, 0, sword.rangeType, sword.range, sword.conut_Projectiles, sword.typeDamage, PlayerModel, null,0);
        }
        else if (slot.Item is Weapon weapon)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(weapon.damage, weapon.attackSpeed, 0, weapon.rangeType, weapon.range, weapon.conut_Projectiles, weapon.typeDamage, PlayerModel, null,0);
        }


        
    }

}
