using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI entry element for a single question inside the encyclopedia list.
/// This script must be attached to the entry prefab.
/// </summary>
public class QuestionEntry : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text titleText;   // Short preview of the question
    public Button button;        // Button to open the full question view

    private CyberQuestion data;                // The question this entry represents
    private QuestionEncyclopedia manager;      // Reference to the encyclopedia manager

    /// <summary>
    /// Sets up the entry with the correct question and assigns the click event.
    /// </summary>
    public void Setup(CyberQuestion question, QuestionEncyclopedia mgr)
    {
        data = question;
        manager = mgr;

        // Display shortened question text so the list looks clean
        if (titleText != null)
            titleText.text = ShortTitle(question.questionText);

        // Setup button click callback
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => manager.ShowDetail(data));
        }
    }

    /// <summary>
    /// Makes a short preview string so that long questions do not break the list layout.
    /// </summary>
    private string ShortTitle(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        const int maxLength = 60;

        return s.Length <= maxLength
            ? s
            : s.Substring(0, maxLength - 3) + "...";
    }
}
