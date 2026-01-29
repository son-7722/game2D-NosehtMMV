using UnityEngine;
using System;
using System.Collections.Generic;

public static class HighScoreManager
{
    private const int MAX_SCORE = 5;

    [Serializable]
    public struct HighScoreData
    {
        public int score;
        public string time; // lưu string cho đơn giản
    }

    // ================= SAVE =================
    public static void SaveScore(int newScore)
    {
        List<HighScoreData> scores = LoadScores();

        HighScoreData newData = new HighScoreData
        {
            score = newScore,
            time = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };

        scores.Add(newData);

        // sort giảm dần theo score
        scores.Sort((a, b) => b.score.CompareTo(a.score));

        // giữ top 5
        if (scores.Count > MAX_SCORE)
            scores.RemoveRange(MAX_SCORE, scores.Count - MAX_SCORE);

        // lưu lại
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt($"HS_Score_{i}", scores[i].score);
            PlayerPrefs.SetString($"HS_Time_{i}", scores[i].time);
        }

        PlayerPrefs.Save();
    }

    // ================= LOAD =================
    public static List<HighScoreData> LoadScores()
    {
        List<HighScoreData> scores = new List<HighScoreData>();

        for (int i = 0; i < MAX_SCORE; i++)
        {
            if (!PlayerPrefs.HasKey($"HS_Score_{i}")) break;

            HighScoreData data = new HighScoreData
            {
                score = PlayerPrefs.GetInt($"HS_Score_{i}", 0),
                time = PlayerPrefs.GetString($"HS_Time_{i}", "")
            };

            scores.Add(data);
        }

        return scores;
    }

    // (OPTIONAL) reset bảng điểm
    public static void ClearScores()
    {
        for (int i = 0; i < MAX_SCORE; i++)
        {
            PlayerPrefs.DeleteKey($"HS_Score_{i}");
            PlayerPrefs.DeleteKey($"HS_Time_{i}");
        }
        PlayerPrefs.Save();
    }
}
