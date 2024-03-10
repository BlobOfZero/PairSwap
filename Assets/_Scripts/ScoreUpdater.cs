using UnityEngine;
using TMPro;
using System.Text;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private StringBuilder scoreTextBuilder = new StringBuilder();

    public void UpdateScore(int score)
    {
        scoreTextBuilder.Clear();
        scoreTextBuilder.Append("Current score: ").Append(score);

        scoreText.text = scoreTextBuilder.ToString();
    }
}
