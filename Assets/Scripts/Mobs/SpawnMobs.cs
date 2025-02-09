using UnityEngine;

public class SpawnMobs : MonoBehaviour
{
     public static SpawnMobs Instance {  get; private set; }

    [SerializeField] Transform spawnpoint;
    [SerializeField] Transform parent;
    [SerializeField] Transform player;
    [SerializeField] GameObject daizen_prefab;
    [SerializeField] GameObject rainger_prefab;

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
            daizen_prefab.GetComponent<EnemyControl>().player = player;
            rainger_prefab.GetComponent<EnemyControl>().player = player;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            Start();
        }
    }
    public void SpawnMobsBut(int id)
    {
        switch (id)
        {
            case 0:
                Instantiate(daizen_prefab, parent);
                break;
            case 1:
                Instantiate(rainger_prefab, parent);
                break;
            default:
                Debug.LogWarning("Необходимо добавить префаб нового моба в SpawnMobs");
                break;
        }
    }
}
