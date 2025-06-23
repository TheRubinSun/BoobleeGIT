using System;
using UnityEngine;

public static class GlobalData
{
    public static string SavePath;
    public static int SaveInt;
    public static string cur_language;
    public static int cur_seed;
    public static int cur_lvl_left;
    public static Vector2 center;
    public static int chunkSize;
   

    //public static bool GOD_MODE;
    public static float VOLUME_SOUNDS = 0.5f;
    public static float VOLUME_MUSICS = 0.5f;


    public static bool isVisibleHpBarEnemy = true;
    public static bool saveZone;
    public static string NAME_NEW_LOCATION = "Game_village";
    public static string NAME_NEW_LOCATION_TEXT;

    public static int difficulty = 1;
    public static int add_mobs = 1;

    
    //public static void SwitchVolume()
    //{

    //}

}
public static class BASE_VALUE_STATS_ARTEFACT
{
    public const int STRENGTH = 1;
    public const float ADD_FOR_CHAR_STRENGTH = 1;

    public const int AGILITY = 1;
    public const float ADD_FOR_CHAR_AGILITY = 1;

    public const int INTELLIGENCE = 1;
    public const float ADD_FOR_CHAR_INTELLIGENCE = 1;

    public const int HP = 2;
    public const float ADD_FOR_CHAR_HP = 2;

    public const int MANA = 2;
    public const float ADD_FOR_CHAR_MANA = 2;

    public const float MANA_REGEN = 0.125f;
    public const float ADD_FOR_CHAR_MANA_REGEN = 0.125f;

    public const int ARMOR = 1;
    public const float ADD_FOR_CHAR_ARMOR = 1f;

    public const int EVASION = 1;
    public const float ADD_FOR_CHAR_EVASION = 1f;

    public const float MOV_SPEED = 0.02f;
    public const float ADD_FOR_CHAR_MOV_SPEED = 0.02f;

    public const float ATT_RANGE = 0.3f;
    public const float ADD_FOR_CHAR_ATT_RANGE = 0.3f;

    public const int ATT_SPEED = 5;
    public const int ADD_FOR_CHAR_ATT_SPEED = 5;

    public const float PROJ_SPEED = 0.3f;
    public const float ADD_FOR_CHAR_PROJ_SPEED = 0.3f;

    public const float EXPBUST = 0.03f;
    public const float ADD_FOR_CHAR_EXPBUST = 0.03f;

    public const float MAGE_RESIS = 0.02f;
    public const float ADD_FOR_CHAR_MAGE_RESIS = 0.02f;

    public const float TECH_RESIS = 0.02f;
    public const float ADD_FOR_CHAR_TECH_RESIS = 0.02f;

    public const float DAMAGE = 1f;
    public const float ADD_FOR_CHAR_DAMAGE = 1f;
}
