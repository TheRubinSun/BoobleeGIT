using UnityEngine;

public static class GlobalData
{
    public static string SavePath;
    public static int SaveInt;
    public static string cur_language;
    public static int cur_seed;
    public static Vector2 center;
    public static int chunkSize;

    //public static bool GOD_MODE;
    public static int VOLUME_SOUNDS;
    public static int VOLUME_MUSICS;

    public static bool isVisibleHpBarEnemy = true;
}
public static class BASE_VALUE_STATS_ARTEFACT
{
    public const int STRENGTH = 1;
    public const int AGILITY = 1;
    public const int INTELLIGENCE = 1;
    public const int HP = 2;
    public const int ARMOR = 1;
    public const int EVASION = 1;
    public const float MOV_SPEED = 0.02f;
    public const float ATT_RANGE = 0.3f;
    public const int ATT_SPEED = 5;
    public const float PROJ_SPEED = 0.3f;
    public const float EXPBUST = 0.03f;
    public const float MAGE_RESIS = 0.02f;
    public const float TECH_RESIS = 0.02f;
}
