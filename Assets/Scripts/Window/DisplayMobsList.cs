using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMobsList : MonoBehaviour 
{
    public static DisplayMobsList Instance;

    [SerializeField] GameObject linePrefab;
    [SerializeField] Transform parent; // Родитель для строк кнопок
    [SerializeField] List<GameObject> lines = new List<GameObject>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void DisplayLinesMobs(List<Mob> mobs)
    {
        ClearLines();

        foreach (Mob mob in mobs)
        {
            GameObject line = Instantiate(linePrefab, parent);
            line.GetComponentInChildren<TextMeshProUGUI>().text = mob.Name;
            lines.Add(line);

            // Получаем кнопку из слота
            Button button = line.GetComponent<Button>();
            if (button != null)
            {
                // Передаём текущий предмет в обработчик клика
                button.onClick.AddListener(() => OnLineClick(mob));
            }
        }
    }
    private void OnLineClick(Mob mob)
    {
        SpawnMobs.Instance.SpawnMobsBut(EnemyList.Instance.GetIdByMob(mob));
    }
    void ClearLines()
    {
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
        lines.Clear();
    }
}
