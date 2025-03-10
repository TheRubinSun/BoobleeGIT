using UnityEngine;

public class LayerManager : MonoBehaviour
{
    // —татические переменные, которые будут сохран€ть значени€ слоев дл€ всех мобов
    public static int obstaclesLayer = -1;
    public static int playerLayer = -1;
    public static int enemyLayer = -1;

    private void Awake()
    {
        if (obstaclesLayer == -1) // ѕровер€ем, чтобы не переинициализировать слои
        {
            obstaclesLayer = LayerMask.NameToLayer("Obstacles");
            playerLayer = LayerMask.NameToLayer("Player");
            enemyLayer = LayerMask.NameToLayer("Enemy");

            Debug.Log($"Obstacles {obstaclesLayer}");
            Debug.Log($"Player {playerLayer}");
            Debug.Log($"Enemy {enemyLayer}");
        }
    }
}
