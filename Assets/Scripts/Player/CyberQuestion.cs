using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Categories used to classify cybersecurity questions
/// </summary>
public enum QuestionCategory
{
    AuthenticationAndAccess,
    PasswordSecurity,
    NetworkAndWebSecurity,
    PhishingAndEmailSecurity,
    MalwareAndThreats,
    GeneralCyberSecurity
}

[CreateAssetMenu(
    fileName = "CyberQuestion",
    menuName = "ScriptableObjects/CyberQuestion",
    order = 1
)]
public class CyberQuestion : ScriptableObject
{
    [Header("Question")]
    [TextArea(2, 5)]
    public string questionText;

    [Header("Answers")]
    public List<string> answers = new List<string>();

    [Tooltip("Index of the correct answer in the answers list")]
    public int correctAnswerIndex;

    [Header("Classification")]
    public QuestionCategory category;

    [Header("Source")]
    [Tooltip("Source of the question (e.g. OWASP, NIST, educational material)")]
    public string source;
}
