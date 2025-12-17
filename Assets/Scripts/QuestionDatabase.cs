using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CyberQuestionDatabase", menuName = "ScriptableObjects/CyberQuestionDatabase")]
public class CyberQuestionDatabase : ScriptableObject
{
    public List<CyberQuestion> questions = new List<CyberQuestion>();
}
