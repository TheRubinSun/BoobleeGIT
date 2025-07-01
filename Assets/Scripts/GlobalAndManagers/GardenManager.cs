using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public interface IPointFarm
{
    public int ID { get; set; }
}
public class GardenManager : MonoBehaviour
{
    public static GardenManager instance;

    [SerializeField] private Transform parentGarden;
    [SerializeField] private Flower[] flowers;

    private List<GameObject> flowersSpawn = new();

    Dictionary<int, FarmPoint> FarmsPoints_local;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if(GlobalWorld.FarmsPoints.Count > 0 && flowers.Length > 0)
        {
            FarmsPoints_local = GlobalWorld.FarmsPoints;
            foreach(KeyValuePair<int, FarmPoint> point in FarmsPoints_local)
            {
                GameObject GetGaObj;
                string key = point.Value.seed_type;
                switch(point.Value.stage_ground)
                {
                    case 1:
                        GetGaObj = GetSeedForKey(key);
                        break;
                    case 2:
                        GetGaObj = GetSproutForKey(key);
                        break;
                    case 3:
                        GetGaObj = GetFinalForKey(key);
                        break;
                    default:
                        goto case 1;
                }
                GameObject flower = Instantiate(GetGaObj, parentGarden);
                flower.transform.position = point.Value.GetPos();
                flower.tag = "Planted";
                flower.GetComponent<IPointFarm>().ID = point.Key;
                flowersSpawn.Add(flower);
            }
        }
        else
        {
            //Debug.LogError("Garden Error - empty ¹400");
        }
    }
    public bool PlantSeed(GameObject pref, string flower_type)
    {
        GameObject flower = Instantiate(pref, parentGarden);
        flower.tag = "Planted";
        Vector2 plantFlower = Player.Instance.GetPosPlayer();
        flower.transform.position = plantFlower;
        flowersSpawn.Add(flower);

        int id = GlobalWorld.AddFarmPoint(new FarmPoint(plantFlower, flower_type, 1));
        flower.GetComponent<IPointFarm>().ID = id;
        Debug.Log($"add Flower in {id}");

        return true;
    }
    public GameObject GetSeedForKey(string key) => flowers.FirstOrDefault(f => f.key_name == key)?.flower_seed;
    public GameObject GetSproutForKey(string key) => flowers.FirstOrDefault(f => f.key_name == key)?.flower_sprout;
    public GameObject GetFinalForKey(string key)
    {
        Flower flower = flowers.FirstOrDefault(f => f.key_name == key);
        return flower.flower_finals[UnityEngine.Random.Range(0, flower.flower_finals.Length)];
    }

}
[Serializable]
public class Flower
{
    public int id;
    public string key_name;
    public GameObject[] flower_finals;
    public GameObject flower_seed;
    public GameObject flower_sprout;

}

