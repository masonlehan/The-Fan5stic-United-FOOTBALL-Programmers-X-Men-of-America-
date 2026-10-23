using UnityEngine;
using TMPro; // Allows us to change our countdown text to a win message

public class WinZone : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI timerText; // Link this to your GamerTimerText

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that stepped into the finish line is tagged "Player"
        if (other.CompareTag("Player"))
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("Player reached the bus! You Win!");

        if (timerText != null)
        {
            // Update your UI text instantly to show victory
            timerText.text = "You Caught the Bus! You Win!";
            timerText.color = Color.green; // Turn text green for victory
        }

        // 1. Stop the countdown timer by turning off its script component
        BusTimer timerScript = timerText.GetComponent<BusTimer>();
        if (timerScript != null)
        {
            timerScript.enabled = false;
        }

        // 2. Freeze the player's physics so they stop moving after winning
        Rigidbody2D playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.bodyType = RigidbodyType2D.Static;
        }
    }
}
