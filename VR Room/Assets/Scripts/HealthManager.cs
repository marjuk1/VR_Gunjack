using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit; // Required for XR components

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("UI")]
    public Slider healthBar;
    public TextMeshProUGUI healthtext;

    [Header("Game Over UI")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI finalScoreText;

    [Header("Interaction Settings on Death")]
    public InteractionLayerMask uiLayerMask; // Assign the "UI" layer in the Inspector
    public XRInteractionManager interactionManager; // Assign the scene's Interaction Manager

    private bool isPlayerDead = false; // Flag to prevent Die() from running multiple times

    [Header("Player Hurt Sounds")]
    public AudioSource hurtAudioSource;
    public AudioClip[] hurtClips;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            if(healthtext != null)
            {
                healthtext.text = setHealthText();
            }
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
    public void PlayHurtSound()
    {
        if (hurtClips.Length == 0 || hurtAudioSource == null)
            return;

        hurtAudioSource.PlayOneShot(hurtClips[Random.Range(0, hurtClips.Length)]);
    }
    private string setHealthText()
    {
        return this.currentHealth.ToString() + " / " + this.maxHealth.ToString();
    }
    public void TakeDamage(float damage)
    {
        if (isPlayerDead) return; // Don't take damage if already dead

        Debug.Log("PLAYER TOOK DAMAGE");
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();
        if(currentHealth <= 0) 
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
        if (healthtext != null)
            healthtext.text = setHealthText();
    }
    private void Die()
    {
        if (isPlayerDead) return; // Ensure this only runs once
        isPlayerDead = true;

        GameManager.Instance.PlayerDied();

        Debug.Log("Player has died. Game Over.");

        // --- Restrict Player Actions to UI Only ---

        // Find the Interaction Manager if it wasn't assigned in the Inspector
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        if (interactionManager != null)
        {
            XRBaseInteractor[] interactors = FindObjectsOfType<XRBaseInteractor>();
            foreach (var interactor in interactors)
            {
                // 1. Force the player to drop any held items
                if (interactor.hasSelection)
                {
                    interactionManager.CancelInteractorSelection(interactor);
                }

                // 2. Change the interactor's mask to ONLY interact with the UI layer
                interactor.interactionLayers = uiLayerMask;
            }
            Debug.Log("Player interactors restricted to UI layer.");
        }
        else
        {
            Debug.LogError("XR Interaction Manager not found. Cannot restrict player actions.");
        }


        // --- Destroy All Enemies ---
        EnemyAIController[] allEnemies = FindObjectsOfType<EnemyAIController>();
        foreach (EnemyAIController enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }
        Debug.Log($"Destroyed {allEnemies.Length} remaining enemies.");

        // --- Show Game Over Screen ---
        if (gameOverScreen != null)
        {
            // Activate the game over screen
            gameOverScreen.SetActive(true);

            // Display the final score
            if (finalScoreText != null && ScoreManager.Instance != null)
            {
                finalScoreText.text = $"Score: {ScoreManager.Instance.currentScore}";
            }

            // Position the UI in front of the player
            if (ScoreManager.Instance != null && ScoreManager.Instance.playerCamera != null)
            {
                Transform playerCamera = ScoreManager.Instance.playerCamera;
                float spawnDistance = ScoreManager.Instance.spawnDistance;
                
                Vector3 spawnPosition = playerCamera.position + new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized * spawnDistance;
                gameOverScreen.transform.position = spawnPosition;

                Vector3 lookAtPosition = new Vector3(playerCamera.position.x, gameOverScreen.transform.position.y, playerCamera.position.z);
                gameOverScreen.transform.LookAt(lookAtPosition);
            }
        }
    }
}
