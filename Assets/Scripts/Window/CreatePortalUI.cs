using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Android.Gradle;
using UnityEngine;

public class CreatePortalUI : MonoBehaviour 
{
    public static CreatePortalUI Instance;

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject prefabLine;
    [SerializeField] List<GameObject> lines = new List<GameObject>();

    [SerializeField] private TextMeshProUGUI TimeText;
    private List<EnemyLine> enemyLines = new List<EnemyLine>();
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

        int i = 0;
        foreach (Mob mob in mobs)
        {
            GameObject line = Instantiate(prefabLine, contentParent);
            EnemyLine enemyLine = line.GetComponent<EnemyLine>();

            enemyLine.TextID.text = i.ToString();
            enemyLine.TextName.text = mob.Name;

            lines.Add(line);
            enemyLines.Add(enemyLine);
            i++;

        }
    }
    public void CreatePortal()
    {
        if (!(lines.Count > 0)) return;

        List<int> idPref = new List<int>();
        List<int> countSpawn = new List<int>();


        string fixedTextTime = TimeText.text.Trim().Replace("\u200B", "");
        float time;


        if (string.IsNullOrEmpty(fixedTextTime))
        {
            Debug.LogWarning("Ошибка! пустая строка времени");
            return;
        }
        if (!float.TryParse(fixedTextTime, out time))
        {
            Debug.LogWarning("Ошибка! данные времени не верны");
            return;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (enemyLines != null && enemyLines.Count > 0)
            {
                string fixedTextCount = enemyLines[i].TextCount.text.Trim().Replace("\u200B", "");
                if (string.IsNullOrEmpty(fixedTextCount)) break;
                Debug.Log($"Count: {fixedTextCount}");
                int count = int.Parse(fixedTextCount);
                if(count > 0)
                {
                    string fixedTextName = enemyLines[i].TextID.text.Trim().Replace("\u200B", "");
                    idPref.Add(int.Parse(fixedTextName));
                    countSpawn.Add(count);
                }
            }
        }

        SpawnMobs.Instance.SpawnPortal(idPref.ToArray(), countSpawn.ToArray(), time);
    }
    void ClearLines()
    {
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
        lines.Clear();
        enemyLines.Clear();
    }
}
