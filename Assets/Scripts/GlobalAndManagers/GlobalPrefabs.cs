using System.Collections.Generic;
using UnityEngine;

public class GlobalPrefabs : MonoBehaviour 
{
    public static GlobalPrefabs Instance { get; private set; }

    public GameObject ItemDropPref;
    public Transform itemsDropParent;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }
}
