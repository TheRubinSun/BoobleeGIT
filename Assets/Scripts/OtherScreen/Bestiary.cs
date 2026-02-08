using UnityEngine;

public class Bestiary : MonoBehaviour
{
    public static Bestiary Instance;
    [SerializeField] private Transform mobsParent;
    [SerializeField] private GameObject mobBut;
    [SerializeField] private GameObject mobPrefab;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    void Start()
    {
        foreach(Mob mob in EnemyList.mobs)
        {
            if(mob != null)
            {

            }
        }
    }

}
