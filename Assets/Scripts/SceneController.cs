using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1; // Reset time scale to normal when changing scenes
    }

     public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
