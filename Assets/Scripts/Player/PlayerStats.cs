using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public RoleClass classPlayer;
    //Характеристики
    public int Cur_Hp { get; set; }
    public int Max_Hp { get; set;}
    public int Armor_Hp { get; set; }

    public float Mov_Speed { get; set; }

    public float Att_Range { get; set; }
    public int Att_Damage { get; set; }
    public int Att_Speed { get; set; }
    public int Proj_Speed { get; set; }

    public int level { get; set; }
    public int freeSkillPoints { get; set; }
    public int cur_exp { get; set; }
    public int nextLvl_exp { get; set; }

    //Knowlange
    public int MagicPoints { get; set; }
    public int TechniquePoints { get; set; }
    public int AdjacentPoints { get; set; }

    //Mod Attack
    public int count_Projectile { get; set; }

    private const int AddHP_PerLvl = 2;

    public PlayerStats() { }
    public void StartStats()
    {
        RoleClass rc = Classes.Instance.GetRoleClass("Shooter");
        Mov_Speed = rc.BonusSpeedMove;
        Max_Hp = rc.BonusHp;
        Att_Speed = rc.BonusAttackSpeed;
        Att_Range = rc.BonusRange;
        nextLvl_exp = 10;
        level = 0;
        Cur_Hp = Max_Hp;
        count_Projectile = 0;
        MagicPoints = 1;
        TechniquePoints = 1;
        AdjacentPoints = 0;
    }
    public void LoadStats(PlayerStats playerSaveData)
    {
        Cur_Hp = playerSaveData.Cur_Hp;
        Max_Hp = playerSaveData.Max_Hp;
        Armor_Hp = playerSaveData.Armor_Hp;

        Mov_Speed = playerSaveData.Mov_Speed;

        Att_Range = playerSaveData.Att_Range;
        Att_Damage = playerSaveData.Att_Damage;
        Att_Speed = playerSaveData.Att_Speed;
        Proj_Speed = playerSaveData.Proj_Speed;

        level = playerSaveData.level;
        freeSkillPoints = playerSaveData.freeSkillPoints;
        cur_exp = playerSaveData.cur_exp;
        nextLvl_exp = playerSaveData.nextLvl_exp;

        count_Projectile = playerSaveData.count_Projectile;
        MagicPoints = playerSaveData.MagicPoints;
        TechniquePoints = playerSaveData.TechniquePoints;
        AdjacentPoints = playerSaveData.AdjacentPoints;
    }
    public void AddMaxHPStat(int addMaxHp)
    {
        Max_Hp += AddHP_PerLvl;
        Cur_Hp += AddHP_PerLvl;
    }
    public void TakeDamageStat(int damage)
    {
        Cur_Hp -= (int)(Mathf.Max(damage / (1 + Armor_Hp / 10f), 1));
    }
    public bool PlayerHealStat(int count_heal)
    {
        if (Cur_Hp < Max_Hp)
        {
            if (Cur_Hp + count_heal >= Max_Hp)
            {
                Cur_Hp = Max_Hp;
            }
            else
            {
                Cur_Hp += count_heal;
            }
            return true;
        }
        return false;
    }
    public void AddExpStat(int add_exp)
    {
        cur_exp += add_exp;
        if (isNewLevel())
            LvlUpStat();
    }
    private bool isNewLevel()
    {
        if (cur_exp >= nextLvl_exp)
        {
            cur_exp -= nextLvl_exp;
            nextLvl_exp = (int)((nextLvl_exp + 10) * 1.4f);
            return true;
        }
        return false;
    }
    public void LvlUpStat()
    {
        level++;
        freeSkillPoints++;
        AddMaxHPStat(AddHP_PerLvl);

        string text = $"{level} lvl";
        Player.Instance.LvlUp(text);
    }
}
