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
        Debug.Log("Game Won — stopping gameplay");
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}