using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // Required for the restart delay timer

public class BusTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeRemaining = 60f; // Starting countdown time in seconds
    private bool isTimerRunning = true;

    [Header("UI Reference")]
    public TextMeshProUGUI timerText;

    void Start()
    {
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateDisplay();
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                StartCoroutine(GameOverAndRestart());
            }
        }
    }

    void UpdateDisplay()
    {
        if (timerText != null)
        {
            // Rounds the decimal time remaining to a clean whole number
            int roundedTime = Mathf.CeilToInt(timeRemaining);
            
            // Displays your required starting text next to the changing number
            timerText.text = "Catch the Bus! - " + roundedTime;
        }
    }

    // This handles changing the text message and waiting before restarting
    System.Collections.IEnumerator GameOverAndRestart()
    {
        if (timerText != null)
        {
            // Changes the screen text instantly at zero
            timerText.text = "You Missed the Bus!";
            timerText.color = Color.red;
        
        }

        Debug.Log("Missed the bus! Waiting 2 seconds to restart...");
        
        // Pauses the execution right here for 2 seconds so the player can read the text
        yield return new WaitForSeconds(2f);

        // Reloads your current scene automatically
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
