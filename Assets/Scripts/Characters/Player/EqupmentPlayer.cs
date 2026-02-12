using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EqupmentPlayer : MonoBehaviour, ISlot
{
    public static EqupmentPlayer Instance { get; private set; }
    public Slot SlotWeaponOne { get; set; }
    public Slot SlotWeaponTwo { get; set; }
    public Slot SlotWeaponThree { get; set; }
    public Slot SlotWeaponFour { get; set; }

    public Slot SlotArrmorOne { get; set; }

    public Slot SlotMinionOne { get; set; }
    public Slot SlotMinionTwo { get; set; }
    public Slot SlotMinionThree { get; set; }
    public Slot SlotMinionFour { get; set; }

    public Slot SlotArtefOne { get; set; }
    public Slot SlotArtefTwo { get; set; }
    public Slot SlotArtefThree { get; set; }
    public Slot SlotArtefFour { get; set; }
    public Slot[] SlotsEqup {  get; set; }

    [SerializeField] GameObject [] slotsObjEquip; //Массив слотов
    [SerializeField] Transform [] EquipSlotPrefab; //Руки, броня, миньоны - куда будут появлсятся предметы визуально
    [SerializeField] Transform PlayerModel;

    readonly Dictionary<int, GameObject> slots_Weapon = new (); //В ячейки рук по порядку префабы оружия 
    readonly Dictionary<int, GameObject> slots_minions= new (); //В ячейки рук по порядку префабы миньонс 
    readonly Dictionary<int, int> slots_artifacts = new (); //В ячейки рук по порядку префабы миньонс 
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
            //DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }

        StartDataEquip();
    }
    public void Start()
    {
        if (PlayerModel == null) PlayerModel = GlobalData.GameManager.PlayerModel;
    }
    public void GiveStartWeapon(Item item)
    {
        if(item == null) return;

        if (!GenInfoSaves.saveGameFiles[GlobalData.SaveInt].isStarted)
        {
            SlotWeaponFour.Item = item;
            SlotWeaponFour.Count = 1;
            PutOnEquip(SlotWeaponFour);
            SlotsManager.UpdateSlotUI(SlotWeaponFour);
        }
    }
    public void LockSlot(int numbSlot)
    {
        GameObject slotObj = GetSlotObj(numbSlot);
        SlotsEqup[numbSlot].enable = false;

        slotObj.GetComponent<MouseButtonHandler>().enabled = false; //Выкл кнопки перемещения
        slotObj.GetComponent<Button>().interactable = false;  //В нашем случае эта строка выполняет роль поменть цвет на неактивный
    }
    public void UnlockSlot(int numbSlot)
    {
        GameObject slotObj = GetSlotObj(numbSlot);
        SlotsEqup[numbSlot].enable = true;

        slotObj.GetComponent<MouseButtonHandler>().enabled = true; //Вкл кнопки перемещения
        slotObj.GetComponent<Button>().interactable = true; //В нашем случае эта строка выполняет роль поменть цвет на активный
    }
    private GameObject GetSlotObj(int numbSlot) => SlotsEqup[numbSlot].SlotObj;
    public void StartDataEquip()
    {
        Item item = ItemsList.GetNoneItem();
        SlotWeaponOne = new Slot(item, slotsObjEquip[0], TypeItem.Weapon);
        SlotWeaponTwo = new Slot(item, slotsObjEquip[1], TypeItem.Weapon);
        SlotWeaponThree = new Slot(item, slotsObjEquip[2], TypeItem.Weapon);
        SlotWeaponFour = new Slot(item, slotsObjEquip[3], TypeItem.Weapon);
        SlotArrmorOne = new Slot(item, slotsObjEquip[4], TypeItem.Armor);
        SlotMinionOne = new Slot(item, slotsObjEquip[5], TypeItem.Minion);
        SlotMinionTwo = new Slot(item, slotsObjEquip[6], TypeItem.Minion);
        SlotMinionThree = new Slot(item, slotsObjEquip[7], TypeItem.Minion);
        SlotMinionFour = new Slot(item, slotsObjEquip[8], TypeItem.Minion);
        SlotArtefOne = new Slot(item, slotsObjEquip[9], TypeItem.Artifact);
        SlotArtefTwo = new Slot(item, slotsObjEquip[10], TypeItem.Artifact);
        SlotArtefThree = new Slot(item, slotsObjEquip[11], TypeItem.Artifact);
        SlotArtefFour = new Slot(item, slotsObjEquip[12], TypeItem.Artifact);
        SlotsEqup = new Slot[] { SlotWeaponOne, SlotWeaponTwo, SlotWeaponThree, SlotWeaponFour, SlotArrmorOne, SlotMinionOne, SlotMinionTwo, SlotMinionThree, SlotMinionFour,
        SlotArtefOne, SlotArtefTwo, SlotArtefThree, SlotArtefFour};
        for (int i = 0; i < SlotsEqup.Length; i++)
        {
            SlotsManager.UpdateSlotUI(SlotsEqup[i]);
            //Inventory.Instance.UpdateSlotUI(slotsEqup[i]);
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
                SlotsEqup[i].Item = ItemsList.GetItemForNameKey(equipment_items[i].NameKey);
                SlotsEqup[i].Count = equipment_items[i].count;
                SlotsEqup[i].artifact_id = equipment_items[i].artefact_id;
                GlobalData.Inventory.UpdateSlotUI(SlotsEqup[i]);
            }
            return true;
        }
        return false;
    }

    public Slot GetSlot(SlotRequest request)
    {
        //Debug.Log($"Слот: {numbSlot} {slotsEqup.Length}");
        return SlotsEqup[request.index];
    }
    public void UpdateAllSlots()
    {
        int id = 0;
        foreach (Slot slot in SlotsEqup)
        {
            PutOnEquip(slot, id);
            id++;
        }
    }
    public void PutOnEquip(Slot slot, int id) //Проверить префаб
    {
        DeleteAttributeArtifact(id);
        if (id < 9)
        {
            DeleteEquipOnSlot(id, EquipSlotPrefab[id]);
        }
        if (slot.Item.NameKey != "item_none")
        {
            AddEquipOnSlot(id);
        }
        UpdateAttribute();
    }
    public void UpdateAttribute()
    {
        GlobalData.Player.UpdateAllStats();
        GlobalData.Player.UpdateHP();
        GlobalData.Player.UpdateMANA();
        GlobalData.Player.UpdateRegenMANA();

        GlobalData.DisplayInfo.UpdateInfoStatus();
        UpdateAllWeaponsStats();
    }
    public void PutOnEquip(Slot slot) //Проверить префаб
    {
        //if (!slot.SlotObj.GetComponent<MouseButtonHandler>().enabled) return;

        if (slot.SlotObj.CompareTag("SlotEquip"))
        {
            int id = Array.IndexOf(SlotsEqup, slot);
            PutOnEquip(slot, id);
        }
    }
    //public void UpdateAllSlots()
    //{
    //    foreach(Slot slot in slotsEqup)
    //    {
    //        PutOnEquip(slot);
    //    }
    //}
    //public void PutOnEquip(Slot slot)
    //{

    //    for (int i = 0; i < slotsEqup.Length; i++)
    //    {
    //        if(slot.SlotObj.CompareTag("SlotEquip"))
    //        {
    //            if (slotsEqup[i] == slot)
    //            {
    //                DeleteEquipOnSlot(i, EquipSlotPrefab[i]);

    //                if(slot.Item.NameKey != "item_none")
    //                {
    //                    AddEquipOnSlot(i);
    //                }

    //            }
    //        }
    //    }
    //}
    public void ChangeVectorOrNotWeapon(int idSlot)
    {
        GlobalData.Player.ChangeToggleWeapon(idSlot);
    }
    private void DeleteEquipOnSlot(int id,Transform deleteAllChild)
    {
        foreach (Transform child in deleteAllChild)
        {
            if (slots_Weapon.ContainsKey(id)) 
                slots_Weapon.Remove(id);
            else if (slots_minions.ContainsKey(id))
                slots_minions.Remove(id);
            Destroy(child.gameObject);  // Удаляем дочерний объект
        }
    }
    private void AddEquipOnSlot(int id)
    {
        if (id < 4) // Проверяем, существует ли Prefab перед инстанциированием и id до 4, так как 0 1 2 3 это слоты для оружия
        {
            int idPref = ItemsList.GetIdWeaponForItem(SlotsEqup[id].Item);  //Получаем номер оружия из списка всех предметов (нужен порядковый номер оржия чтобы создать подходящий префаб)
            if(ResourcesData.GetWeaponPrefab(SlotsEqup[id].Item.NameKey) != null)
            {
                GameObject weaponObj = Instantiate(ResourcesData.GetWeaponPrefab(SlotsEqup[id].Item.NameKey), EquipSlotPrefab[id]);  //Создаем оружие в слот 
                LoadParametersWeapon(weaponObj, SlotsEqup[id]); //Загружаем параметры с слолта в оружие
                GlobalData.Player.SetWeaponsObj(id, weaponObj.GetComponent<WeaponControl>()); //Передаем в словарь у игрока в список оружия
                slots_Weapon[id] = weaponObj; //Словарь в этом классе, пока не используется 
                slots_artifacts[id] = SlotsEqup[id].artifact_id; //Оружия тоже артефакты
                GlobalData.Player.ChangeToggleWeapon(id);


                UpdateAllArtifactsAtribute();
                //if (SlotsEqup[id].artifact_id > 0)
                //{
                //    LoadAttributeArtifact(SlotsEqup[id].artifact_id); //Загружаем параметры с слота артефакта
                //}
            }
            else
            {
                Debug.LogWarning($"Ошибка 401, префаба нет {id}");
            }
        }
        else if(id > 4 && id < 9) //Для слотов миньон
        {
            int idPref = ItemsList.GetIdMinoinForItem(SlotsEqup[id].Item);  //Получаем номер миньона из списка всех предметов (нужен порядковый номер миньоена чтобы создать подходящий префаб)
            if (ResourcesData.GetMinionPrefab(idPref) != null)
            {
                GameObject minionObj = Instantiate(ResourcesData.GetMinionPrefab(idPref), EquipSlotPrefab[id]);
                LoadParametersMinion(minionObj, SlotsEqup[id]);
                GlobalData.Player.SetMinionsObj(id, minionObj.GetComponent<MinionControl>());
                slots_minions[id] = minionObj; //Словарь в этом классе, пока не используется 
            }
            else
            {
                Debug.LogWarning($"Ошибка 401, префаба нет {id}");
            }
        }
        else if (id > 8 && id < 13) //Для слотов артифакты
        {
            //LoadAttributeArtifact(slotsEqup[id].artifact_id);
            slots_artifacts[id] = SlotsEqup[id].artifact_id;
            UpdateAllArtifactsAtribute();
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
            LoadParametersWeapon(slotsWeapon.Value, SlotsEqup[slotsWeapon.Key]); //Загружаем параметры с слолта в оружие
        }
    }
    public void UpdateAllMinionsStats()
    {
        foreach (KeyValuePair<int, GameObject> slotsMinions in slots_minions)
        {
            LoadParametersMinion(slotsMinions.Value, SlotsEqup[slotsMinions.Key]); //Загружаем параметры с слота в миньона
        }
    }
    public void UpdateAllArtifactsAtribute()
    {
        GlobalData.EquipStats.AllNull();
        foreach (KeyValuePair<int, int> slot_artifact in slots_artifacts)
        {
            if(slot_artifact.Value > 0)
                LoadAttributeArtifact(slot_artifact.Value); //Загружаем параметры с слота артефакта
        }
    }
    private void LoadParametersWeapon(GameObject weaponObj, Slot slot)
    {
        if(slot.Item is Weapon weapon)
        {
            weaponObj.GetComponent<WeaponControl>().ApplyStats(weapon, PlayerModel);
        }
        
        //if (slot.Item is StaffBullet staff)
        //{
        //    weaponObj.GetComponent<StaffLogic>().GetStatsStaff(staff, staff.damage, staff.attackSpeedCoof, staff.addAttackSpeed, staff.projectileSpeed, staff.rangeType, staff.range, staff.conut_Projectiles, staff.spreadAngle, staff.typeDamage, PlayerModel, staff.manaCost,
        //        ResourcesData.GetProjectilesPrefab(staff.idPrefabShot), staff.projectileSpeedCoof, staff.effectID);
        //}
        //else if(slot.Item is LazerStaffGun stuffLGun)
        //{
        //    weaponObj.GetComponent<RayStaffLogic>().GetStatsRayStaff(stuffLGun, stuffLGun.damage, stuffLGun.attackSpeedCoof, stuffLGun.addAttackSpeed, 0, stuffLGun.rangeType, stuffLGun.range, stuffLGun.conut_Projectiles, 0f, stuffLGun.typeDamage, stuffLGun.CountPenetration, stuffLGun.manaCost, PlayerModel, stuffLGun.idPrefabShot, null, 0 ,stuffLGun.effectID);
        //}
        //else if (slot.Item is LazerGun lGun)
        //{
        //    weaponObj.GetComponent<RayWeaponLogic>().GetStatsLazerGun(lGun, lGun.damage, lGun.attackSpeedCoof, lGun.addAttackSpeed, 0, lGun.rangeType, lGun.range, lGun.conut_Projectiles, 0f, lGun.typeDamage, lGun.CountPenetration, PlayerModel, lGun.idPrefabShot , null, 0, lGun.effectID);
        //}
        //else if (slot.Item is Gun gun)
        //{
        //    //Debug.Log($"Gun: {gun.NameKey}: {gun.projectileSpeed}");
        //    weaponObj.GetComponent<RangeWeaponLogic>().GetStatsWeapon(gun, gun.damage, gun.attackSpeedCoof, gun.addAttackSpeed, gun.projectileSpeed, gun.rangeType, gun.range, gun.conut_Projectiles, gun.spreadAngle, gun.typeDamage, PlayerModel,
        //        ResourcesData.GetProjectilesPrefab(gun.idPrefabShot), gun.projectileSpeedCoof, gun.effectID);
        //}
        //else if (slot.Item is MeleWeapon sword)
        //{
        //    weaponObj.GetComponent<MeleWeaponLogic>().GetStatsWeapon(sword, sword.damage, sword.attackSpeedCoof, sword.addAttackSpeed, 0, sword.rangeType, sword.range, sword.conut_Projectiles, 0f, sword.typeDamage, PlayerModel, null, 0, sword.effectID);
        //}
        //else if (slot.Item is Weapon weapon)
        //{
        //    weaponObj.GetComponent<WeaponControl>().GetStatsWeapon(weapon, weapon.damage, weapon.attackSpeedCoof, weapon.addAttackSpeed, 0, weapon.rangeType, weapon.range, weapon.conut_Projectiles, 0f, weapon.typeDamage, PlayerModel, null, 0, weapon.effectID);
        //}
    }
    private void LoadParametersMinion(GameObject minionObj, Slot slot)
    {
        if(slot.Item is Minion minion)
        {
            if(minion is GunMinion gunMinion)
            {
                minionObj.GetComponent<GunMinCon>().GetStatsGunMinion(minion.radius_search, minion.time_red, minion.move_speed, minion.typeMob, gunMinion.bulletId, gunMinion.effectId, gunMinion.damage, gunMinion.speedProj);
            }
            else
            {
                minionObj.GetComponent<MinionControl>().GetStatsMinion(minion.radius_search, minion.time_red, minion.move_speed, minion.typeMob);
            }

        }
    }
    private void LoadAttributeArtifact(int idArt)
    {
        //EquipStats equipStats = EquipStats.Instance;
        //Artifacts allArtifacts = Artifacts.Instance;
        ArtifactObj artifact = GlobalData.Artifacts.GetArtifact(idArt); // Получаем артефакт по ID

        // Загружаем атрибуты артефакта в EquipStats
        if(artifact != null)
        {
            GlobalData.EquipStats?.ApplyArtifact(artifact);
        }
        //GlobalData.EquipStats.Bonus_Equip_Strength += artifact.Artif_Strength;
        //GlobalData.EquipStats.Bonus_Equip_Agility += artifact.Artif_Agility;
        //GlobalData.EquipStats.Bonus_Equip_Intelligence += artifact.Artif_Intelligence;
        //GlobalData.EquipStats.Bonus_Equip_Hp += artifact.Artif_Hp;
        //GlobalData.EquipStats.Bonus_Equip_Armor += artifact.Artif_Armor;
        //GlobalData.EquipStats.Bonus_Equip_Evasion += artifact.Artif_Evasion;
        //GlobalData.EquipStats.Bonus_Equip_Mov_Speed += artifact.Artif_Mov_Speed;
        //GlobalData.EquipStats.Bonus_Equip_Att_Range += artifact.Artif_Att_Range;
        //GlobalData.EquipStats.Bonus_Equip_Att_Speed += artifact.Artif_Att_Speed;
        //GlobalData.EquipStats.Bonus_Equip_Proj_Speed += artifact.Artif_Proj_Speed;
        //GlobalData.EquipStats.Bonus_Equip_ExpBust += artifact.Artif_ExpBust;
        //GlobalData.EquipStats.Bonus_Magic_Resis += artifact.Artif_Mage_Resis;
        //GlobalData.EquipStats.Bonus_Tech_Resis += artifact.Artif_Tech_Resis;
        //GlobalData.EquipStats.Bonus_Equip_Att_Damage += artifact.Artif_Damage;
        //GlobalData.EquipStats.Bonus_Equip_Mana += artifact.Artif_Mana;
        //GlobalData.EquipStats.Bonus_Equip_Regen_Mana += artifact.Artif_ManaRegen;
    }
    private void DeleteAttributeArtifact(int idSlot)
    {
        if (!slots_artifacts.ContainsKey(idSlot) || slots_artifacts[idSlot] < 1) return;

        int artId = slots_artifacts[idSlot];
        slots_artifacts.Remove(idSlot);

        ArtifactObj artifact = GlobalData.Artifacts.GetArtifact(artId);


        //EquipStats equipStats = EquipStats.Instance;

        // Вычитаем атрибуты артефакта из EquipStats
        if (artifact != null)
        {
            GlobalData.EquipStats?.RemoveArtifact(artifact);
        }

        //equipStats.Bonus_Equip_Strength -= artifact.Artif_Strength;
        //equipStats.Bonus_Equip_Agility -= artifact.Artif_Agility;
        //equipStats.Bonus_Equip_Intelligence -= artifact.Artif_Intelligence;
        //equipStats.Bonus_Equip_Hp -= artifact.Artif_Hp;
        //equipStats.Bonus_Equip_Armor -= artifact.Artif_Armor;
        //equipStats.Bonus_Equip_Evasion -= artifact.Artif_Evasion;
        //equipStats.Bonus_Equip_Mov_Speed -= artifact.Artif_Mov_Speed;
        //equipStats.Bonus_Equip_Att_Range -= artifact.Artif_Att_Range;
        //equipStats.Bonus_Equip_Att_Speed -= artifact.Artif_Att_Speed;
        //equipStats.Bonus_Equip_Proj_Speed -= artifact.Artif_Proj_Speed;
        //equipStats.Bonus_Equip_ExpBust -= artifact.Artif_ExpBust;
        //equipStats.Bonus_Magic_Resis -= artifact.Artif_Mage_Resis;
        //equipStats.Bonus_Tech_Resis -= artifact.Artif_Tech_Resis;
        //equipStats.Bonus_Equip_Att_Damage -= artifact.Artif_Damage;
        //equipStats.Bonus_Equip_Mana -= artifact.Artif_Mana;
        //equipStats.Bonus_Equip_Regen_Mana -= artifact.Artif_ManaRegen;
    }

}
public enum TypeEquipSlot
{
    None,
    Weapon,
    Armor,
    Minion,
    Artifact
}
