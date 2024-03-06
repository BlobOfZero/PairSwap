using System.Collections;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;

    public void StopGame(int score)
    {
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();
        
    }

    public void RestartGame()
    {

    }
}
