using System.Collections.Generic;
using UnityEngine;

public static class GameDataHolder
{
    public static SavesDataInfo savesDataInfo;
    public static PlayerData PlayerData;
    public static ArtifactsData ArtifactsData;
    public static WorldData WorldData;
    public static ItemsData ItemsData;
    public static RoleClassesData RoleClassesData;
    public static EnemyData EnemyData;
    public static ItemsDropOnEnemy ItemsDropOnEnemy;
    public static Sprite[] spriteList;
    //public static Dictionary<int, Sprite> spriteById;
    public static Dictionary<int, Sprite> spriteItemsById = new Dictionary<int, Sprite>();
    public static Dictionary<int, Sprite> spritePlayerHairById = new Dictionary<int, Sprite>();
    public static Dictionary<int, Sprite> spritePlayerHeadById = new Dictionary<int, Sprite>();
}
