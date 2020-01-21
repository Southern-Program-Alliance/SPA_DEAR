using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionUIManager : MonoBehaviour
{
    // constants
    private const char ANSWER_A = 'A';
    private const char ANSWER_B = 'B';
    private const char ANSWER_C = 'C';

    [SerializeField] GameObject questionUI = null;
    [SerializeField] TextMeshProUGUI scenarioText = null;
    [SerializeField] TextMeshProUGUI scenarioNumberText = null;
    [SerializeField] int activeQuestionIndex = 0;
    private SO_QuestionInfo currentQuestion = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool inActive)
    {
        questionUI.SetActive(true);
    }

    public void SetScenarioText(string inText)
    {
        scenarioText.text = inText;
        scenarioNumberText.text = "1";
    }

    public void GetCurrentQuestioInfo()
    {
        currentQuestion = SpawnManager.Instance.GetQuestion(activeQuestionIndex);
    }

    private void DisplayCurrentQuestion()
    {
        questionUI.SetActive(true);
        scenarioText.text = currentQuestion.questionText;
        scenarioNumberText.text = (activeQuestionIndex + 1).ToString();
    }

    public void OnQuestionButtonClicked(char inButtonLetter)
    {
        Debug.Log("BUTTON PRESS");
        if (currentQuestion.answer == inButtonLetter)
            DisplayCorrectText(true);
        else
            DisplayCorrectText(false);

    }

    private void DisplayCorrectText(bool inCorrect)
    {
        scenarioText.text = currentQuestion.correctText;
    }


}
