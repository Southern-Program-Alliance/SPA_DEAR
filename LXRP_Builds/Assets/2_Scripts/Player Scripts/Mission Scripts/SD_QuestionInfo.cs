using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question 1", menuName = "ScriptableObjects/QuestionInfo", order = 2)]
public class SO_QuestionInfo : ScriptableObject
{
    public int questionNo;
    public string questionText;
    public bool isCorrect;
    public char answer;
    public string correctText;
    public string incorrectText;

    private bool isSelected = false;
    public bool IsSelected { get => isSelected; set => isSelected = value; }
}

