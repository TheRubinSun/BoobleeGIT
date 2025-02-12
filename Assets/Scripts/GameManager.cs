using TMPro;
using UnityEngine;

public class GameManager: MonoBehaviour 
{
    public static GameManager Instance;

    bool BuildingMode;

    public int KillsEnemy = 0;
    public int enemisRemaining = 0;
    public TextMeshProUGUI InfoReaminingEnemy;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        EnemySetting.OnEnemyDeath += HandleEnemyDeath;
    }
    private void OnDisable()
    {
        EnemySetting.OnEnemyDeath -= HandleEnemyDeath;
    }
    private void HandleEnemyDeath(EnemySetting enemy)
    {
        KillsEnemy++;
        enemisRemaining--;
        Debug.Log($"Убит {enemy.Name} {enemy.max_Hp}");
        Player.Instance.AddExp(enemy.GiveExp);
        if (InfoReaminingEnemy != null)
        {
            InfoReaminingEnemy.text = $"Убито врагов {KillsEnemy} из {enemisRemaining}";
        }
    }





    public void SpawnMobs()
    {

    }
}
