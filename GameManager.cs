using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;


public class GameManager : MonoBehaviour
{
    private int currentEnergy;
    [SerializeField] private int energyThreshold = 3;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject enemySpawner;
    private bool bossCalled = false;
    [SerializeField] private Image energyBar;
    [SerializeField] GameObject gameUi;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseGame;
    [SerializeField] private GameObject winGame;
    [SerializeField] private GameObject red;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private CinemachineCamera cam;
    private int usbNeedToPickup = 0;   // số USB cần nhặt để hoàn thành wave
    private bool waveReadyToSave = false;
    [SerializeField] private GameObject shopUI;


    [SerializeField] private GameObject mobileControls; // Reference to the Mobile Canvas/GameObject

    [Header("Wave Score")]
    [SerializeField] private TextMeshProUGUI waveScoreText;
    private int waveScore = 0;


    [Header("Score System")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;

    [Header("Wave System")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI usbText;
    private int wave = 1;
    private int usbCount = 0;
    private int bossesAlive = 0;
    private float difficultyMultiplier = 1.0f;
    [SerializeField] private Transform[] bossSpawnPoints;
    [SerializeField] private Button continueButton;

    void Start()
    {
        currentEnergy = 0;
        UpdateEnergyBar();

        boss.SetActive(false);
        MainMenu();

        audioManager.StopAudioGame();
        cam.Lens.OrthographicSize = 5f;
        red.SetActive(false);

        if (mobileControls != null)
            mobileControls.SetActive(false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(SaveSystem.HasSave());

        UpdateScoreText();
        UpdateWaveUI();
    }

    // ================= SAVE / LOAD =================

    public void SaveGame()
    {
        Player player = FindAnyObjectByType<Player>();
        Gun gun = FindAnyObjectByType<Gun>();

        SaveData data = new SaveData(
            wave,
            score,
            usbCount,
            difficultyMultiplier,

            player != null ? player.GetMaxHp() : 100f,
            player != null ? player.GetCurrentHp() : 100f,
            player != null ? player.GetBaseMoveSpeed() : 5f,

            gun != null ? gun.GetDamage() : 10f,
            gun != null ? gun.currenAmmo : 0
        );

        SaveSystem.SaveGame(data);
    }




    public void ContinueGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            StartGame();
            return;
        }

        wave = data.wave;
        score = data.score;
        usbCount = data.usbCount;

        waveScore = 0;
        UpdateWaveScoreUI();

        difficultyMultiplier = data.difficultyMultiplier;

        currentEnergy = 0;
        UpdateEnergyBar();
        UpdateScoreText();
        UpdateWaveUI();

        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            player.SetMaxHp(data.maxHp);
            player.SetHp(data.currentHp);
            player.SetMoveSpeed(data.moveSpeed);
        }

        Gun gun = FindAnyObjectByType<Gun>();
        if (gun != null)
        {
            gun.SetDamage(data.damage);
            gun.currenAmmo = data.ammo;
            gun.SendMessage("UpdateAmmoText", SendMessageOptions.DontRequireReceiver);
        }

        StartGame();
    }






    public void AddScore(int amount)
    {
        score += amount;        // tổng điểm toàn game
        waveScore += amount;   // điểm RIÊNG wave

        UpdateScoreText();
        UpdateWaveScoreUI();

        // ✅ Đạt điều kiện gọi boss bằng điểm wave
        if (!bossCalled && waveScore >= GetWaveScoreNeed())
        {
            CallBoss();
        }

        if (score >= 9999999)
        {
            WinGame();
        }
    }

    void UpdateWaveScoreUI()
    {
        if (waveScoreText != null)
        {
            waveScoreText.text =
                $"Wave Score: {waveScore} / {GetWaveScoreNeed()}";
        }
    }


    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }



    private void UpdateWaveUI()
    {
        if (waveText != null) waveText.text = "Wave: " + wave;
        if (usbText != null) usbText.text = "USB: " + usbCount;
    }

    public void AddUsb()
    {
        usbCount++;
        usbNeedToPickup--;

        UpdateWaveUI();

        // ✅ chỉ khi:
        // - boss đã chết hết
        // - đã nhặt đủ USB của wave
        if (waveReadyToSave && usbNeedToPickup <= 0)
        {
            waveReadyToSave = false;

            StartNextWave();   // sang wave mới
            SaveGame();        // ✅ SAVE DUY NHẤT 1 LẦN
            OpenShop();
        }
    }
    public int GetUsb()
    {
        return usbCount;
    }

    public bool SpendUsb(int amount)
    {
        if (usbCount < amount) return false;

        usbCount -= amount;
        UpdateWaveUI();
        return true;
    }
    void OpenShop()
    {
        Time.timeScale = 0f;
        shopUI.SetActive(true);
    }



    public void OnBossKilled()
    {
        bossesAlive--;

        if (bossesAlive <= 0)
        {
            waveReadyToSave = true;   // ✅ boss đã chết hết
        }
    }






    private void StartNextWave()
    {
        wave++;

        waveScore = 0; // ✅ RESET ĐIỂM WAVE
        UpdateWaveScoreUI();

        difficultyMultiplier += 0.1f;
        currentEnergy = 0;
        UpdateEnergyBar();
        UpdateWaveUI();

        bossCalled = false;
        red.SetActive(false);
        cam.Lens.OrthographicSize = 5f;

        gameUi.SetActive(true);
        enemySpawner.SetActive(true);
        audioManager.PlayDefaultAudio();

        Player player = FindAnyObjectByType<Player>();
        if (player != null)
            player.UpdateSpeed(GetSpeedMultiplier());
    }



    public float GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }

    public float GetSpeedMultiplier()
    {
        // 2% increase per wave (starting from wave 1)
        // Wave 1: 1.0
        // Wave 2: 1.02
        return 1.0f + ((wave - 1) * 0.02f);
    }

    public void AddEnergy()
    {
        if (bossCalled) return;

        currentEnergy++;
        UpdateEnergyBar();

        // ✅ Điều kiện 1: Đầy energy (wave cao thì cần nhiều hơn)
        if (currentEnergy >= GetEnergyThreshold())
        {
            CallBoss();
        }
    }

    int GetWaveScoreNeed()
    {
        return 120 + (wave - 1) * 30;
    }


    int GetEnergyThreshold()
    {
        return energyThreshold + wave;
    }


    private void CallBoss()
    {
        currentEnergy = 0;
        UpdateEnergyBar();

        if (bossCalled) return;
        bossCalled = true;
        ClearAllNormalEnemies();

        bossesAlive = (wave + 1) / 2;
        usbNeedToPickup = bossesAlive;
        waveReadyToSave = false;

        enemySpawner.SetActive(false);
        gameUi.SetActive(false);

        audioManager.PlayBossAudio();
        cam.Lens.OrthographicSize = 10f;
        red.SetActive(true);

        if (mobileControls != null)
            mobileControls.SetActive(true);

        // ===== SPAWN BOSS =====
        for (int i = 0; i < bossesAlive; i++)
        {
            Transform spawnPoint = bossSpawnPoints[
                i % bossSpawnPoints.Length   // tránh out of range
            ];

            Vector3 randomOffset = new Vector3(
                Random.Range(-1.5f, 1.5f),
                Random.Range(-1.5f, 1.5f),
                0
            );

            Instantiate(
                boss,
                spawnPoint.position + randomOffset,
                Quaternion.identity
            ).SetActive(true);
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(SaveSystem.HasSave());
    }

    private void ClearAllNormalEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
    public int GetCurrentWave()
    {
        return wave;
    }



    private void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            float fillAmount = Mathf.Clamp01(
                (float)currentEnergy / GetEnergyThreshold()
            );
            energyBar.fillAmount = fillAmount;
        }
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseGame.SetActive(false);
        winGame.SetActive(false);

        if (mobileControls != null) mobileControls.SetActive(false);

        Time.timeScale = 0f;
    }
    public void GameOverMenu()
    {
        SaveSystem.DeleteSave();
        gameOverMenu.SetActive(true);
        mainMenu.SetActive(false);
        pauseGame.SetActive(false);
        winGame.SetActive(false);

        if (mobileControls != null) mobileControls.SetActive(false);

        Time.timeScale = 0f;
    }
    public void PauseGameMenu()
    {
        pauseGame.SetActive(true);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winGame.SetActive(false);

        // Keep controls visible in Pause? Or hide? Usually hide to show Pause Menu.
        if (mobileControls != null) mobileControls.SetActive(false);

        Time.timeScale = 0f;
    }
    public void StartGame()
    {
        pauseGame.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winGame.SetActive(false);

        if (mobileControls != null) mobileControls.SetActive(true);

        Time.timeScale = 1f;
        audioManager.PlayDefaultAudio();
    }
    public void ResumeGame()
    {
        pauseGame.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winGame.SetActive(false);

        if (mobileControls != null) mobileControls.SetActive(true);

        Time.timeScale = 1f;
    }
    public void WinGame()
    {
        winGame.SetActive(true);
        mainMenu.SetActive(false);
        pauseGame.SetActive(false);
        gameOverMenu.SetActive(false);

        if (mobileControls != null) mobileControls.SetActive(false);

        Time.timeScale = 0f;
    }
}
