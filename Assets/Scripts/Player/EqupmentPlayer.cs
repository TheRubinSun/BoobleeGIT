using System.Collections.Generic;
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
    [SerializeField] Transform [] EquipSlotPrefab;

    Dictionary<int, GameObject> slots_Weapon = new Dictionary<int, GameObject>();
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
        LoadSave();
    }
    public Slot GetSlot(int numbSlot)
    {
        //Debug.Log($"Слот: {numbSlot} {slotsEqup.Length}");
        return slotsEqup[numbSlot];
    }
    public void PutOnEquip()
    {

        for (int i = 0; i < slotsEqup.Length; i++)
        {
            if(slotsEqup[i] != null && slotsEqup[i].Item.NameKey != "item_none" && EquipSlotPrefab[i].childCount < 1)
            {
                int idPref = ItemsList.Instance.GetIdWeaponForNum(slotsEqup[i].Item);
                // Проверяем, существует ли Prefab перед инстанциированием
                if (WeaponDatabase.GetWeaponPrefab(idPref) != null)
                {
                    GameObject weaponObj = Instantiate(WeaponDatabase.GetWeaponPrefab(idPref), EquipSlotPrefab[i]);
                    LoadParametersWeapon(weaponObj, slotsEqup[i]);

                    if(i < 4) //До 4, так как 0 1 2 3 это слоты для оружия
                    {
                        Player.Instance.SetWeaponsObj(i, weaponObj.GetComponent<WeaponControl>());
                    }
                    
                    slots_Weapon[i] = weaponObj;
                }
                else
                {
                    Debug.LogWarning("Ошибка 400");
                }
            }
            else if(slotsEqup[i].Item.NameKey == "item_none" && EquipSlotPrefab[i] != null)
            {
                foreach (Transform child in EquipSlotPrefab[i])
                {
                    if(slots_Weapon.ContainsKey(i)) slots_Weapon.Remove(i);
                    Destroy(child.gameObject);  // Удаляем дочерний объект
                }
            }
        }
    }
    private void LoadParametersWeapon(GameObject weaponObj, Slot slot)
    {


        if (slot.Item is Gun gun)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(gun.damage, gun.attackSpeed, gun.projectileSpeed, gun.rangeType, gun.range, gun.typeDamage, WeaponDatabase.GetProjectilesPrefab(gun.idBulletPref));
        }
        else if (slot.Item is Sword sword)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(sword.damage, sword.attackSpeed, 0, sword.rangeType, sword.range, sword.typeDamage);
        }
        else if (slot.Item is Weapon weapon)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(weapon.damage, weapon.attackSpeed, 0, weapon.rangeType, weapon.range, weapon.typeDamage);
        }


        
    }
    private void LoadSave()
    {

    }
}
