using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple encyclopedia manager that uses manually created buttons (two columns).
/// Each button is assigned a CyberQuestion in the inspector (lists must match).
/// On click the DetailPanel shows question and answers (correct answer highlighted green).
/// DetailPanel has only one Back button to close it.
/// </summary>
public class EncyclopediaManualManager : MonoBehaviour
{
    [Header("Manual question mapping")]
    [Tooltip("Buttons you created manually in the UI (left+right columns). Order matters.")]
    public List<Button> questionButtons = new List<Button>();

    [Tooltip("Corresponding ScriptableObjects (CyberQuestion). Must be same length as questionButtons.")]
    public List<CyberQuestion> questions = new List<CyberQuestion>();

    [Header("Detail panel UI (assign)")]
    public GameObject detailPanel;              // panel to show/hide
    public TMP_Text detailQuestionText;         // full question text
    public List<TMP_Text> answerTexts;          // 4 (or n) TMP texts for answers
    public Button detailBackButton;   // only back button inside detail panel
    public TMP_Text categoryText;
    public TMP_Text sourceText;
              

    [Header("Colors")]
    public Color defaultAnswerColor = Color.white;
    public Color correctAnswerColor = Color.green;

    private void Awake()
    {
        // basic validation
        if (questionButtons.Count != questions.Count)
        {
            Debug.LogWarning($"EncyclopediaManualManager: questionButtons.Count ({questionButtons.Count}) != questions.Count ({questions.Count}). Make sure lists match.");
        }

        // hide detail panel at start
        if (detailPanel != null) detailPanel.SetActive(false);

        // wire buttons
        int len = Mathf.Min(questionButtons.Count, questions.Count);
        for (int i = 0; i < len; i++)
        {
            int idx = i; // capture
            Button btn = questionButtons[i];
            if (btn == null) continue;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnQuestionButtonClicked(idx));
        }

        // detail back
        if (detailBackButton != null)
        {
            detailBackButton.onClick.RemoveAllListeners();
            detailBackButton.onClick.AddListener(CloseDetail);
        }
    }

    private void OnQuestionButtonClicked(int index)
    {
        if (index < 0 || index >= questions.Count) return;
        CyberQuestion q = questions[index];
        if (q == null)
        {
            Debug.LogWarning("EncyclopediaManualManager: question is null at index " + index);
            return;
        }

        ShowDetail(q);
    }

    private void ShowDetail(CyberQuestion question)
    {
        if (detailPanel == null || detailQuestionText == null || answerTexts == null || answerTexts.Count == 0)
        {
            Debug.LogError("EncyclopediaManualManager: UI references missing.");
            return;
        }

        detailPanel.SetActive(true);

        // set question text
        detailQuestionText.text = question.questionText;

        if (categoryText != null)
        {
            categoryText.text = "Category: " + question.category.ToString();
        }
        else
        {
            Debug.LogWarning("CategoryText reference is missing in EncyclopediaManualManager!");
        }

        if (sourceText != null)
        {
            sourceText.text = "Source: " + question.source;
        }
        else
        {
            Debug.LogWarning("SourceText reference is missing in EncyclopediaManualManager!");
        }

        // populate answers (answerTexts count may be >= question.answers.Count)
        for (int i = 0; i < answerTexts.Count; i++)
        {
            TMP_Text t = answerTexts[i];
            if (t == null) continue;

            if (i < question.answers.Count)
            {
                t.gameObject.SetActive(true);
                t.text = question.answers[i];

                // highlight correct one green, others default
                if (i == question.correctAnswerIndex)
                    t.color = correctAnswerColor;
                else
                    t.color = defaultAnswerColor;
            }
            else
            {
                // no answer for this slot -> hide text
                t.gameObject.SetActive(false);
            }
        }
    }

    private void CloseDetail()
    {
        if (detailPanel != null) detailPanel.SetActive(false);
    }
}
