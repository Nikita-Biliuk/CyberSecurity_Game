using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CyberQuestion", menuName = "ScriptableObjects/CyberQuestion")]
public class CyberQuestion : ScriptableObject
{
    [TextArea(2, 5)]
    public string questionText;

    public List<string> answers = new List<string>();
    public int correctAnswerIndex;
}

