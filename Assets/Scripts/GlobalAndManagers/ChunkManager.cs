using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Transform PlayerPos;
    [SerializeField] private Vector2Int renderDistance;

    private int chunkSize;
    private Vector2Int lastPlayerChunk;

    private static Dictionary<Vector2Int, GameObject> chunkMap = new Dictionary<Vector2Int, GameObject>();
    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();

    public static bool isGenerated = false;

    private void Start()
    {
        if (PlayerPos == null)
            PlayerPos = GameObject.Find("PlayerModel").transform; // <-- автопоиск камеры
    }
    public static void UpdateAllChunks(Dictionary<Vector2Int, GameObject> _allChunks)
    {
        chunkMap.Clear();
        chunkMap = _allChunks;
    }
    private void Update()
    {
        if ((!isGenerated)) return;

        chunkSize = GlobalData.chunkSize;

        Vector2Int curentPlayerChunk= GetPlayerChunk(PlayerPos.position, chunkSize, GlobalData.center);
        if(curentPlayerChunk != lastPlayerChunk)
        {
            lastPlayerChunk = curentPlayerChunk;
            UpdateChunks();
        }
    }

    private void UpdateChunks()
    {
        HashSet<Vector2Int> chunksToActivate = new HashSet<Vector2Int>();

        for(int y = -renderDistance.y; y <= renderDistance.y; y++)
        {
            for(int x = -renderDistance.x; x <= renderDistance.x; x++)
            {
                Vector2Int chunkCoord = lastPlayerChunk + new Vector2Int(x, y);
                chunksToActivate.Add(chunkCoord);
            }
        }
        foreach(Vector2Int coord in chunksToActivate)
        {
            if(chunkMap.TryGetValue(coord, out var chunk))
            {
                if(!activeChunks.Contains(coord))
                {
                    chunk.SetActive(true);
                    activeChunks.Add(coord);
                }
            }
        }
        foreach(Vector2Int coord in new HashSet<Vector2Int>(activeChunks))
        {
            if(!chunksToActivate.Contains(coord))
            {
                if(chunkMap.TryGetValue(coord, out var chunk))
                {
                    chunk.SetActive(false);
                    activeChunks.Remove(coord);
                }
            }
        }
    }

    private Vector2Int GetPlayerChunk(Vector3 playerPos, int chunkSize, Vector2 center)
    {
        int x = Mathf.FloorToInt((playerPos.x - center.x) / chunkSize);
        int y = Mathf.FloorToInt((playerPos.y - center.y) / chunkSize);
        return new Vector2Int(x, y);
    }
}
