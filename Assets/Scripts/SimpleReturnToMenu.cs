using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleReturnToMenu : MonoBehaviour
{
    [Header("Settings")]
    public string mainMenuScene = "Title Screen"; 
    
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        
        // Resume the game time in case it was paused
        Time.timeScale = 1f;
        
        // Delete the player object to avoid duplicates
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Delete player: " + player.name);
            Destroy(player);
        }
        
        // Clean up CharacterSelector instance
        if (CharacterSelector.instance != null)
        {
            CharacterSelector.instance.ResetSelection();
            Destroy(CharacterSelector.instance.gameObject);
        }
        
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuScene);
    }
}