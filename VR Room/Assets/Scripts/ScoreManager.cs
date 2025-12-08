using UnityEngine;
using TMPro;
using Unity.XR.CoreUtils;
using System.Collections; // Required for Coroutines

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Scoring")]
    public int currentScore = 0;
    public int winScore = 1000;
    public int pointsPerKill = 100;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject winScreen;
    public TextMeshProUGUI finalScoreText;

    [Header("World Space UI Settings")]
    public Transform playerCamera; // Reference to the player's head/camera
    public float spawnDistance = 2f; // How far in front of the player to spawn the UI

    [Header("Win Effects")]
    public float dimDuration = 1.5f; // How long it takes for the world to go dark
    private float initialAmbientIntensity;
    private bool hasWon = false;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        initialAmbientIntensity = RenderSettings.ambientIntensity;
    }

    void Start()
    {
        if (winScreen != null)
            winScreen.SetActive(false); // Hide win screen at the start
        UpdateScoreUI();

        // Auto-find the player camera if it's not assigned
        if (playerCamera == null)
        {
            var rig = FindObjectOfType<XROrigin>();
            if (rig != null)
            {
                playerCamera = rig.Camera.transform;
            }
        }
    }

    public void AddScoreOnKill()
    {
        if (hasWon) return; // Stop adding score after winning

        currentScore += pointsPerKill;
        UpdateScoreUI();

        if (currentScore >= winScore)
        {
            hasWon = true;
            TriggerWinCondition();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    private void TriggerWinCondition()
    {
        StartCoroutine(FadeWorldToBlack(dimDuration));

        if (winScreen != null)
        {
            // Position the world-space UI in front of the player
            if (playerCamera != null)
            {
                // Set position in front of the camera, at the same height
                Vector3 spawnPosition = playerCamera.position + new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized * spawnDistance;
                winScreen.transform.position = spawnPosition;

                // Make the UI look at the player (but only on the Y-axis)
                Vector3 lookAtPosition = new Vector3(playerCamera.position.x, winScreen.transform.position.y, playerCamera.position.z);
                winScreen.transform.LookAt(lookAtPosition);
            }

            winScreen.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {currentScore}";
            }
        }
        Debug.Log("You Won!");
    }

    private IEnumerator FadeWorldToBlack(float duration)
    {
        float startIntensity = RenderSettings.ambientIntensity;
        float time = 0;

        while (time < duration)
        {
            // Lerp the ambient intensity down to 0
            RenderSettings.ambientIntensity = Mathf.Lerp(startIntensity, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        RenderSettings.ambientIntensity = 0f;
    }

    // Call this if you need to reset the lighting, e.g., from WinScreenController
    public void ResetLighting()
    {
        RenderSettings.ambientIntensity = initialAmbientIntensity;
    }
}