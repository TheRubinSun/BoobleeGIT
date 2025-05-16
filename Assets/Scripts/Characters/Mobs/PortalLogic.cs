using System.Collections;
using UnityEngine;

public class PortalLogic : MonoBehaviour 
{

    [SerializeField] private Transform spawnPoint;
    private Animator anim;

    [SerializeField] private Animator[] worlds;
    [SerializeField] private Animator anim_child_world;

    private GameObject[] mobsPref;
    private GameObject mobPref;

    private int[] countsSpawn;
    private int countSpawn;

    private float time;
    private Transform parent;
    public void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void CreateEnemy(GameObject[] _mobsPref, int[] _countsSpawn, float _time, Transform _parent)
    {
        mobsPref = _mobsPref;
        countsSpawn = _countsSpawn;
        time = _time;
        parent = _parent;
    }
    public void SetDataPortal(SpawnMobPortal dataPortal, Transform _parent)
    {
        mobPref = dataPortal.enemy_prefab;
        countSpawn = dataPortal.endRandomCountSpawn;
        time = dataPortal.timeDurationSpawn;
        parent = _parent;
    }
    public void Event_RunningPortalForSolo()
    {
        anim.SetTrigger("CreWorld");
        AddWorldAnim();
        Debug.Log($"{mobPref.name} {countSpawn} {time}");
        StartCoroutine(SpawnEnemiesOverTime(mobPref, countSpawn, time, parent));
    }
    public void Event_RunningPortal()
    {
        anim.SetTrigger("CreWorld");
        AddWorldAnim();

        if (mobsPref == null)
        {
            Event_RunningPortalForSolo();
        }
        else
        {
            for (int i = 0; i < mobsPref.Length; i++)
            {
                StartCoroutine(SpawnEnemiesOverTime(mobsPref[i], countsSpawn[i], time, parent));
            }
        }
    }
    public void Event_DestroyPortal()
    {
        Destroy(gameObject);
    }

    private IEnumerator SpawnEnemiesOverTime(GameObject mob, int countSpawn, float time, Transform parent)
    {
        float timeBeetweenSpawn = time / countSpawn;
        for (int i = 0; i < countSpawn; i++)
        {
            Instantiate(mob, spawnPoint.position, Quaternion.identity, parent);
            yield return new WaitForSeconds(timeBeetweenSpawn);
        }
        Destroy(anim_child_world.gameObject);
        anim.SetTrigger("DestPort");
    }
    private void AddWorldAnim()
    {
        anim_child_world.runtimeAnimatorController = worlds[0].runtimeAnimatorController;
    }

}

