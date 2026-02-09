using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    public static NewPlayer? newPlayer = default;

    public static int ID_head;
    public static int ID_hair;

    public static bool IsBigUI;

    //public static bool GOD_MODE;
    public static float VOLUME_SOUNDS = 0.5f;
    public static float VOLUME_MUSICS = 0.5f;


    public static bool isVisibleHpBarEnemy = true;
    public static bool saveZone;
    public static string NAME_NEW_LOCATION = "Game_village";
    public static string NAME_NEW_LOCATION_TEXT;

    public static int difficulty = 1;
    public static int add_mobs = 1;

    public static float ChanceCharOnWeapon = 0.10f;

    public static ScreenResolutions screen_resole;

    public static Inventory Inventory => Inventory.Instance;
    public static EqupmentPlayer EqupmentPlayer => EqupmentPlayer.Instance;
    public static EquipStats EquipStats =>  EquipStats.Instance;
    public static ShopLogic ShopLogic => ShopLogic.Instance;
    public static Player Player => Player.Instance;
    public static DisplayInfo DisplayInfo => DisplayInfo.Instance;
    public static GameManager GameManager => GameManager.Instance;
    public static CullingManager CullingManager => CullingManager.Instance;
    public static LocalizationManager LocalizationManager => LocalizationManager.Instance;
    public static Artifacts Artifacts => Artifacts.Instance;
    public static SoundsManager SoundsManager => SoundsManager.Instance;
    public static UIControl UIControl => UIControl.Instance;
    public static PlayerControl PlayerControl => PlayerControl.Instance;
    public static PlayerInputHandler PlayerInputHandler => PlayerInputHandler.Instance;
    public static CraftLogic CraftLogic => CraftLogic.Instance;
    public static DragAndDrop DragAndDrop => DragAndDrop.Instance;
    public static LvlUpLogic LvlUpLogic => LvlUpLogic.Instance;
    public static DisplayMobsList DisplayMobsList => DisplayMobsList.Instance;
    public static CreatePortalUI CreatePortalUI => CreatePortalUI.Instance;
    public static GameMenuLog GameMenuLog => GameMenuLog.Instance;
    public static Options Options => Options.Instance;
    public static SwitchKey SwitchKey => SwitchKey.Instance;
    public static GenInfoSaves GenInfoSaves => GenInfoSaves.Instance;
    public static SpawnMobs SpawnMobs => SpawnMobs.Instance;
    public static CreatePlayer CreatePlayer => CreatePlayer.Instance;
    public static ScreenResolutions GetScreenResolutions() => new ScreenResolutions(Screen.width, Screen.height, Screen.currentResolution.refreshRateRatio.numerator, Screen.currentResolution.refreshRateRatio.denominator);
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

