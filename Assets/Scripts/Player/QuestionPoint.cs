using UnityEngine;

public class QuestionPoint : MonoBehaviour
{
    private bool answered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!answered && other.CompareTag("Player"))
        {
            AskQuestion();
        }
    }

    private void AskQuestion()
    {
        CyberQuestionManager.Instance.ShowRandomQuestion(OnQuestionAnswered);
    }

    private void OnQuestionAnswered(bool correct)
{
    // fixes double-callback issue
    GameManager.instance.RegisterAnswer(correct);

    if (correct)
    {
        answered = true;
        gameObject.SetActive(false);
        GameManager.instance.CheckWinCondition();
    }
    else
    {
        // ask again after a short delay
        Invoke(nameof(AskQuestionAgain), 0.1f);
    }
}

private void AskQuestionAgain()
{
    CyberQuestionManager.Instance.ShowRandomQuestion(OnQuestionAnswered);
}
}
