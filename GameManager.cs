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

    void Start()
    {
        currentEnergy = 0;
        UpdateEnergyBar();
        
        score = 0;
        UpdateScoreText();
        
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
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void AddEnergy()
    {
        if (bossCalled)
        {
            return;
        }
        currentEnergy += 1;
        UpdateEnergyBar();
        if(currentEnergy == energyThreshold)
        {
            CallBoss();
        }
    }
    private void CallBoss()
    {
        bossCalled = true;
        boss.SetActive(true);

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
