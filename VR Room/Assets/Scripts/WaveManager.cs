using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    private Transform[] spawnPoints;

    public int startingEnemyCount = 5;
    public float startingSpawnRate = 1.2f;

    // Difficulty scaling
    public int enemiesPerWaveIncrease = 3;
    public float spawnRateMultiplier = 0.92f;
    public float enemyHealthIncrease = 10f;
    public float enemySpeedIncrease = 0.2f;

    private int currentWave = 0;
    private int enemiesToSpawn;
    private float spawnRate;
    public float EnemyDetectionRange = 5f;

    // Time between waves
    private float waveTimer = 0f;
    public float waveInterval = 30f;

    private bool waveInProgress = false;

    void Awake()
    {
        // Automatically get all child transforms EXCEPT the parent itself
        List<Transform> spawns = new List<Transform>();

        foreach (Transform child in transform)
        {
            spawns.Add(child);
        }

        spawnPoints = spawns.ToArray();


    }
    void Start()
    {
        waveTimer = waveInterval;
        enemiesToSpawn = startingEnemyCount;
        spawnRate = startingSpawnRate;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive())
            return;

        waveTimer += Time.deltaTime;

        if (!waveInProgress && waveTimer >= waveInterval)
        {
            StartCoroutine(StartWave());
            waveTimer = 0f;
        }
    }

    IEnumerator StartWave()
    {
        waveInProgress = true;
        currentWave++;

        Debug.Log("Starting Wave " + currentWave);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (!GameManager.Instance.IsGameActive()) yield break;
            SpawnEnemy();
            yield return new WaitForSeconds(spawnRate);
        }

        waveInProgress = false;

        // Apply difficulty scaling for next wave
        enemiesToSpawn += enemiesPerWaveIncrease;
        spawnRate *= spawnRateMultiplier;
        spawnRate = Mathf.Clamp(spawnRate, 0.2f, 100f);
    }

    [System.Obsolete]
    void SpawnEnemy()
    {
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);
        enemy.SetActive(true);

        EnemyAIController ai = enemy.GetComponent<EnemyAIController>();
        if (ai != null)
        {
            ai.detectionRange = EnemyDetectionRange;
            ai.maxHealth += currentWave * enemyHealthIncrease;
            ai.agent.speed += currentWave * enemySpeedIncrease;
        }
    }
}
