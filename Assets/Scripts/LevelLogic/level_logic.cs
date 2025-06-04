using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class level_logic : MonoBehaviour
{
    [SerializeField] private List<SpawnMobPortal> spawnEnemy = new List<SpawnMobPortal>();
    [SerializeField] private float timeToBackHome;
    [SerializeField] private GameObject portalHome;
    [SerializeField] private Transform MobsLogicOBJ;
    [SerializeField] private Transform SpawnMobsParent;

    private float[] cooldownEnemySpawn;

    [SerializeField] private Transform[] PosForPortals;
    //private Transform parentPortal;
    private SpawnMobs spawnMobsLogic;

    private int countMobs;

    private void Start()
    {
        cooldownEnemySpawn = new float[spawnEnemy.Count];
        spawnMobsLogic = MobsLogicOBJ.GetComponent<SpawnMobs>();
        //parentPortal = this.transform;
        
        foreach (SpawnMobPortal spawnMobPortal in spawnEnemy)
        {
            if(spawnMobPortal.startSpawn)
            {
                Transform pos = PosForPortals[Random.Range(0, PosForPortals.Length)];
                SpawnPortal(spawnMobPortal, pos);
            }
        }

        StartCoroutine(ProcessWave());
        //бїИђ ьюсют
        //foreach (SpawnMobPortal mob in spawnEnemy)
        //{
        //    int countTemp = Random.Range(mob.minCountSpawn, mob.maxCountSpawn);
        //    mob.endRandomCountSpawn = countTemp;
        //    //countMobs += countTemp;
        //}

    }
    private void Update()
    {
        for (int i = 0; i < cooldownEnemySpawn.Length; i++)
        {
            cooldownEnemySpawn[i] += Time.deltaTime;

            if (cooldownEnemySpawn[i] >= spawnEnemy[i].coolDown)
            {
                Transform pos = PosForPortals[Random.Range(0, PosForPortals.Length)];
                SpawnPortal(spawnEnemy[i], pos);
                cooldownEnemySpawn[i] = 0;
            }
        }
    }
    IEnumerator ProcessWave()
    {
        float elapcedTime = 0f;
        elapcedTime += Time.deltaTime;

        yield return new WaitForSeconds(timeToBackHome);

        portalHome.SetActive(true);

        yield return null;
    }
    private void SpawnPortal(SpawnMobPortal spawnEnemy, Transform portalPos)
    {
        spawnEnemy.endRandomCountSpawn = Random.Range(spawnEnemy.minCountSpawn, spawnEnemy.maxCountSpawn + 1);
        GameObject portal = Instantiate(spawnMobsLogic.tech_portal_pref, portalPos);
        PortalLogic portLog = portal.GetComponent<PortalLogic>();
        portLog.SetDataPortal(spawnEnemy, SpawnMobsParent);
    }
}
[System.Serializable]
public class SpawnMobPortal
{
    public int minCountSpawn;
    public int maxCountSpawn;
    [System.NonSerialized] public int endRandomCountSpawn;
    public float timeDurationSpawn;
    public float coolDown;
    public bool startSpawn;

    public GameObject enemy_prefab;
    

    //public AllEnemy enemy_type;
    //private string key_name_enemy;

    //public string GetKey_name() => endRandomCountSpawn;
    //public void SetKey_name(int endCount) => endRandomCountSpawn = name;
}

