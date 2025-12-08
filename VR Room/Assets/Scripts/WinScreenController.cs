using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenController : MonoBehaviour
{
    /// <summary>
    /// Reloads the current scene to restart the game.
    /// </summary>
    public void Continue()
    {
        // Un-pause the game before reloading
        Time.timeScale = 1f;
        // Get the current scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        // This stops the editor play mode
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}