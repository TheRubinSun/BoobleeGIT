using NUnit.Framework;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    public ParalaxLayer[] layers;

    void Start()
    {
        foreach (ParalaxLayer layer in layers)
        {
            layer.part1 = layer.layerObj.transform.GetChild(0);
            layer.part2 = layer.layerObj.transform.GetChild(1);
            layer.spriteWidth = layer.part1.GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void Update()
    {
        foreach (ParalaxLayer layer in layers)
        {
            // Двигаем оба фона влево
            layer.part1.Translate(Vector3.left * layer.speed * Time.deltaTime);
            layer.part2.Translate(Vector3.left * layer.speed * Time.deltaTime);

            // Проверка и зацикливание
            if (layer.part1.position.x <= -layer.spriteWidth)
            {
                layer.part1.position = new Vector3(layer.part2.position.x + layer.spriteWidth, layer.part1.position.y, layer.part1.position.z);
                SwapParts(layer);
            }
            else if (layer.part2.position.x <= -layer.spriteWidth)
            {
                layer.part2.position = new Vector3(layer.part1.position.x + layer.spriteWidth, layer.part2.position.y, layer.part2.position.z);
                SwapParts(layer);
            }
        }

    }

    // Меняет местами ссылки, чтобы всегда первый был "левее"
    void SwapParts(ParalaxLayer layer)
    {
        Transform temp = layer.part1;
        layer.part1 = layer.part2;
        layer.part2 = temp;
    }
}
[System.Serializable]
public class ParalaxLayer
{
    public GameObject layerObj;
    public float speed = 1f;
    public Transform part1 { get; set; }
    public Transform part2 { get; set; }
    public float spriteWidth { get; set; }
}
