using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnMobs : MonoBehaviour
{
    public static SpawnMobs Instance {  get; private set; }

    [SerializeField] Transform spawnpoint;
    [SerializeField] Transform[] portals_pos;
    public GameObject tech_portal_pref;
    public GameObject mage_portal_pref;
    public Transform parent;
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
            //if (gameObject.scene.name != "DontDestroyOnLoad")
            //{
            //    DontDestroyOnLoad(gameObject); // Обеспечивает сохранение объекта между сценами
            //}
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
    public void SpawnPortal(int[] idPrefabs, int[] countsSpawn, float time)
    {
        GameObject[] prefEnemies = new GameObject[idPrefabs.Length];
        for (int i = 0; i < idPrefabs.Length; i++)
        {
            prefEnemies[i] = mobs_prefab[idPrefabs[i]];
        }

        GameObject portal = Instantiate(tech_portal_pref, portals_pos[Random.Range(0, portals_pos.Length)]);
        portal.GetComponent<PortalLogic>().CreateEnemy(prefEnemies, countsSpawn, time, parent);
    }
}
