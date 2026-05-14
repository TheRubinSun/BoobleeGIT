using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Transform parentPlayerProj;
    [SerializeField] private Transform parentPlayerLazer;
    [SerializeField] private Transform parentEnemyProj;
    [SerializeField] private Transform parentEnemyLazer;

    public static ProjectilePool instance;

    private Dictionary<string, List<IPoolData>> pool = new Dictionary<string, List<IPoolData>>();
    private Dictionary<string, List<IPoolData>> lazer_pool = new Dictionary<string, List<IPoolData>>();
    private Dictionary<string, List<IPoolData>> enemy_pool = new Dictionary<string, List<IPoolData>>();
    private Dictionary<string, List<IPoolData>> enemy_lazer_pool = new Dictionary<string, List<IPoolData>>();
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public Projectile_Logic GetPlayerProjectile(GameObject prefab, out GameObject newProj, out Rigidbody2D rb2)
    {
        return GetProjectile(pool, parentPlayerProj, prefab, out newProj, out rb2);
    }
    public Projectile_Logic GetEnemyProjectile(GameObject prefab, out GameObject newProj, out Rigidbody2D rb2)
    {
        return GetProjectile(enemy_pool, parentEnemyProj, prefab, out newProj, out rb2);
    }
    public LazerControl GetPlayerLazer(GameObject prefab, out GameObject newProj)
    {
        return GetLazer(lazer_pool, parentPlayerLazer, prefab, out newProj);
    }
    public LazerControl GetEnemyLazer(GameObject prefab, out GameObject newProj)
    {
        return GetLazer(enemy_lazer_pool, parentEnemyLazer, prefab, out newProj);
    }
    public Projectile_Logic GetProjectile(Dictionary<string, List<IPoolData>> use_pool, Transform path, GameObject prefab, out GameObject newProj, out Rigidbody2D rb2)
    {
        string nameType = prefab.name;
        if (!use_pool.ContainsKey(nameType))
            use_pool[nameType] = new List<IPoolData>();

        List<IPoolData> list = use_pool[nameType];

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].Obj.activeInHierarchy)
            {
                newProj = list[i].Obj;
                rb2 = ((ProjectilePoolData)list[i]).Rb2;
                return ((ProjectilePoolData)list[i]).Logic; 
            }
        }

        //Если нет, то создаем новый
        newProj = Instantiate(prefab, path);
        newProj.SetActive(false);
        rb2 = newProj.GetComponent<Rigidbody2D>();

        ProjectilePoolData newData = new ProjectilePoolData(newProj, newProj.GetComponent<Projectile_Logic>(), rb2);
        list.Add(newData);

        return newData.Logic;
    }
    public LazerControl GetLazer(Dictionary<string, List<IPoolData>> use_pool, Transform path, GameObject prefab, out GameObject newProj)
    {
        string nameType = prefab.name;
        if (!use_pool.ContainsKey(nameType))
            use_pool[nameType] = new List<IPoolData>();

        List<IPoolData> list = use_pool[nameType];

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].Obj.activeInHierarchy)
            {
                newProj = list[i].Obj;
                return ((LaserPoolData)list[i]).Logic;
            }
        }

        //Если нет, то создаем новый
        newProj = Instantiate(prefab, path);
        newProj.SetActive(false);

        LaserPoolData newData = new LaserPoolData(newProj, newProj.GetComponent<LazerControl>());
        list.Add(newData);

        return newData.Logic;
    }
    //public T GetProjectile<T>(GameObject prefab, out GameObject newProj, out Rigidbody2D rb2) where T : Component
    //{
    //    string nameType = prefab.name;
    //    if (!pool.ContainsKey(nameType))
    //    {
    //        pool[nameType] = new List<IPoolData>();
    //    }
    //    List<IPoolData> list = pool[nameType];

    //    for(int i = 0; i < list.Count; i++)
    //    {
    //        if (!list[i].Obj.activeInHierarchy)
    //        {
    //            rb2 = ((PoolData<T>)list[i]).Rigidbody;
    //            newProj = ((PoolData<T>)list[i]).Obj;
    //            return ((PoolData<T>)list[i]).Component;
    //        }
    //    }

    //    //Если нет, то создаем новый
    //    newProj = Instantiate(prefab, parentProj);
    //    newProj.SetActive(false);

    //    T component = newProj.GetComponent<T>();
    //    rb2 = new Rigidbody2D();
    //    PoolData<T> newData = new PoolData<T>(newProj, component, rb2);
    //    list.Add(newData);

    //    return component;
    //    //foreach(PLData proj in pool[nameType])
    //    //{
    //    //    if(!proj.Obj.activeInHierarchy)
    //    //        return proj;
    //    //}
    //    //return CreateNewProj(pool[nameType], prefab, typeProj);
    //}
    //public PLData CreateNewProj(List<PLData> projectilesType, GameObject prefab, TypeProj typeProj)
    //{
    //    GameObject newProj = Instantiate(prefab, parentProj);
    //    newProj.SetActive(false);
    //    PLData newProjData;
    //    switch (typeProj)
    //    {
    //        case TypeProj.Projectile:
    //            { newProjData = new ProjectileData(newProj, newProj.GetComponent<Projectile_Logic>()); break; }
    //        case TypeProj.Lazer:
    //            { newProjData = new LazersData(newProj, newProj.GetComponent<LazerControl>()); break; }
    //        default: goto case TypeProj.Lazer;
    //    }    

    //    projectilesType.Add(newProjData);

    //    //Debug.Log($"Создаем новую секцию | Name: {prefab.name} | Count: {projectilesType.Count}");
    //    return newProjData;
    //}
}
public interface IPoolData
{
    public GameObject Obj { get; }
}
// Данные для физических снарядов (Хранят и логику, и Rigidbody2D)
public class ProjectilePoolData : IPoolData
{
    public GameObject Obj { get; private set; }
    public Projectile_Logic Logic { get; private set; }
    public Rigidbody2D Rb2 { get; private set; } // Кешируем физику тут!

    public ProjectilePoolData(GameObject obj, Projectile_Logic logic, Rigidbody2D rb2)
    {
        Obj = obj;
        Logic = logic;
        Rb2 = rb2;
    }
}

// Данные для лазеров (Никакой физики, только логика лазера)
public class LaserPoolData : IPoolData
{
    public GameObject Obj { get; private set; }
    public LazerControl Logic { get; private set; }

    public LaserPoolData(GameObject obj, LazerControl logic)
    {
        Obj = obj;
        Logic = logic;
    }
}
//public class PoolData<T> : IPoolData where T : Component
//{
//    public GameObject Obj { get; private set; }
//    public T Component { get; private set; }
//    public Rigidbody2D Rigidbody { get; private set; }
//    public PoolData(GameObject obj, T component, Rigidbody2D rigidbody = null)
//    {
//        Obj = obj;
//        Component = component;
//        Rigidbody = rigidbody;
//    }
//    public PoolData(GameObject obj, T component)
//    {
//        Obj = obj;
//        Component = component;
//    }
//}


