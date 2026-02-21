using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridNodes : MonoBehaviour
{
    public static GridNodes Instance;

    public bool WatchGrid;
    public LayerMask unwalkableMask;
    public Vector2Int gridWorldSize;
    public float nodeRadius;

    public NodeP[,] grid { get; private set; }
    private float nodeDiametr;
    private Vector2Int gridSize;


    Color32 colorBlock = new Color32(255, 0, 0, 20);
    Color32 noColor = new Color32(255, 255, 255, 10);
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        nodeDiametr = nodeRadius * 2;
        gridSize = new Vector2Int(Mathf.RoundToInt(gridWorldSize.x / nodeDiametr), Mathf.RoundToInt(gridWorldSize.y / nodeDiametr));
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new NodeP[gridSize.x, gridSize.y];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x/2 - Vector2.up * gridWorldSize.y/2;
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                Vector2 worldPosNode = worldBottomLeft + Vector2.right * (x * nodeDiametr) + Vector2.up * (y * nodeDiametr);
                bool walkable = !(Physics2D.OverlapCircle(worldPosNode, nodeRadius * 1.1f, unwalkableMask));
                grid[x, y] = new NodeP(walkable, worldPosNode, new Vector2Int(x, y));
            }
        }
    }
    public NodeP NodeFromWorldPoint(Vector2 worldPos) //ѕолучить узел по координатам
    {
        float precentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float precentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        precentX = Mathf.Clamp01(precentX);
        precentY = Mathf.Clamp01(precentY);
        int x = Mathf.RoundToInt((gridSize.x) * precentX);
        int y = Mathf.RoundToInt((gridSize.y) * precentY);
        return grid[x, y];
    }
    public List<NodeP> GetNeighbors(NodeP node)
    {
        List<NodeP> neighbors = new List<NodeP>();
        for(int x = - 1; x <= 1; x++)
        {
            for(int y = - 1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;

                if(checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    if(Mathf.Abs(x) == 1 &&  Mathf.Abs(y) == 1) //ѕроверка на диагональные движени€
                    {
                        NodeP horizontalNeigbor = grid[node.gridPos.x + x, node.gridPos.y];
                        NodeP vertiacalNeigbor = grid[node.gridPos.x, node.gridPos.y + y];

                        //Ќельз€ идти по диагонали, если р€дом есть блок (инчае моб будет срезать путь через блок-клетку)
                        if (!horizontalNeigbor.walkable || !vertiacalNeigbor.walkable) 
                            continue;
                    }
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
    public NodeP GetNeighborsRange(NodeP node, int radius)
    {
        List<NodeP> neighbors = new List<NodeP>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                bool isEdge = (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius);
                if(!isEdge) continue;

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    if (grid[checkX, checkY].walkable)
                        return grid[checkX, checkY];
                }
            }
        }
        return null;
    }

    public NodeP GetNodeByGridPos(int xPos, int yPos) => grid[Mathf.Clamp(xPos, 0, gridSize.x - 1), Mathf.Clamp(yPos, 0, gridSize.y - 1)];
    private void OnDrawGizmos()
    {
        if(!WatchGrid) return;
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));
        if(grid != null)
        {
            foreach(NodeP node in grid)
            {
                Gizmos.color = (node.walkable) ? noColor : colorBlock;
                Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiametr - .1f));
            }
        }
    }
    public void RefreshGrid()
    {
        Debug.Log("—етка обновлена!");
        CreateGrid(); // ѕерезапускаем логику сканировани€ физики
    }
}
