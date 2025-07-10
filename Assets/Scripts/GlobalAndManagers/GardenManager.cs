using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public interface IPointFarm
{
    public int ID { get; set; }
    public int IdDirtBed {  get; set; }
}
public class GardenManager : MonoBehaviour
{
    public static GardenManager instance;

    [SerializeField] private Transform parentGarden;
    [SerializeField] private Flower[] flowers;
    [SerializeField] private Transform parentDirtBedsOBJ;
    private List<DirtBed> dirtBeds = new();

    private List<GameObject> flowersSpawn = new();

    Dictionary<int, FarmPoint> FarmsPoints_local;
    private void Awake()
    {
        instance = this;
    }
   
    private void Start()
    {
        int id = 0;
        foreach(Transform child in parentDirtBedsOBJ)
        {
            DirtBed dirtbed = child.GetComponent<DirtBed>();
            dirtbed.ID = id;
            dirtBeds.Add(dirtbed);
            id++;
        }
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

                if(dirtBeds[point.Value.IdDirtBed] != null) 
                    flower.transform.position = (new Vector2(0, -0.15f) + (Vector2)dirtBeds[point.Value.IdDirtBed].transform.position);
                else
                    flower.transform.position = point.Value.GetPos();

                flower.tag = "Planted";
                IPointFarm flowerData = flower.GetComponent<IPointFarm>();
                flowerData.ID = point.Key;
                flowerData.IdDirtBed = point.Value.IdDirtBed;
                flowersSpawn.Add(flower);

                dirtBeds[point.Value.IdDirtBed].Busy = true;
            }
        }
        else
        {
            //Debug.LogError("Garden Error - empty ¹400");
        }

    }
    public bool PlantSeed(GameObject pref, string flower_type, int idDirt, Vector2 pos)
    {
        GameObject flower = Instantiate(pref, parentGarden);
        flower.tag = "Planted";
        Vector2 plantFlower = (new Vector2(0, -0.15f) + pos);
        flower.transform.position = plantFlower;
        flowersSpawn.Add(flower);

        int id = GlobalWorld.AddFarmPoint(new FarmPoint(idDirt, plantFlower, flower_type, 1));

        IPointFarm flowerData = flower.GetComponent<IPointFarm>();
        flowerData.ID = id;
        flowerData.IdDirtBed = idDirt;

        return true;
    }
    public void RemoveSeed(int idDirtBed)
    {
        Debug.Log($"Remove seed idDirtBed: {idDirtBed}  {dirtBeds[idDirtBed].name}");

        dirtBeds[idDirtBed].Busy = false;
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

