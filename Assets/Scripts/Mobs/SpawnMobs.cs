using UnityEngine;

public class SpawnMobs : MonoBehaviour
{
     public static SpawnMobs Instance {  get; private set; }

    [SerializeField] Transform spawnpoint;
    [SerializeField] Transform parent;
    [SerializeField] Transform player;
    [SerializeField] GameObject[] mobs_prefab;

    private void Awake()
    {
        // Проверка на существование другого экземпляра
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
        }
    }
    private void Start()
    {
        if(player != null)
        {
            foreach(GameObject mob_pref in mobs_prefab)
            {
                mob_pref.GetComponent<BaseEnemyLogic>().player = player;
            }

        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            Start();
        }
    }
    public void SpawnMobsBut(int id)
    {
        if (mobs_prefab[id] != null)
        {
            Instantiate(mobs_prefab[id], parent);
        }
        else
        {
            Debug.LogWarning("Необходимо добавить префаб нового моба в SpawnMobs");
        }
    }
}
