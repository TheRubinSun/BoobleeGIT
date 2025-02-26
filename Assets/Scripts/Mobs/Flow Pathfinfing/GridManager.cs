using UnityEngine;

public class GridManager : MonoBehaviour 
{
    public int width, height;
    public float cellSize = 1f;
    public Node[,] grid;

    private void Start()
    {
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        grid = new Node[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2 worldPos = new Vector2(x, y) * cellSize;
                bool isWalkable = !Physics2D.OverlapCircle(worldPos, cellSize * 0.4f, LayerMask.GetMask("Wall"));
                grid[x, y] = new Node(isWalkable, worldPos);
            }
        }
    }
    public Node GetNodeFromWorldPosition(Vector2 worldPos)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt(worldPos.x), 0, width - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(worldPos.y), 0, height - 1);
        return grid[x, y];
    }
}
public class Node
{
    public bool isWalkable;
    public Vector2 worldPosition;
    public int cost = int.MaxValue;
    public Vector2 flowDirection = Vector2.zero;

    public Node(bool isWalkable, Vector2 worldPosition)
    {
        this.isWalkable = isWalkable;
        this.worldPosition = worldPosition;
    }
}
