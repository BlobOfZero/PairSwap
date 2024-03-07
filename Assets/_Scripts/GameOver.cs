using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;

    public void StopGame(int score)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
