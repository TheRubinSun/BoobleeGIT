using UnityEngine;

public class EquipStats : MonoBehaviour
{
    public static EquipStats Instance { get; set; }
    public int Bonus_Equip_Strength { get; set; }
    public int Bonus_Equip_Agility { get; set; }
    public int Bonus_Equip_Intelligence { get; set; }
    public int Bonus_Equip_Hp { get; set; }
    public int Bonus_Equip_Armor { get; set; }
    public int Bonus_Equip_Evasion { get; set; }
    public float Bonus_Equip_Mov_Speed { get; set; }
    public float Bonus_Equip_Att_Range { get; set; }
    public int Bonus_Equip_Att_Damage { get; set; }
    public int Bonus_Equip_Att_Speed { get; set; }
    public int Bonus_Equip_Proj_Speed { get; set; }
    public float Bonus_Equip_ExpBust { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
