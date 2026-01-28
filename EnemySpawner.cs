using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float baseSpawnTime = 2f;

    private Coroutine spawnCoroutine;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        StartSpawn();
    }

    private void OnDisable()
    {
        StopSpawn();
    }

    // ================= PUBLIC API =================

    public void StartSpawn()
    {
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnEnemyCoroutine());
    }

    public void StopSpawn()
{
    if (spawnCoroutine != null)
    {
        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }
}


    // ================= CORE =================

    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            if (Time.timeScale == 0f)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(GetSpawnInterval());

            if (enemies.Length == 0 || spawnPoints.Length == 0)
                continue;

            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(enemy, spawnPoint.position, Quaternion.identity);
        }
    }

    private float GetSpawnInterval()
    {
        if (gameManager == null) return baseSpawnTime;

        float difficulty = gameManager.GetDifficultyMultiplier();
        return Mathf.Clamp(baseSpawnTime / difficulty, 0.3f, baseSpawnTime);
    }
}
