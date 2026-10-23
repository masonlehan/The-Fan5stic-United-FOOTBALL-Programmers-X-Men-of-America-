using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string levelToLoad = "SampleScene"; 

    public void PlayGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void QuitGame()
    {
        // Closes the running game application seamlessly
        Debug.Log("Player has quit the game!"); 
        Application.Quit(); 
    }
}
