using UnityEngine;

public class NodeP
{
    public bool walkable;      //Можно пройти?
    public Vector2 worldPos;   //Позиция в мире
    public Vector2Int gridPos; //Координата в массиве сетки
    public int pathID; // Новое поле для оптимизации 

    public int gCost; // Расстояние от старта
    public int hCost; // Расстояние до цели
    public int fCost => gCost + hCost; // Общая стоимость
    public NodeP parent;
    public NodeP(bool _walkable, Vector2 _worldPos, Vector2Int _gridPos)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridPos = _gridPos;
        pathID = 0;
    }
}
