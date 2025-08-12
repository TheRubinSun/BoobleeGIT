using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LaserBatchRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    // Список лазеров как пар точек
    private List<(Vector3 start, Vector3 end)> lasers = new List<(Vector3, Vector3)>();

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> indices = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    [SerializeField] private Color32 colorLazer;
    [SerializeField] private int SortOrder;
    [SerializeField] private float tilingFactor = 1f; // сколько раз повторять текстуру на длине лазера
    public float laserWidth = 500f;

    void Awake()
    {
        mesh = new Mesh { name = "LaserBatchMesh" };
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = SortOrder;
        meshRenderer.material = Instantiate(meshRenderer.material);
        GetComponent<MaterialControl>().SetMaterial(meshRenderer.material);
        meshRenderer.material.color = colorLazer;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Очистить все лазеры
    public void ClearLasers()
    {
        lasers.Clear();
        UpdateMesh();
    }

    // Добавить лазер в список
    public void AddLaser(Vector3 start, Vector3 end)
    {
        lasers.Add((start, end));
        UpdateMesh();
    }

    // Удалить лазер по индексу
    public void RemoveLaser(int index)
    {
        if (index < 0 || index >= lasers.Count)
            return;

        lasers.RemoveAt(index);
        UpdateMesh();
    }

    // Перестроить меш из текущего списка лазеров
    private void UpdateMesh()
    {
        vertices.Clear();
        indices.Clear();
        uvs.Clear();

        for (int i = 0; i < lasers.Count; i++)
        {
            // Преобразуем в локальные координаты объекта для корректной отрисовки
            Vector3 localStart = transform.InverseTransformPoint(lasers[i].start);
            Vector3 localEnd = transform.InverseTransformPoint(lasers[i].end);

            Vector3 dir = (localEnd - localStart).normalized;
            Vector3 normal = Vector3.Cross(dir, Vector3.forward) * laserWidth * 0.5f;

            int indexStart = vertices.Count;
            float length = Vector3.Distance(localStart, localEnd);

            vertices.Add(localStart - normal);
            vertices.Add(localStart + normal);
            vertices.Add(localEnd - normal);
            vertices.Add(localEnd + normal);

            // UV с учётом длины и tilingFactor для повторения текстуры
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(length * tilingFactor, 0));
            uvs.Add(new Vector2(length * tilingFactor, 1));

            // Треугольники
            indices.Add(indexStart + 0);
            indices.Add(indexStart + 1);
            indices.Add(indexStart + 2);

            indices.Add(indexStart + 2);
            indices.Add(indexStart + 1);
            indices.Add(indexStart + 3);
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(indices, 0);
    }
}
