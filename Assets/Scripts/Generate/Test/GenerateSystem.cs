using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateSystem : MonoBehaviour
{
    public int width = 50;
    public int hight = 50;
    private int offsetWidth;
    private int offsetHight;
    int[,] map_index_type;

    public float noiseScale = 20f; // Масштаб шума для генерации карты

    [SerializeField] Tile[] tiles_ground;
    [SerializeField] Tile[] tiles_trees;
    [SerializeField] Tile[] tiles_stones;
    [SerializeField] Tilemap tilemap_ground;
    [SerializeField] Tilemap tilemap_objects;

    int max_seed = 9999999;
    int seed = 0;
    private void Start()
    {
        seed = Random.Range(1000000, max_seed);
        GenerateMap();
    }
    public void NewGenOldSeed()
    {
        GenerateMap();
    }
    public void NewGenNewSeed()
    {
        seed = Random.Range(1000000, max_seed);
        GenerateMap();
    }
    public void PrintIdArray()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = hight - 1; y >= -hight; y--)
        {
            for (int x = -width; x < width; x++)
            {
                sb.Append($" {map_index_type[x+offsetWidth, y+offsetHight]}");
            }
            sb.Append('\n');
        }
        Debug.Log( sb.ToString() );
    }
    private void GenerateMap()
    {
        tilemap_ground.ClearAllTiles();
        tilemap_objects.ClearAllTiles();

        //tilemap_ground.transform.position = new Vector2(-(width / 4), -(hight / 4));

        map_index_type = new int[width * 2, hight * 2];

        offsetWidth = width;
        offsetHight = hight;

        Random.InitState(seed);
        Debug.Log(seed);

        // Генерация смещения для шума на основе сида
        float offsetX = Random.Range(0f, 1000f);
        float offsetY = Random.Range(0f, 1000f);

        for (int y = -hight; y < hight; y++)
        {
            for (int x = -width; x < width; x++)
            {
                float perlinValue = Mathf.PerlinNoise(
                    (float)x / width * noiseScale + offsetX,
                    (float)y / hight * noiseScale + offsetY
                    );

                Tile tile;

                int typeBlock = GetTileIndexFromPerlinValue(perlinValue);

                map_index_type[x + offsetWidth, y + offsetHight] = typeBlock;
                tile = tiles_ground[typeBlock];

                tilemap_ground.SetTile(new Vector3Int(x, y), tile);

                GenerateTrees(x,y, perlinValue, typeBlock);
            }

        }
    }
    private void GenerateTrees(int x, int y, float PerlNoise, int typeBlock)
    {
        float objChance = Random.value;

        if (typeBlock == 3 && objChance < 0.15f)
        {
            tilemap_objects.SetTile(new Vector3Int(x, y), tiles_trees[Random.Range(0, tiles_trees.Length)]);
        }
        else if (typeBlock == 4 && objChance < 0.4f)
        {
            tilemap_objects.SetTile(new Vector3Int(x, y), tiles_trees[Random.Range(0, tiles_trees.Length)]);
        }

        if (typeBlock == 3 && objChance < 0.1f)
        {
            tilemap_objects.SetTile(new Vector3Int(x, y), tiles_stones[Random.Range(0, tiles_stones.Length)]);
        }
        else if (typeBlock == 4 && objChance < 0.05f)
        {
            tilemap_objects.SetTile(new Vector3Int(x, y), tiles_stones[Random.Range(0, tiles_stones.Length)]);
        }
    }
    private int GetTileIndexFromPerlinValue(float perlinValue)
    {
        if (perlinValue < 0.3f) return 0; // Deep Water
        if (perlinValue < 0.4f) return 1; // Water
        if (perlinValue < 0.46f) return 2; // Sand
        if (perlinValue < 0.6f) return 3; // Grass
        if (perlinValue < 0.8f) return 4; // Deep Grass
        if (perlinValue < 0.9f) return 5; // Snow
        if (perlinValue < 1f) return 6; // Stone
        return 4; // Deep Grass (по умолчанию)
    }
}
