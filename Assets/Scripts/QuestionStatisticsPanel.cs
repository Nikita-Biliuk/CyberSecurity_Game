using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

/// <summary>
/// Displays statistics about cybersecurity questions:
/// total count and count per category.
/// </summary>
public class QuestionStatisticsPanel : MonoBehaviour
{
    [Header("Database")]
    public CyberQuestionDatabase questionDatabase;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text totalText;
    public TMP_Text categoryText;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    /// <summary>
    /// Called by the Statistics button.
    /// </summary>
    public void Open()
    {
        Debug.Log("QuestionStatisticsController STARTED");

        if (panel != null)
            panel.SetActive(true);

        UpdateStatistics();
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void UpdateStatistics()
    {
        if (questionDatabase == null || questionDatabase.questions == null)
        {
            Debug.LogWarning("Question database not assigned!");
            return;
        }

        List<CyberQuestion> questions = questionDatabase.questions;

        // Total count
        totalText.text = "Total questions: " + questions.Count;

        // Count per category
        Dictionary<QuestionCategory, int> categoryCounts =
            new Dictionary<QuestionCategory, int>();

        foreach (QuestionCategory cat in System.Enum.GetValues(typeof(QuestionCategory)))
            categoryCounts[cat] = 0;

        foreach (CyberQuestion q in questions)
            categoryCounts[q.category]++;

        // Build formatted output
        string result = "";
        foreach (var pair in categoryCounts)
        {
            string niceName = FormatCategoryName(pair.Key.ToString());
            result += $"{niceName}: {pair.Value}\n";
        }

        categoryText.text = result;
    }

    /// <summary>
    /// Converts enum names like "NetworkAndWebSecurity"
    /// into "Network And Web Security"
    /// </summary>
    private string FormatCategoryName(string raw)
    {
        return Regex.Replace(raw, "(\\B[A-Z])", " $1");
    }
}
