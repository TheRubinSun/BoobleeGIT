using NUnit.Framework;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    public static Paralax Instance; 
    public ParalaxLayer[] layers;
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        foreach (ParalaxLayer layer in layers)
        {
            layer.part1 = layer.layerObj.transform.GetChild(0);
            layer.part2 = layer.layerObj.transform.GetChild(1);
            layer.part3 = layer.layerObj.transform.GetChild(2);

            var sprite = layer.part1.GetComponent<SpriteRenderer>().sprite;
            layer.spriteWidth = sprite.rect.width / sprite.pixelsPerUnit * layer.part1.localScale.x;

            Vector2 localPosStart = layer.layerObj.transform.InverseTransformPoint(new Vector2(leftEdge, 0f)); 
            // Устанавливаем начальные позиции
            layer.part1.localPosition = new Vector3(localPosStart.x, -1f, 0f);
            layer.part2.localPosition = new Vector3(localPosStart.x + layer.spriteWidth, -1f, 0f);
            layer.part3.localPosition = new Vector3(localPosStart.x + layer.spriteWidth * 2f, -1f, 0f);

            // Храним слои в массиве для удобной работы
            layer.parts = new Transform[] { layer.part1, layer.part2, layer.part3 };
        }
    }
    public void ParalaxUpdate()
    {
        Start();
    }

    void Update()
    {
        foreach (ParalaxLayer layer in layers)
        {
            layer.layerObj.transform.Translate(Vector3.left * layer.speed * Time.deltaTime);

            foreach (Transform part in layer.parts)
            {
                if (part.position.x + layer.spriteWidth < leftEdge)
                {
                    // Найдём самый правый элемент
                    Transform rightMost = GetRightMost(layer.parts);
                    float newX = rightMost.localPosition.x + layer.spriteWidth;

                    // Переместим "ушедший" слой в конец
                    part.localPosition = new Vector3(newX, part.localPosition.y, part.localPosition.z);
                }
            }
        }
    }

    Transform GetRightMost(Transform[] parts)
    {
        Transform rightMost = parts[0];
        foreach (Transform t in parts)
        {
            if (t.localPosition.x > rightMost.localPosition.x)
                rightMost = t;
        }
        return rightMost;
    }
}
[System.Serializable]
public class ParalaxLayer
{
    public GameObject layerObj;
    public float speed = 1f;
    public Transform part1 { get; set; }
    public Transform part2 { get; set; }
    public Transform part3 { get; set; }
    public Transform[] parts { get; set; }
    public float spriteWidth { get; set; }
}
