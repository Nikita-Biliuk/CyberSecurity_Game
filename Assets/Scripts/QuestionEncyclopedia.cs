using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionEncyclopedia : MonoBehaviour
{
    [Header("Database")]
    public CyberQuestionDatabase questionDatabase;

    [Header("List UI")]
    public Transform contentParent;
    public GameObject entryPrefab;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public TMP_Text detailQuestionText;

    // ✅ NEW
    public TMP_Text detailCategoryText;
    public TMP_Text detailSourceText;

    public Transform answersContainer;
    public GameObject answerButtonPrefab;
    public Button closeDetailButton;

    private List<CyberQuestion> allQuestions;

    void Awake()
    {
        allQuestions = questionDatabase.questions;

        PopulateList();

        detailPanel.SetActive(false);

        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(() => detailPanel.SetActive(false));
    }

    void PopulateList()
    {
        foreach (Transform t in contentParent)
            Destroy(t.gameObject);

        foreach (var q in allQuestions)
        {
            GameObject entry = Instantiate(entryPrefab, contentParent);
            var comp = entry.GetComponent<QuestionEntry>();
            comp.Setup(q, this);
        }
    }

    public void ShowDetail(CyberQuestion question)
    {
        detailPanel.SetActive(true);

        // Question text
        detailQuestionText.text = question.questionText;

        // ✅ Category
        if (detailCategoryText != null)
            detailCategoryText.text = "Category: " + question.category.ToString();

        // ✅ Source
        if (detailSourceText != null)
            detailSourceText.text = "Source: " + question.source;

        // Clear old answers
        foreach (Transform t in answersContainer)
            Destroy(t.gameObject);

        // Answers
        for (int i = 0; i < question.answers.Count; i++)
        {
            GameObject btnObj = Instantiate(answerButtonPrefab, answersContainer);

            TMP_Text t = btnObj.GetComponentInChildren<TMP_Text>();
            t.text = question.answers[i];

            Image img = btnObj.GetComponent<Image>();
            img.color = (i == question.correctAnswerIndex)
                ? Color.green
                : Color.white;
        }
    }
}
