using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameActive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayerDied()
    {
        gameActive = false;
        Debug.Log("Game Over — stopping gameplay");
    }

    public void PlayerWon()
    {
        gameActive = false;

        // Kill all enemies
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        foreach (var enemy in enemies)
        {
            enemy.OnGameEnded();
        }

        Debug.Log("Player won - enemies cleared");
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}