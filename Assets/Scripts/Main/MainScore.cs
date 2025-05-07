using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bestComboText;
    [SerializeField] TextMeshProUGUI bestScoreText;
    public void SetScore(int score, int bestScore, int bestCombo)
    {
        scoreText.text = score.ToString();
        bestScoreText.text = bestScore.ToString();
        bestComboText.text = bestCombo.ToString();
    }
}
