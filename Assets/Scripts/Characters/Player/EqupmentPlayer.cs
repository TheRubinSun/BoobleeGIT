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
    public Slot slotMinionOne { get; set; }
    public Slot slotMinionTwo { get; set; }
    public Slot slotMinionThree { get; set; }
    public Slot slotMinionFour { get; set; }
    public Slot[] slotsEqup {  get; set; }

    [SerializeField] GameObject [] slotsObjEquip; //Массив слотов
    [SerializeField] Transform [] EquipSlotPrefab; //Префабы рук как родителя
    [SerializeField] Transform PlayerModel;

    Dictionary<int, GameObject> slots_Weapon = new Dictionary<int, GameObject>(); //В ячейки рук по порядку префабы оружия 
    Dictionary<int, GameObject> slots_minions= new Dictionary<int, GameObject>(); //В ячейки рук по порядку префабы миньонс 
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

        StartDataEquip();
    }
    public void StartDataEquip()
    {
        Item item = ItemsList.Instance.GetNoneItem();
        slotWeaponOne = new Slot(item, slotsObjEquip[0], TypeItem.Weapon);
        slotWeaponTwo = new Slot(item, slotsObjEquip[1], TypeItem.Weapon);
        slotWeaponThree = new Slot(item, slotsObjEquip[2], TypeItem.Weapon);
        slotWeaponFour = new Slot(item, slotsObjEquip[3], TypeItem.Weapon);
        slotArrmorOne = new Slot(item, slotsObjEquip[4], TypeItem.Armor);
        slotMinionOne = new Slot(item, slotsObjEquip[5], TypeItem.Minion);
        slotMinionTwo = new Slot(item, slotsObjEquip[6], TypeItem.Minion);
        slotMinionThree = new Slot(item, slotsObjEquip[7], TypeItem.Minion);
        slotMinionFour = new Slot(item, slotsObjEquip[8], TypeItem.Minion);
        slotsEqup = new Slot[] { slotWeaponOne, slotWeaponTwo, slotWeaponThree, slotWeaponFour, slotArrmorOne, slotMinionOne, slotMinionTwo, slotMinionThree, slotMinionFour };
        for (int i = 0; i < slotsEqup.Length; i++)
        {
            Inventory.Instance.UpdateSlotUI(slotsEqup[i]);
        }
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
            else if (slots_minions.ContainsKey(id)) slots_minions.Remove(id);
            Destroy(child.gameObject);  // Удаляем дочерний объект
        }
    }
    private void AddEquipOnSlot(int id)
    {
        if (id < 4) // Проверяем, существует ли Prefab перед инстанциированием и id до 4, так как 0 1 2 3 это слоты для оружия
        {
            int idPref = ItemsList.Instance.GetIdWeaponForNum(slotsEqup[id].Item);  //Получаем номер оружия из списка всех предметов (нужен порядковый номер оржия чтобы создать подходящий префаб)
            if(ResourcesData.GetWeaponPrefab(idPref) != null)
            {
                GameObject weaponObj = Instantiate(ResourcesData.GetWeaponPrefab(idPref), EquipSlotPrefab[id]);  //Создаем оружие в слот 
                LoadParametersWeapon(weaponObj, slotsEqup[id]); //Загружаем параметры с слолта в оружие
                Player.Instance.SetWeaponsObj(id, weaponObj.GetComponent<WeaponControl>()); //Передаем в словарь у игрока в список оружия
                slots_Weapon[id] = weaponObj; //Словарь в этом классе, пока не используется 
                Player.Instance.ChangeToggleWeapon(id);
            }
            else
            {
                Debug.LogWarning($"Ошибка 401, префаба нет {id}");
            }
        }
        else if(id > 4 && id < 9) //Для слотов миньон
        {
            int idPref = ItemsList.Instance.GetIdMinoinForNum(slotsEqup[id].Item);  //Получаем номер миньона из списка всех предметов (нужен порядковый номер миньоена чтобы создать подходящий префаб)
            if (ResourcesData.GetMinionPrefab(idPref) != null)
            {
                GameObject minionObj = Instantiate(ResourcesData.GetMinionPrefab(idPref), EquipSlotPrefab[id]);
                LoadParametersMinion(minionObj, slotsEqup[id]);
                Player.Instance.SetMinionsObj(id, minionObj.GetComponent<MinionControl>());
                slots_minions[id] = minionObj; //Словарь в этом классе, пока не используется 
            }
            else
            {
                Debug.LogWarning($"Ошибка 401, префаба нет {id}");
            }
        }
        else
        {
            Debug.LogWarning($"Ошибка 400 {id}");
        }
    }
    public void UpdateAllWeaponsStats()
    {
        foreach(KeyValuePair<int, GameObject> slotsWeapon in slots_Weapon)
        {
            LoadParametersWeapon(slotsWeapon.Value, slotsEqup[slotsWeapon.Key]); //Загружаем параметры с слолта в оружие
        }
    }
    public void UpdateAllMinionsStats()
    {
        foreach (KeyValuePair<int, GameObject> slotsMinions in slots_minions)
        {
            LoadParametersMinion(slotsMinions.Value, slotsEqup[slotsMinions.Key]); //Загружаем параметры с слота в миньона
        }
    }
    private void LoadParametersWeapon(GameObject weaponObj, Slot slot)
    {
        if (slot.Item is Gun gun)
        {
            //Debug.Log($"Gun: {gun.NameKey}: {gun.projectileSpeed}");
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(gun.damage, gun.attackSpeed, gun.projectileSpeed, gun.rangeType, gun.range, gun.conut_Projectiles, gun.spreadAngle, gun.typeDamage, PlayerModel,
                ResourcesData.GetProjectilesPrefab(gun.idBulletPref), gun.projectileSpeedCoof);
        }
        else if (slot.Item is MeleWeapon sword)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(sword.damage, sword.attackSpeed, 0, sword.rangeType, sword.range, sword.conut_Projectiles, 0f, sword.typeDamage, PlayerModel, null,0);
        }
        else if (slot.Item is Weapon weapon)
        {
            weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(weapon.damage, weapon.attackSpeed, 0, weapon.rangeType, weapon.range, weapon.conut_Projectiles, 0f, weapon.typeDamage, PlayerModel, null,0);
        }
    }
    private void LoadParametersMinion(GameObject minionObj, Slot slot)
    {
        if(slot.Item is Minion minion)
        {
            minionObj.GetComponent<MinionControl>().GetStatsMinion(minion.radius_search, minion.time_red, minion.move_speed, minion.typeMob);
        }
    }
}
