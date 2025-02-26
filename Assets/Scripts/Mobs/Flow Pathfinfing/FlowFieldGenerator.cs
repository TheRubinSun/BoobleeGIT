using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;

public class FlowFieldGenerator : MonoBehaviour
{
    public GridManager gridManager;
    public Transform player;

    private NativeArray<int> costField;
    private NativeArray<Vector2> flowField;

    public void GenerateFlowField()
    {
        int totalCells = gridManager.width * gridManager.height;
        costField = new NativeArray<int>(totalCells, Allocator.TempJob);
        flowField = new NativeArray<Vector2>(totalCells, Allocator.TempJob);

        for (int i = 0; i < totalCells; i++) costField[i] = int.MaxValue;

        Node targetNode = gridManager.GetNodeFromWorldPosition(player.position);
        costField[targetNodeIndex(targetNode)] = 0;

        // Запуск BFS через Job System
        FlowFieldJob flowFieldJob = new FlowFieldJob
        {
            gridWidth = gridManager.width,
            gridHeight = gridManager.height,
            costField = costField,
            flowField = flowField
        };

        JobHandle jobHandle = flowFieldJob.Schedule();
        jobHandle.Complete();

        ApplyFlowField();
        costField.Dispose();
        flowField.Dispose();
    }
    private void ApplyFlowField()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                int index = x + y * gridManager.width;
                gridManager.grid[x,y].flowDirection = flowField[index];
            }
        }
    }
    private int targetNodeIndex(Node node)
    {
        int x = Mathf.RoundToInt(node.worldPosition.x);
        int y = Mathf.RoundToInt(node.worldPosition.y);
        return x + y * gridManager.width;
    }
}
public struct FlowFieldJob : IJob
{
    public int gridWidth;
    public int gridHeight;

    public NativeArray<int> costField;
    public NativeArray<Vector2> flowField;

    public void Execute()
    {
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(0);

        while (queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();
            int currentCost = costField[currentIndex];

            foreach(int neighborIndex in GetNeighbors(currentIndex))
            {
                if(costField[neighborIndex] > currentCost + 1)
                {
                    costField[neighborIndex] = currentCost + 1;
                    queue.Enqueue(neighborIndex);
                }
            }
        }
        for(int i = 0; i < costField.Length; i++)
        {
            Vector2 bestDirection = Vector2.zero;
            int bestCost = costField[i];

            foreach(int neighborIndex in GetNeighbors(i))
            {
                if (costField[neighborIndex] < bestCost)
                {
                    bestCost = costField[neighborIndex];
                    bestDirection = GetDirection(i, neighborIndex);
                }
            }
            flowField[i] = bestDirection.normalized;
        }
    }
    private List<int> GetNeighbors(int index)
    {
        List<int> neighbors = new List<int>();
        int x = index % gridWidth;
        int y = index / gridHeight;

        if (x > 0) neighbors.Add(index - 1);
        if (x < gridWidth - 1) neighbors.Add(index + 1);
        if (y > 0) neighbors.Add(index - gridWidth);
        if (y < gridHeight - 1) neighbors.Add(index + gridWidth);

        return neighbors;
    }
    private Vector2 GetDirection(int fromIndex, int toIndex)
    {
        int fromX = fromIndex % gridWidth;
        int fromY = fromIndex / gridWidth;
        int toX = toIndex % gridWidth;
        int toY = toIndex / gridWidth;
        return new Vector2(toX - fromX, toY - fromY);
    }
}
