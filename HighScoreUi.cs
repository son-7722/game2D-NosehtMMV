using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void OnEnable()
    {
        LoadUI();
    }

    void LoadUI()
    {
        List<HighScoreManager.HighScoreData> scores =
            HighScoreManager.LoadScores();

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (i < scores.Count)
            {
                scoreTexts[i].text =
                    $"#{i + 1}  {scores[i].score}  |  {scores[i].time}";
            }
            else
            {
                scoreTexts[i].text = $"#{i + 1}  ---";
            }
        }
    }

    // ===== BUTTON =====
    public void ShowHighScore()
    {
        mainMenuPanel.SetActive(false);
        highScorePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        highScorePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
