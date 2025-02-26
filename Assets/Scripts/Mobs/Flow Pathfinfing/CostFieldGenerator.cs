using UnityEngine;

public class CostFieldGenerator:MonoBehaviour
{
    public GridManager gridManager;

    public void GenerateCostField()
    {
        foreach (Node node in gridManager.grid)
        {
            if (!node.isWalkable)
            {
                node.cost = 255; // Непроходимо
            }
            else
            {
                node.cost = 1; // Проходимо
            }
        }
    }
}
