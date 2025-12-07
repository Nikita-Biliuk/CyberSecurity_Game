using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CyberQuestionManager : MonoBehaviour
{
    public static CyberQuestionManager Instance;

    [Header("UI Elements")]
    public GameObject questionPanel;
    public TMP_Text questionText;
    public List<Button> answerButtons;

    [Header("Questions")]
    public List<CyberQuestion> allQuestions;

    [Header("Settings")]
    [Tooltip("UI-only setting now.")]
    public bool allowRetryUntilCorrect = false;

    private CyberQuestion currentQuestion;
    private System.Action<bool> onQuestionAnswered;
    private bool questionActive = false; // double-click protection

    private void Awake()
    {
        Instance = this;
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    public void ShowRandomQuestion(System.Action<bool> callback)
    {
        // Preliminary checks
        if (allQuestions == null || allQuestions.Count == 0 || questionPanel == null || questionText == null || answerButtons == null)
        {
            Debug.LogWarning("CyberQuestionManager: Missing components or questions.");
            callback?.Invoke(true);
            return;
        }

        onQuestionAnswered = callback;

        currentQuestion = GetRandomQuestion();
        if (currentQuestion == null)
        {
            callback?.Invoke(true);
            return;
        }

        DisplayQuestion(currentQuestion);

        Time.timeScale = 0f;      // pause game
        questionActive = true;    // open for answers
    }

    private CyberQuestion GetRandomQuestion()
    {
        if (allQuestions == null || allQuestions.Count == 0)
            return null;

        return allQuestions[Random.Range(0, allQuestions.Count)];
    }

    private void DisplayQuestion(CyberQuestion question)
    {
        questionText.text = question.questionText;

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < question.answers.Count)
            {
                var btn = answerButtons[i];
                btn.gameObject.SetActive(true);

                var txt = btn.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = question.answers[i];

                int index = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnAnswerSelected(index));
                btn.interactable = true; // turn on button
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        questionPanel.SetActive(true);
    }

    private void OnAnswerSelected(int index)
    {
        if (!questionActive) return; // double-click protection
        questionActive = false;

        // Turn off all buttons to prevent further clicks
        for (int i = 0; i < answerButtons.Count; i++)
            if (answerButtons[i] != null) answerButtons[i].interactable = false;

        bool correct = (index == currentQuestion.correctAnswerIndex);

        // Before invoking the callback, register the answer
        onQuestionAnswered?.Invoke(correct);

        // Close question UI
        if (questionPanel != null) questionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
