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

            int spawnCount = GetSpawnCount(); // 🔥 số enemy spawn theo wave

            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = enemies[Random.Range(0, enemies.Length)];
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Vector3 offset = new Vector3(
                    Random.Range(-1.2f, 1.2f),
                    Random.Range(-1.2f, 1.2f),
                    0
                );

                Instantiate(enemy, spawnPoint.position + offset, Quaternion.identity);
            }
        }
    }


    private float GetSpawnInterval()
{
    if (gameManager == null) return baseSpawnTime;

    int wave = gameManager.GetCurrentWave();

    // 🔥 giảm 0.05 mỗi wave
    float spawnTime = baseSpawnTime - (wave - 1) * 0.05f;

    // (tuỳ chọn) kết hợp difficulty
    spawnTime /= gameManager.GetDifficultyMultiplier();

    // ⛔ giới hạn tránh spawn quá nhanh
    return Mathf.Clamp(spawnTime, 0.5f, baseSpawnTime);
}
    int GetSpawnCount()
    {
        if (gameManager == null) return 1;
        int wave = gameManager.GetCurrentWave();
        return Mathf.Clamp((wave - 1) / 4 + 1, 1, 10);

    }
}
