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
    [SerializeField] private GameObject mobileControls; // Reference to the Mobile Canvas/GameObject
    
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
    [SerializeField] private Transform bossSpawnPoint; // Where to spawn bosses

    void Start()
    {
        currentEnergy = 0;
        UpdateEnergyBar();
        
        score = 0;
        UpdateScoreText();
        UpdateWaveUI(); // Init UI

        boss.SetActive(false);
        MainMenu();
        audioManager.StopAudioGame();
        cam.Lens.OrthographicSize = 5f ;
        red.SetActive(false);
        
        // Ensure Mobile Controls are OFF at start
        if(mobileControls != null) mobileControls.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        
        if (score >= 9999999)
        {
            WinGame();
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
        UpdateWaveUI();
    }

    public void OnBossKilled()
    {
        bossesAlive--;
        if (bossesAlive <= 0)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        wave++;
        difficultyMultiplier += 0.1f;
        currentEnergy = 0;
        UpdateEnergyBar();
        UpdateWaveUI();
        
        bossCalled = false;
        red.SetActive(false);
        cam.Lens.OrthographicSize = 5f;
        
        gameUi.SetActive(true); // Re-enable UI (Energy Bar, etc)
        
        // Restart Spawning
        enemySpawner.SetActive(true);
        audioManager.PlayDefaultAudio();
        
        // Update Player Speed
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            player.UpdateSpeed(GetSpeedMultiplier());
        }
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
        if (bossCalled)
        {
            return;
        }
        currentEnergy += 1;
        UpdateEnergyBar();
        if(currentEnergy >= energyThreshold) // Changed to >= just in case
        {
            CallBoss();
        }
    }
    private void CallBoss()
    {
        bossCalled = true;
        
        // Spawn Bosses based on Wave
        bossesAlive = wave; // 1 boss in wave 1, 2 in wave 2...
        
        // We need to instantiate bosses because we need multiple.
        // Assuming 'boss' is a Prefab reference not scene object, OR we use the scene object as a template.
        // Since 'boss' was set to inactive, let's use it as a template if possible, or just instantiate it.
        // But if 'boss' is a scene object (it was dragging in Inspector), Instantiate(boss) works (clones it).
        
        Vector3 spawnPos = (bossSpawnPoint != null) ? bossSpawnPoint.position : boss.transform.position;
        
        for (int i = 0; i < bossesAlive; i++)
        {
            // Offset slightly to avoid overlap
            Vector3 offset = new Vector3(i * 2, 0, 0); 
            GameObject newBoss = Instantiate(boss, spawnPos + offset, Quaternion.identity);
            newBoss.SetActive(true);
        }

        // Original boss object stays inactive/hidden (it serves as prefab/template now)

        enemySpawner.SetActive(false);
        gameUi.SetActive(false);
        audioManager.PlayBossAudio();
        cam.Lens.OrthographicSize = 10f;
        red.SetActive(true);
    }
    private void UpdateEnergyBar()
    {
        if(energyBar != null)
        {
            float fillAmount = Mathf.Clamp01((float)currentEnergy / (float)energyThreshold);
            energyBar.fillAmount = fillAmount;
        }
    }
    public void MainMenu()
    {
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseGame.SetActive(false);
        winGame.SetActive(false);
        
        if(mobileControls != null) mobileControls.SetActive(false);
        
        Time.timeScale = 0f;
    }
    public void GameOverMenu()
    {
        gameOverMenu.SetActive(true);
        mainMenu.SetActive(false);
        pauseGame.SetActive(false);
        winGame.SetActive(false);
        
        if(mobileControls != null) mobileControls.SetActive(false);
        
        Time.timeScale = 0f;
    }
    public void PauseGameMenu()
    {
        pauseGame.SetActive(true);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winGame.SetActive(false);
        
        // Keep controls visible in Pause? Or hide? Usually hide to show Pause Menu.
        if(mobileControls != null) mobileControls.SetActive(false);
        
        Time.timeScale = 0f;
    }
    public void StartGame()
    {
        pauseGame.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winGame.SetActive(false);
        
        if(mobileControls != null) mobileControls.SetActive(true);
        
        Time.timeScale = 1f;
        audioManager.PlayDefaultAudio();
    }
    public void ResumeGame()
    {
        pauseGame.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winGame.SetActive(false);
        
        if(mobileControls != null) mobileControls.SetActive(true);
        
        Time.timeScale = 1f;
    }
    public void WinGame()
    {
        winGame.SetActive(true);
        mainMenu.SetActive(false);
        pauseGame.SetActive(false);
        gameOverMenu.SetActive(false);
        
        if(mobileControls != null) mobileControls.SetActive(false);
        
        Time.timeScale = 0f;
    }
}
