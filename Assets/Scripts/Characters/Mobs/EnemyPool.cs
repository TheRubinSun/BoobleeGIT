using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance;
    [SerializeField] private Transform enemyParent;

    private Dictionary<string, List<IPoolData>> enemyPools = new Dictionary<string, List<IPoolData>>();
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        
    }
    public EnemyPoolData GetEnemy(GameObject enemy_prefab, Vector2 newPos)
    {
        GameObject enemy;
        Rigidbody2D rb2;

        string nameEnemy = enemy_prefab.name;
        if (!enemyPools.ContainsKey(nameEnemy))
            enemyPools[nameEnemy] = new List<IPoolData>();

        List<IPoolData> pool = enemyPools[nameEnemy]; 
        for(int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].Obj.activeInHierarchy)
            {
                EnemyPoolData enemyD = (EnemyPoolData)pool[i];
                enemy = enemyD.Obj;
                rb2 = enemyD.Rb2;

                if(enemyD.Transform == null)
                    enemyD.NewTrans(enemy.transform);

                NewPos(enemyD.Transform, newPos);
                return enemyD;
            }
        }
        enemy = Instantiate(enemy_prefab, enemyParent);
        rb2 = enemy.GetComponent<Rigidbody2D>();
        EnemyPoolData newEnemy = new EnemyPoolData(enemy, enemy.GetComponent<BaseEnemyLogic>(), rb2);
        NewPos(newEnemy.Transform, newPos);

        enemy.SetActive(false);
        pool.Add(newEnemy);

        return newEnemy;
    }
    private void NewPos(Transform enemyPos, Vector2 newPos)
    {
        enemyPos.position = newPos;
    }
}
public class EnemyPoolData : IPoolData
{
    public GameObject Obj { get; private set; }
    public BaseEnemyLogic Logic { get; private set; }
    public Rigidbody2D Rb2 { get; private set; } // Кешируем физику тут!
    public Transform Transform { get; private set; }
    public EnemyPoolData(GameObject obj, BaseEnemyLogic logic, Rigidbody2D rb2)
    {
        Obj = obj;
        Transform = obj.transform;
        Logic = logic;
        Rb2 = rb2;
    }
    public void NewTrans(Transform enemyTrans)
    {
        Transform = enemyTrans;
    }
}
