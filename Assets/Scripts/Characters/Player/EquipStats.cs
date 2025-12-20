using UnityEngine;

public class EquipStats : MonoBehaviour
{
    public static EquipStats Instance { get; set; }
    public int Bonus_Equip_Strength { get; set; }
    public int Bonus_Equip_Agility { get; set; }
    public int Bonus_Equip_Intelligence { get; set; }
    public int Bonus_Equip_Hp { get; set; }
    public float Bonus_Equip_Mana { get; set; }
    public float Bonus_Equip_Regen_Mana { get; set; }
    public int Bonus_Equip_Armor { get; set; }
    public int Bonus_Equip_Evasion { get; set; }
    public float Bonus_Equip_Mov_Speed { get; set; }
    public float Bonus_Equip_Att_Range { get; set; }
    public int Bonus_Equip_Att_Damage { get; set; }
    public int Bonus_Equip_Att_Speed { get; set; }
    public float Bonus_Equip_Proj_Speed { get; set; }
    public float Bonus_Equip_ExpBust { get; set; }
    public float Bonus_Magic_Resis { get; set; }
    public float Bonus_Tech_Resis { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void AllNull()
    {
        Bonus_Equip_Strength = 0;
        Bonus_Equip_Agility = 0;
        Bonus_Equip_Intelligence = 0;
        Bonus_Equip_Hp = 0;
        Bonus_Equip_Mana = 0;
        Bonus_Equip_Regen_Mana = 0;
        Bonus_Equip_Armor = 0;
        Bonus_Equip_Evasion = 0;
        Bonus_Equip_Mov_Speed = 0f;
        Bonus_Equip_Att_Range = 0f;
        Bonus_Equip_Att_Damage = 0;
        Bonus_Equip_Att_Speed = 0;
        Bonus_Equip_Proj_Speed = 0f;
        Bonus_Equip_ExpBust = 0f;
        Bonus_Magic_Resis = 0f;
        Bonus_Tech_Resis = 0f;
    }
    public void ApplyArtifact(ArtifactObj artifact)
    {
        if (artifact == null) return;

        Bonus_Equip_Strength += artifact.Artif_Strength;
        Bonus_Equip_Agility += artifact.Artif_Agility;
        Bonus_Equip_Intelligence += artifact.Artif_Intelligence;
        Bonus_Equip_Hp += artifact.Artif_Hp;
        Bonus_Equip_Armor += artifact.Artif_Armor;
        Bonus_Equip_Evasion += artifact.Artif_Evasion;
        Bonus_Equip_Mov_Speed += artifact.Artif_Mov_Speed;
        Bonus_Equip_Att_Range += artifact.Artif_Att_Range;
        Bonus_Equip_Att_Speed += artifact.Artif_Att_Speed;
        Bonus_Equip_Proj_Speed += artifact.Artif_Proj_Speed;
        Bonus_Equip_ExpBust += artifact.Artif_ExpBust;
        Bonus_Magic_Resis += artifact.Artif_Mage_Resis;
        Bonus_Tech_Resis += artifact.Artif_Tech_Resis;
        Bonus_Equip_Att_Damage += artifact.Artif_Damage;
        Bonus_Equip_Mana += artifact.Artif_Mana;
        Bonus_Equip_Regen_Mana += artifact.Artif_ManaRegen;
    }
    public void RemoveArtifact(ArtifactObj artifact)
    {
        if (artifact == null) return;

        Bonus_Equip_Strength -= artifact.Artif_Strength;
        Bonus_Equip_Agility -= artifact.Artif_Agility;
        Bonus_Equip_Intelligence -= artifact.Artif_Intelligence;
        Bonus_Equip_Hp -= artifact.Artif_Hp;
        Bonus_Equip_Armor -= artifact.Artif_Armor;
        Bonus_Equip_Evasion -= artifact.Artif_Evasion;
        Bonus_Equip_Mov_Speed -= artifact.Artif_Mov_Speed;
        Bonus_Equip_Att_Range -= artifact.Artif_Att_Range;
        Bonus_Equip_Att_Speed -= artifact.Artif_Att_Speed;
        Bonus_Equip_Proj_Speed -= artifact.Artif_Proj_Speed;
        Bonus_Equip_ExpBust -= artifact.Artif_ExpBust;
        Bonus_Magic_Resis -= artifact.Artif_Mage_Resis;
        Bonus_Tech_Resis -= artifact.Artif_Tech_Resis;
        Bonus_Equip_Att_Damage -= artifact.Artif_Damage;
        Bonus_Equip_Mana -= artifact.Artif_Mana;
        Bonus_Equip_Regen_Mana -= artifact.Artif_ManaRegen;
    }
}
