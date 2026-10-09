using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseManagerScript : MonoBehaviour
{
    public GameObject PauseCanvas;

    public GameObject ResumeButton;
    public GameObject QuitButton;

    Button resumeButtonComponent;
    Button quitButtonComponent;

    void Start()
    {
        // We cannot use GameObject.Find() here because the Canvas 
        //  (and its Button children) start inactive.
        // None of them can be found!
        //
        // By using a public property, we can pre-assign the references in the Inspector.
        // Next, we get the Button components from the assigned GameObjects.
        resumeButtonComponent = ResumeButton.GetComponent<Button>();
        quitButtonComponent = QuitButton.GetComponent<Button>();

        resumeButtonComponent.onClick.AddListener(ResumeGame);
        quitButtonComponent.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        // This checks if the Escape key was pressed this frame.
        //
        // This is not an ideal way to do this, but it works for simple cases.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Check if the Canvas is "active." (Starts as inactive!)
            if (PauseCanvas.activeSelf == false)
            {
                // If the Canvas is hidden (inactive),
                //  we will pause the game and show it.
                PauseGame();
            }
            else
            {
                // If the Canvas is already visible (active),
                //  we will resume the game and hide it.
                ResumeGame();
            }
        }
    }

    void QuitGame()
    {
        // If running in the Unity editor, stop playing instead of quitting.
        //
        // The following is special instructions for the C# system.
        // It will be removed when a build is made.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // Quit the application (this will not have any effect in the Unity editor).
        Application.Quit();
    }
    
    void ResumeGame()
    {
        // Hide the pause menu and resume the game.
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void PauseGame()
    {
        // Show the pause menu and pause the game.
        PauseCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
