using System;
using UnityEngine.Tilemaps;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance;
    public GameObject chunkPrefab;
    public int chunkSize;
    public GameObject map;
    public Transform chunksParent;
    public static Vector2Int chunkOffset;

    public Dictionary<Vector2Int, GameObject> allChunks = new Dictionary<Vector2Int, GameObject>();
    //private void Start()
    //{
    //    GenerateChunks();
    //}
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    public IEnumerator GenerateChunks()
    {
        chunksParent = transform;
        Tilemap[] mapTilemaps = map.GetComponentsInChildren<Tilemap>();

        BoundsInt bounds = mapTilemaps[0].cellBounds;

        // Вычисляем смещение (чтобы минимальные координаты чанков начинались с (0,0))
        //chunkOffset = new Vector2Int(
        //    Mathf.FloorToInt(bounds.xMin / (float)chunkSize),
        //    Mathf.FloorToInt(bounds.yMin / (float)chunkSize)
        //);
        //GlobalData.ChunkOffset = chunkOffset;

        GlobalData.chunkSize = chunkSize;

        int centerX = (bounds.xMin + bounds.xMax) / 2;
        int centerY = (bounds.yMin + bounds.yMax) / 2;
        GlobalData.center = new Vector2(centerX, centerY);


        int MaxRangeX = Math.Max(centerX - (bounds.xMin), (bounds.xMax) - centerX);
        int MaxRangeY = Math.Max(centerY - (bounds.yMin), (bounds.yMax) - centerY);

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        // Счетчик для контроля нагрузки на кадр
        int chunksProcessed = 0;
        int chunksPerFrame = 4; // Регулируй это число (сколько чанков за 1 кадр)

        for (int dy = 0; dy <= MaxRangeY; dy += chunkSize)
        {
            for (int dx = 0; dx <= MaxRangeX; dx += chunkSize)
            {
                // Перебор по 4 направлениям (квадратный волновой рост)
                List<Vector2Int> offsets = new List<Vector2Int>()
                {
                    new Vector2Int(centerX + dx, centerY + dy),
                    new Vector2Int(centerX - dx, centerY + dy),
                    new Vector2Int(centerX + dx, centerY - dy),
                    new Vector2Int(centerX - dx, centerY - dy)
                };

                foreach(Vector2Int pos in offsets)
                {
                    if(visited.Contains(pos)) continue;
                    visited.Add(pos);

                    if (pos.x < bounds.xMin || pos.x >= bounds.xMax || pos.y < bounds.yMin || pos.y >= bounds.yMax) continue;

                    Vector2 worldPos = new Vector2(pos.x, pos.y);
                    Vector2 worldCenter = new Vector2(centerX, centerY);
                    Vector2Int chunkCoord = new Vector2Int(Mathf.FloorToInt((pos.x - centerX) / chunkSize), Mathf.FloorToInt((pos.y - centerY) / chunkSize));

                    CreateChunkAt(worldPos, chunkCoord, pos, mapTilemaps);
                    chunksProcessed++;

                    if(chunksProcessed >= chunksPerFrame)
                    {
                        chunksProcessed = 0;
                        yield return null;
                    }

                }





                //Vector2 positionChunk = new Vector2(dx, dy); // позиция в мировых координатах
                //GameObject chunkObj = Instantiate(chunkPrefab, positionChunk, Quaternion.identity, chunksParent);


                //Vector2Int chunkCoord = new Vector2Int(Mathf.FloorToInt((x - chunkOffset.x) / chunkSize), Mathf.FloorToInt((y - chunkOffset.y) / chunkSize));

                //allChunks[chunkCoord] = chunkObj;
                //chunkObj.name = $"Chunk_{x}_{y}";

                //ChunkID chunkID = chunkObj.AddComponent<ChunkID>();
                //chunkID.coord = chunkCoord;
                //chunkID.coordFloat = new Vector2((x - chunkOffset.x) / chunkSize, (y - chunkOffset.y) / chunkSize);

                //// Копирование тайлов
                //Tilemap[] chunkTilemaps = chunkObj.GetComponentsInChildren<Tilemap>();
                //foreach (var mapTilemap in mapTilemaps)
                //{
                //    Tilemap chunkTilemap = FindChildTilemap(chunkTilemaps, mapTilemap.name);
                //    if (chunkTilemap == null) continue;

                //    for (int cx = 0; cx < chunkSize; cx++)
                //    {
                //        for (int cy = 0; cy < chunkSize; cy++)
                //        {
                //            Vector3Int tilePos = new Vector3Int(x + cx, y + cy, 0);
                //            TileBase tile = mapTilemap.GetTile(tilePos);

                //            if (tile != null)
                //            {
                //                Vector3Int localPos = new Vector3Int(cx, cy, 0);
                //                chunkTilemap.SetTile(localPos, tile);

                //                Matrix4x4 matrix = mapTilemap.GetTransformMatrix(tilePos);
                //                chunkTilemap.SetTransformMatrix(localPos, matrix);
                //            }
                //        }
                //    }
                //}
                //chunkObj.SetActive(false);
            }
        }
        ChunkManager.UpdateAllChunks(allChunks);
        ChunkManager.isGenerated = true;
        yield return null;
    }

    private void CreateChunkAt(Vector2 worldPos,  Vector2Int chunkCoord, Vector2Int pos, Tilemap[] mapTilemaps)
    {
        GameObject chunkObj = Instantiate(chunkPrefab, worldPos, Quaternion.identity, chunksParent);
        chunkObj.name = $"Chunk_{chunkCoord.x}_{chunkCoord.y}";
        allChunks[chunkCoord] = chunkObj;


        Tilemap[] chunkTilemaps = chunkObj.GetComponentsInChildren<Tilemap>();
        foreach (var mapTilemap in mapTilemaps)
        {
            Tilemap chunkTilemap = FindChildTilemap(chunkTilemaps, mapTilemap.name);
            if (chunkTilemap == null) continue;

            for (int cx = 0; cx < chunkSize; cx++)
            {
                for (int cy = 0; cy < chunkSize; cy++)
                {
                    Vector3Int tilePos = new Vector3Int(pos.x + cx, pos.y + cy, 0);
                    TileBase tile = mapTilemap.GetTile(tilePos);

                    if (tile != null)
                    {
                        Vector3Int localPos = new Vector3Int(cx, cy, 0);
                        chunkTilemap.SetTile(localPos, tile);

                        Matrix4x4 matrix = mapTilemap.GetTransformMatrix(tilePos);
                        chunkTilemap.SetTransformMatrix(localPos, matrix);
                    }
                }
            }
        }
        //chunkObj.SetActive(false);
    }
    public void DeactivateAllChunks()
    {
        foreach(GameObject chunk in allChunks.Values)
        {
            chunk.SetActive(false);
        }
    }
    private void CopyTile()
    {

    }
    private Tilemap FindChildTilemap(Tilemap[] tilemaps, string name)
    {
        foreach(var t in tilemaps)
        {
            if(t.name == name)
                return t;
        }
        return null;
    }
}
