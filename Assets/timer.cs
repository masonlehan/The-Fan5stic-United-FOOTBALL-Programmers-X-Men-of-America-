using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelTimer : MonoBehaviour
{
    public float timeRemaining = 60f;

    public TMP_Text timerText;
    public GameObject timesUpPanel;

    public Image restartButtonImage;
    public Image quitButtonImage;

    public Outline restartOutline;
    public Outline quitOutline;

    public Color restartColor = new Color(0f, 0.6f, 0f);
    public Color restartSelectedColor = new Color(0.5f, 1f, 0.5f);

    public Color quitColor = Color.red;
    public Color quitSelectedColor = new Color(1f, 0.5f, 0.5f);

    private bool timerRunning = true;
    private int selectedOption = 0;

    void Start()
    {
        Time.timeScale = 1f;
        timesUpPanel.SetActive(false);
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;

                timesUpPanel.SetActive(true);
                Time.timeScale = 0f;

                UpdateMenu();
            }

            UpdateTimerDisplay();
        }
        else
        {
            HandleMenuInput();
        }
    }

    void UpdateTimerDisplay()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
    }

    
    void HandleMenuInput()
    {
        bool moveUp = false;
        bool moveDown = false;
        bool select = false;

        if (Keyboard.current != null)
        {
            moveUp = Keyboard.current.upArrowKey.wasPressedThisFrame;
            moveDown = Keyboard.current.downArrowKey.wasPressedThisFrame;

            select = Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame;
        }

        if (Gamepad.current != null)
        {
            moveUp |= Gamepad.current.dpad.up.wasPressedThisFrame || Gamepad.current.leftStick.up.wasPressedThisFrame;
            
            moveDown |= Gamepad.current.dpad.down.wasPressedThisFrame || Gamepad.current.leftStick.down.wasPressedThisFrame;
            
            select |= Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame;
        }

        if (moveUp || moveDown)
        {
            selectedOption = 1 - selectedOption;
            UpdateMenu();
        }

        if (select)
        {
            if (selectedOption == 0)
            {
                RestartLevel();
            }
            else
            {
                QuitGame();
            }
        }
    }


    void UpdateMenu()
    {
        restartButtonImage.color = selectedOption == 0
            ? restartSelectedColor
            : restartColor;

        quitButtonImage.color = selectedOption == 1
            ? quitSelectedColor
            : quitColor;

        restartOutline.enabled = selectedOption == 0;
        quitOutline.enabled = selectedOption == 1;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Quitting Game!");

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
