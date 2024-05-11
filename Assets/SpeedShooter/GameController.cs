using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI pointsText;
    public GameObject endGamePanel;
    public TextMeshProUGUI endGamePointsText;
    public TextMeshProUGUI endGameAccuracyText;
    public Button playAgainButton;

    private float startTime;
    private int totalShots = 0;
    private int hits = 0;
    private int points = 0;
    private float gameDuration = 60.0f; // Duration in seconds
    private bool gameIsActive = true;
    private float accuracy = 100.0f; // Start with 100% accuracy

    void Start()
    {
        startTime = Time.time;
        endGamePanel.SetActive(false);
        UpdateAccuracy(); // Initialize accuracy display
    }

    void Update()
    {
        if (gameIsActive)
        {
            UpdateTimer();
            UpdateAccuracy();
            UpdatePoints();
        }
    }

    void UpdateTimer()
    {
        float timeLeft = gameDuration - (Time.time - startTime);
        if (timeLeft > 0)
        {
            timerText.text = "Time: " + timeLeft.ToString("F1") + "s";
        }
        else
        {
            timerText.text = "Time: 0.0s";
            if (gameIsActive)  // Check if the game hasn't already been ended
            {
                EndGame();
            }
        }
    }

    void UpdateAccuracy()
    {
        accuracyText.text = "Accuracy: " + accuracy.ToString("F1") + "%";
    }

    void UpdatePoints()
    {
        pointsText.text = "Points: " + points;
    }

    public void RecordShot(bool isHit)
    {
        totalShots++;
        if (isHit)
        {
            hits++;
            points += 100; // Adjust scoring as needed
        }
        else
        {
            // Adjust accuracy based on misses
            if (totalShots > 0) // Avoid division by zero
                accuracy = 100f * hits / totalShots;
            UpdateAccuracy();
        }
    }

    void EndGame()
    {
        gameIsActive = false;
        endGamePanel.SetActive(true);
        endGamePointsText.text = "Points: " + points;
        endGameAccuracyText.text = "Accuracy: " + accuracy.ToString("F1") + "%";
        playAgainButton.onClick.AddListener(RestartGame);

        // Make cursor visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}