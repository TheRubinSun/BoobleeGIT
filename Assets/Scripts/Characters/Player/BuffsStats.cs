using UnityEngine;

public class BuffsStats
{
    public int  Buff_Strength {  get; set; }
    public int  Buff_Agility { get; set; }
    public int  Buff_Intelligence { get; set; }
    public int  Buff_Max_Hp { get; set;}
    public float  Buff_Max_Mana { get; set; }
    public float  Buff_Regen_Mana { get; set; }
    public int  Buff_Armor { get; set; }
    public int  Buff_Evasion {  get; set; }
    public float  Buff_Mov_Speed { get; set; }
    public float  Buff_Att_Range { get; set; }
    public int  Buff_Att_Damage { get; set; }
    public int  Buff_Att_Speed { get; set; }
    public float  Buff_Proj_Speed { get; set; }
    public float  Buff_ExpBust {  get; set; }
    public float  Buff_Magic_Resis { get; set; }
    public float  Buff_Tech_Resis { get; set; }

    public void AllNull()
    {
        Buff_Strength = 0;
        Buff_Agility = 0;
        Buff_Intelligence = 0;
        Buff_Max_Hp = 0;
        Buff_Max_Mana = 0f;
        Buff_Regen_Mana = 0f;
        Buff_Armor = 0;
        Buff_Evasion = 0;
        Buff_Mov_Speed = 0f;
        Buff_Att_Range = 0f;
        Buff_Att_Damage = 0;
        Buff_Att_Speed = 0;
        Buff_Proj_Speed = 0f;
        Buff_ExpBust = 0f;
        Buff_Magic_Resis = 0f;
        Buff_Tech_Resis = 0f;
    }
    public void BuffNull(AllStats parametrs)
    {
        switch (parametrs)
        {
            case AllStats.Strength:
                Buff_Strength = 0;
                break;

            case AllStats.Agility:
                Buff_Agility = 0;
                break;

            case AllStats.Intelligence:
                Buff_Intelligence = 0;
                break;

            case AllStats.Max_Hp:
                Buff_Max_Hp = 0;
                break;

            case AllStats.Max_Mana:
                Buff_Max_Mana = 0f;
                break;

            case AllStats.Regen_Mana:
                Buff_Regen_Mana = 0f;
                break;

            case AllStats.Armor:
                Buff_Armor = 0;
                break;

            case AllStats.Evasion:
                Buff_Evasion = 0;
                break;

            case AllStats.Mov_Speed:
                Buff_Mov_Speed = 0f;
                break;

            case AllStats.Att_Range:
                Buff_Att_Range = 0f;
                break;

            case AllStats.Att_Damage:
                Buff_Att_Damage = 0;
                break;

            case AllStats.Att_Speed:
                Buff_Att_Speed = 0;
                break;

            case AllStats.Proj_Speed:
                Buff_Proj_Speed = 0f;
                break;

            case AllStats.ExpBust:
                Buff_ExpBust = 0f;
                break;

            case AllStats.Magic_Resis:
                Buff_Magic_Resis = 0f;
                break;

            case AllStats.Tech_Resis:
                Buff_Tech_Resis = 0f;
                break;
        }
    }
}
