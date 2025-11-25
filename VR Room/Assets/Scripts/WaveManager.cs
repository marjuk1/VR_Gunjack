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
    public float spawnRateMultiplier = 0.92f; // spawns 8% faster each wave
    public float enemyHealthIncrease = 10f;
    public float enemySpeedIncrease = 0.2f;

    private int currentWave = 0;
    private int enemiesToSpawn;
    private float spawnRate;
    public float EnemyDetectionRange = 5f;

    private int enemiesAlive = 0;
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
        enemiesToSpawn = startingEnemyCount;
        spawnRate = startingSpawnRate;
    }

    void Update()
    {
        if (!waveInProgress && enemiesAlive == 0)
            StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        waveInProgress = true;
        currentWave++;

        Debug.Log("Starting Wave " + currentWave);

        enemiesAlive = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnRate);
        }

        waveInProgress = false;

        // Apply difficulty scaling for next wave
        enemiesToSpawn += enemiesPerWaveIncrease;
        spawnRate *= spawnRateMultiplier;
        if (spawnRate < 0.2f) spawnRate = 0.2f; // set a limit so it's not insane
    }

    void SpawnEnemy()
    {
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);

        EnemyAIController ai = enemy.GetComponent<EnemyAIController>();
        if (ai != null)
        {
            ai.detectionRange = EnemyDetectionRange;
            ai.maxHealth += currentWave * enemyHealthIncrease;
            ai.agent.speed += currentWave * enemySpeedIncrease;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}
