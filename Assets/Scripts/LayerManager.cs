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
            
            //Debug.Log($"Obstacles {obstaclesLayer}");
            //Debug.Log($"Player {playerLayer}");
            //Debug.Log($"Player Man{playerManegerLayer}");
        }
    }
}
