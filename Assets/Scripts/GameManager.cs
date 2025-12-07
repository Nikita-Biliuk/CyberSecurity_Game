using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // -----------------------------
    // Game States
    // -----------------------------
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    // -----------------------------
    // Main Screens
    // -----------------------------
    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    // -----------------------------
    // Current Stats
    // -----------------------------
    [Header("Current Stat Displays")]
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentMightDisplay;
    public TMP_Text currentProjectileSpeedDisplay;
    public TMP_Text currentMagnetDisplay;

    // -----------------------------
    // Results Screen Stats
    // -----------------------------
    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    // -----------------------------
    // Time Management
    // -----------------------------
    [Header("Stopwatch")]
    public float timeLimit;
    float stopwatchTime;
    public TMP_Text stopwatchDisplay;

    public bool isGameOver = false;
    public bool choosingUpgrade;
    public GameObject playerObject;

    // -----------------------------
    // Quiz Management
    // -----------------------------
    [Header("Quiz Results (optional)")]
    public GameObject quizResultsScreen;        // results screen for quiz
    public TMP_Text correctAnswersText;         // the number of correct answers
    public TMP_Text incorrectAnswersText;       // the number of incorrect answers
    public Button quizContinueButton;           // button to continue after quiz

    [HideInInspector] public int correctAnswersCount = 0;
    [HideInInspector] public int incorrectAnswersCount = 0;

    // -----------------------------
    // Unity Methods
    // -----------------------------
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DisableScreens();

        if (quizResultsScreen != null)
            quizResultsScreen.SetActive(false);
    }

    // -----------------------------
    // Update Loop
    // -----------------------------
    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                }
                break;
        }
    }

    // -----------------------------
    // State Management
    // -----------------------------
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Game State Changed to: " + currentState);
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            if (pauseScreen != null) pauseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            if (pauseScreen != null) pauseScreen.SetActive(false);
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void DisableScreens()
    {
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (resultsScreen != null) resultsScreen.SetActive(false);
        if (levelUpScreen != null) levelUpScreen.SetActive(false);
    }

    // -----------------------------
    // Main Game Methods
    // -----------------------------
    public void GameOver()
    {
        if (timeSurvivedDisplay != null && stopwatchDisplay != null)
            timeSurvivedDisplay.text = stopwatchDisplay.text;

        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        if (resultsScreen != null)
            resultsScreen.SetActive(true);
    }

    public void AssingChosenCharacterUI(CharacterScriptableObject chosenCharacterData)
    {
        if (chosenCharacterImage != null)
            chosenCharacterImage.sprite = chosenCharacterData.Icon;

        if (chosenCharacterName != null)
            chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        if (levelReachedDisplay != null)
            levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.LogError("Weapons and Passive Items UI lists do not match the size of the chosen weapons and passive items data lists!");
            return;
        }

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            if (chosenPassiveItemsData[i].sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;
        UpdateStopwatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    void UpdateStopwatchDisplay()
    {
        if (stopwatchDisplay == null)
            return;

        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        levelUpScreen.SetActive(false);
        Time.timeScale = 1f;
        ChangeState(GameState.Gameplay);
    }

    // -----------------------------
    // Quiz Methods
    // -----------------------------
    public void RegisterAnswer(bool correct)
    {
        if (correct)
            correctAnswersCount++;
        else
            incorrectAnswersCount++;
    }

    public void CheckWinCondition()
    {
        QuestionPoint[] points = FindObjectsOfType<QuestionPoint>();
        bool allAnswered = true;

        foreach (var p in points)
        {
            if (p.gameObject.activeSelf)
            {
                allAnswered = false;
                break;
            }
        }

        if (allAnswered)
        {
            Debug.Log("Win!");
            HandleVictory();
        }
    }

    private void HandleVictory()
    {
        if (quizResultsScreen != null)
        {
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);

            if (pauseScreen != null) pauseScreen.SetActive(false);
            if (resultsScreen != null) resultsScreen.SetActive(false);
            if (levelUpScreen != null) levelUpScreen.SetActive(false);

            quizResultsScreen.SetActive(true);

            if (correctAnswersText != null)
                correctAnswersText.text = "Correct answers: " + correctAnswersCount;

            if (incorrectAnswersText != null)
                incorrectAnswersText.text = "Incorrect answers: " + incorrectAnswersCount;

            if (quizContinueButton != null)
            {
                quizContinueButton.onClick.RemoveAllListeners();
                quizContinueButton.onClick.AddListener(() =>
                {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("Title Screen"); // To main menu
                });
            }
        }
        else
        {
            // Main win
            isGameOver = true;
            Time.timeScale = 0f;

            if (resultsScreen != null)
            {
                resultsScreen.SetActive(true);
                if (levelReachedDisplay != null) levelReachedDisplay.text = "Victory!";
                if (timeSurvivedDisplay != null) timeSurvivedDisplay.text = stopwatchDisplay != null ? stopwatchDisplay.text : "";
            }

            ChangeState(GameState.GameOver);
        }
    }
}
