using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public static Clouds Instance;

    [SerializeField] private Sprite[] sprites_clouds;
    [SerializeField] private Transform cloudsParent;
    [SerializeField] private SpriteRenderer prefabRenderer;
    private Vector2 minMaxSpeed = new Vector2(0.3f, 0.7f); //0.7f, 1.5f

    private List<Transform> clouds = new List<Transform>();
    private Camera mainCam; // Кэшируем камеру

    private float heightOffset = 15f;
    private float weightOffset = 15f;

    private float camHeight;
    private float camWidth;

    private float halfWidth;
    private float heightLayer;
    private Vector3 finalScale;
    private float start_boost_speed;
    private int dir;
    private int initialCount = 20;//30
    private List<CloudObj> poolClouds;
    private List<CloudObj> activeClouds;

    private Coroutine createCoroutine;
    private Coroutine clearCoroutine;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    private void Start()
    {
        poolClouds = new List<CloudObj>();
        activeClouds = new List<CloudObj>();
        CreateAllClouds();
    }
    public void StartCloudsLogic()
    {
        mainCam = Camera.main; // Сохраняем ссылку

        camHeight = mainCam.orthographicSize * 2f;
        camWidth = camHeight * mainCam.aspect;

        heightLayer = (camHeight + (heightOffset * 2f)) / 5;
        finalScale = new Vector3(camWidth + (weightOffset * 2f), camHeight + (heightOffset * 2f), 1f);
        halfWidth = finalScale.x / 2f;

        dir = (Random.value > 0.5f) ? 1 : -1;
        start_boost_speed = 120f;

        clearCoroutine = StartCoroutine(CleareClouds());
        createCoroutine = StartCoroutine(CreateClouds());
        Invoke(nameof(SetDefaultBoost), 1f);
    }
    //public void Recreate()
    //{
    //    // 1. Останавливаем старые процессы
    //    if (createCoroutine != null) StopCoroutine(createCoroutine);
    //    if (clearCoroutine != null) StopCoroutine(clearCoroutine);

    //    CancelInvoke(nameof(SetDefaultBoost)); // Останавливаем отложенный буст
    //    start_boost_speed = 50f;

    //    RemoveAllObj();
    //    StartCloudsLogic();
    //}
    public void StopCloudsLogic()
    {
        // 1. Останавливаем старые процессы
        if (createCoroutine != null) StopCoroutine(createCoroutine);
        if (clearCoroutine != null) StopCoroutine(clearCoroutine);
        // Выключаем все активные облака и очищаем список активных
        for (int i = 0; i < activeClouds.Count; i++)
        {
            activeClouds[i].GameObject.SetActive(false);
        }
        activeClouds.Clear();
        gameObject.SetActive(false);
    }
    private void RemoveAllObj()
    {
        foreach (CloudObj cloudObj in poolClouds)
        {
            Destroy(cloudObj.GameObject);
        }
        poolClouds.Clear();
        activeClouds.Clear();
    }
    private void CreateAllClouds()
    {
        for(int i = 0; i < initialCount; i++)
        {
            AddCloudToPool();
        }
    }
    private CloudObj AddCloudToPool()
    {
        SpriteRenderer rend = Instantiate(prefabRenderer, cloudsParent);
        CloudObj newClouds = new CloudObj(rend, Random.Range(minMaxSpeed.x, minMaxSpeed.y));
        newClouds.GameObject.SetActive(false);
        poolClouds.Add(newClouds);
        initialCount = poolClouds.Count();
        return newClouds;
    }
    private void SpawnCloudsFromPool(Vector2 position)
    {
        CloudObj cloud = null;
        for (int i = 0; i < poolClouds.Count; i++)
        {
            if (!poolClouds[i].GameObject.activeSelf)
            {
                cloud = poolClouds[i];
                break;
            }
        }
        if(cloud == null)
        {
            cloud = AddCloudToPool();
        }
        cloud.Transform.position = position;
        cloud.SpriteRenderer.sprite = sprites_clouds[Random.Range(0, sprites_clouds.Length)];
        cloud.GameObject.SetActive(true);
        activeClouds.Add(cloud);
    }
    private void Update()
    {
        for (int i = 0;i < activeClouds.Count;i++)
        {
            activeClouds[i].Transform.position +=
                new Vector3(activeClouds[i].speed * dir * start_boost_speed * Time.deltaTime, 0, 0);
        }
    }
    private void SetDefaultBoost() => start_boost_speed = 1f;
    //IEnumerator CleareClouds()
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(1f);

    //        float camX = mainCam.transform.position.x;

    //        float leftBoundary = camX - halfWidth;
    //        float rightBoundary = camX + halfWidth;

    //        for (int i = clouds.Count - 1; i >= 0; i--)
    //        {
    //            if (clouds[i] == null)
    //            {
    //                clouds.RemoveAt(i);
    //                continue;
    //            }
    //            float cloudX = clouds[i].transform.position.x;

    //            if (cloudX < leftBoundary || cloudX > rightBoundary)
    //            {
    //                Transform cloudToDestroy = clouds[i];
    //                clouds.RemoveAt(i);
    //                Destroy(cloudToDestroy.gameObject);
    //            }
    //        }
    //    }
    //}
    IEnumerator CleareClouds()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);

            float camX = mainCam.transform.position.x;

            float leftBoundary = camX - halfWidth;
            float rightBoundary = camX + halfWidth;

            for (int i = activeClouds.Count - 1; i >= 0; i--)
            {
                if (activeClouds[i] == null)
                {
                    activeClouds.RemoveAt(i);
                    continue;
                }
                float cloudX = activeClouds[i].Transform.position.x;

                if (cloudX < leftBoundary || cloudX > rightBoundary)
                {
                    activeClouds[i].GameObject.SetActive(false);
                    activeClouds.RemoveAt(i);
                }
            }
        }
    }
    IEnumerator CreateClouds()
    {
        while(GlobalData.OnClouds && true)
        {
            yield return new WaitForSeconds(Random.Range(8f, 15f) / start_boost_speed);

            if (start_boost_speed > 1 && Random.value > 0.5f) continue;

            int layerArea = Random.Range(0, 5);
            Vector2 cameraPos = Camera.main.transform.position;

            float bootomEdge = cameraPos.y - (finalScale.y / 2f);
            float minY = bootomEdge + (layerArea * heightLayer);
            float maxY = minY + heightLayer;
            float coordY = Random.Range(minY, maxY);

            float coordX = cameraPos.x + (finalScale.x / 2f * -dir);
            Vector2 posCloud = new Vector2(coordX, coordY);
            SpawnCloudsFromPool(posCloud);
            //SpriteRenderer newCloudRend = Instantiate(prefabRenderer, posCloud, Quaternion.identity, cloudsParent);
            //newCloudRend.sprite = sprites_clouds[Random.Range(0, sprites_clouds.Length)];
            //clouds.Add(newCloudRend.transform);
        }
            
    }
}
class CloudObj
{
    public readonly GameObject GameObject;
    public readonly SpriteRenderer SpriteRenderer;
    public readonly Transform Transform;
    public readonly float speed;
    public CloudObj(SpriteRenderer spriteRenderer, float speed)
    {
        GameObject = spriteRenderer.gameObject;
        SpriteRenderer = spriteRenderer;
        Transform = spriteRenderer.transform;
        this.speed = speed;

        Color c = SpriteRenderer.color;
        // 60/255 = 0.23f, 200/255 = 0.78f
        c.a = Random.Range(0.25f, 0.70f);
        SpriteRenderer.color = c;
    }
}
