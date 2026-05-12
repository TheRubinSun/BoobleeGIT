using UnityEngine;

public class LayerManager : MonoBehaviour
{
    // —татические переменные, которые будут сохран€ть значени€ слоев дл€ всех мобов
    public static int obstaclesLayer = -1;
    public static int playerLayer = -1;
    public static int playerManagerLayer = -1;
    public static int enemyLayer = -1;
    public static int touchObjectsLayer = -1;
    public static int interactableLayer = -1;
    public static int touchTriggObjLayer = -1;
    public static int enemyObject = -1;
    public static int interactableTriggerLayer = -1;

    public static LayerMask enemyAll;
    public static LayerMask allToughTrigger;
    //public static LayerMask allBreakObj;

    public static int allTrigger;
    public static int allTriggerObject;
    private void Awake()
    {
        if (obstaclesLayer == -1) // ѕровер€ем, чтобы не переинициализировать слои
        {
            obstaclesLayer = LayerMask.NameToLayer("Obstacles");
            playerLayer = LayerMask.NameToLayer("Player");
            playerManagerLayer = LayerMask.NameToLayer("PlayerManager");
            enemyLayer = LayerMask.NameToLayer("Enemy");
            touchObjectsLayer = LayerMask.NameToLayer("TouchObjects");
            interactableLayer = LayerMask.NameToLayer("Interactable");
            touchTriggObjLayer = LayerMask.NameToLayer("TouchTriggObj");
            enemyObject = LayerMask.NameToLayer("EnemyObject");
            interactableTriggerLayer = LayerMask.NameToLayer("InteractableTrigger");

            enemyAll = (1 << enemyLayer) | (1 << enemyObject);
            allToughTrigger = (1 << obstaclesLayer) | (1 << touchObjectsLayer) | (1 << touchTriggObjLayer);
            allTrigger = (1 << enemyObject) | (1 << touchObjectsLayer) | (1 << touchTriggObjLayer);
            allTriggerObject = (1 << touchObjectsLayer) | (1 << touchTriggObjLayer);
            //allBreakObj = (1 << touchObjectsLayer) | (1 << enemyObject) | (1 << touchTriggObjLayer);
        }
    }
}
