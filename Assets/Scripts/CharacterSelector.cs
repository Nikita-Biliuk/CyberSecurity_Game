using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;

    [Header("Selected Character")]
    public CharacterScriptableObject characterData;
    public bool characterSelected = false;

    private string lastSelectedScene = ""; 

    private void Awake()
    {
        // Destroy duplicate instances
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If new scene is menu or title screen, reset selection
        if (scene.name == "Menu" || 
            scene.name == "Title Screen") 
        {
            ResetSelection();
        }
        // If new scene is not menu or title screen, save its name
        else if (scene.name != "Title Screen" && 
                 scene.name != "Menu")
        {
            lastSelectedScene = scene.name;
        }
    }

    public void ResetSelection()
    {
        characterData = null;
        characterSelected = false;
        Debug.Log("Character selection reset");
    }

    public void ReplaceCharacter(CharacterScriptableObject newCharacter)
    {
        characterData = newCharacter;
        characterSelected = true;

    }

    public void SelectCharacter(CharacterScriptableObject character)
    {
        characterData = character;
        characterSelected = true;
        
        // Save selection to PlayerPrefs

        PlayerPrefs.Save();
        

    }

    public static CharacterScriptableObject GetData()
    {
        if (instance == null)
            return null;
        return instance.characterData;
    }

    // New clearing method
    public void ClearForMenu()
    {

        characterData = null;
        characterSelected = false;
    }
}