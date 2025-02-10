using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MathQuestion", menuName = "题库/数学题")]
public class MathQuestionOrde : ScriptableObject
{
    public List<SingleMathQuestion> singleQuestions;
    public List<MultipleMathQuestion> multipleQuestions;
    // public void GetMathQuestion(MathQuestionType type)
    // {

    // }
}
[System.Serializable]
public class SingleMathQuestion
{
    public string QuestionText; // 题目文本，存储当前问题的文本内容
    public List<string> AnswerText; // 答案文本，存储当前答案的文本内容
    public int CorrectAnswerIndex; //正确答案索引
}

[System.Serializable]
public class MultipleMathQuestion
{
    public List<string> QuestionText; // 题目文本，存储当前问题的文本内容
    public int CorrectAnswerIndex; //正确答案索引
}