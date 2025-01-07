using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //动画显示效果引用
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum MathQuestionsType
{
    Single,
    Multiple,
}
public class countManager : MonoBehaviour, _ISeriportInput
{

    //public List<QuestionOrder> questionOrders;//问题库
    //[SerializeField] GameObject[] options; //答案选项
    //[SerializeField] Button[] buttons;//按钮

    //开始动画 算法模式
    [SerializeField] Transform Levelstart; //开始动画 蓝色圈第一题目

    //加音乐

    [SerializeField] Transform Correctprompt; //正确提示
    [SerializeField] Transform Correctanswerdisplay; //正确答案显示

    [SerializeField] Transform Wrongprompt; //错误提示
    [SerializeField] Transform Startpromptt; //开始提示
    [SerializeField] Transform Endprompt; //结束提示
    [SerializeField] Transform Ratioizehint; //比大小游戏开始提示   
    [SerializeField] Transform Addandsubtractgameips; //加减法游戏开始提示   
    [SerializeField] Transform Finalscore; //最终回答正确数量
    [SerializeField] Text checkpoint; //关卡
    private int checkpointnumber = 1; //关卡数
    [SerializeField] Text correctAnswers;//正确数量
    private int correctAnswersCount = 0; //正确数量

    [SerializeField] Transform[] SingleQuestionAnserbuttons;//单选题答案选项按钮
    [SerializeField] Transform[] MultipleQuestionAnserbuttons;//多选题答案选项按钮
    [SerializeField] Transform[] Choosemorequestionbanks;//多选题选择题库

    [SerializeField] MathQuestionOrde mathQuestionOrde;
    int currentLevel = 0; //当前关卡
    int singleQuestionCount = 5; //单选题数量
    private bool canAnswer = false;//是否可以回答
    MathQuestionsType currentMathQuestionType = MathQuestionsType.Single; //当前问题类型
    MathQuestionsType currentMathQuestionType2 = MathQuestionsType.Multiple; //当前问题类型
    public AudioClip bottonAudio;

    void Start()
    {
        BindButton();
        Levelstartanimation();// 开始渐出动画
        correctAnswersCountDisplay(); //文本UI显示
    }
    private void Levelstartanimation()// 开始渐出动画
    {

        if (currentMathQuestionType == MathQuestionsType.Single)//单选题
        {

            //Startpromptt.localScale = Vector3.one;  // 游戏开始，设置缩放为 1
            // Startpromptt.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            Addandsubtractgameips.transform.DOScale(1, 0.5f).OnComplete(() =>
             {
                 Startpromptt.transform.DOScale(1, 1f) //DOScale=缩放显示(1=大小, 0.5f=速度)
               .OnComplete(() =>
               {
                   Startpromptt.localScale = Vector3.zero;  // 游戏开始，设置缩放为 1
                   Levelstart.transform.DOScale(1, 0.5f);
                   // 更新关卡文本为当前关卡的问题
                   Levelstart.Find("Text").GetComponent<Text>().text = mathQuestionOrde.singleQuestions[currentLevel].QuestionText;//更新关卡文本为当前关卡的问题
                   float drictionTime = 1f;//动画时间
                   for (int i = 0; i < SingleQuestionAnserbuttons.Length; i++) // 遍历单选题的每个按钮
                   {
                       // 更新按钮文本为当前关卡的答案
                       SingleQuestionAnserbuttons[i].Find("Text").GetComponent<Text>().text = mathQuestionOrde.singleQuestions[currentLevel].AnswerText[i];
                       SingleQuestionAnserbuttons[i].DOScale(1, drictionTime += 0.1f); // 逐个放大显示答案选项，放大时间逐渐增加
                   }

                   canAnswer = true;
                   Debug.Log("单题目动画完成，可以开始答题！");
               });
             });
        }
        else if (currentMathQuestionType == MathQuestionsType.Multiple) //多选题
        {

            Ratioizehint.transform.DOScale(1, 0.5f).OnComplete(() =>  //DOScale=缩放显示(1=大小, 0.5f=速度)
            {

                Startpromptt.transform.DOScale(1, 1f) //DOScale=缩放显示(1=大小, 0.5f=速度)
                    .OnComplete(() =>
                {
                    Startpromptt.localScale = Vector3.zero;  // 游戏开始，设置缩放为 1
                    for (int i = 0; i < Choosemorequestionbanks.Length; i++) // 遍历多选题的每个显示题目
                    {
                        Choosemorequestionbanks[i].Find("Text").GetComponent<Text>().text = mathQuestionOrde.multipleQuestions[currentLevel - singleQuestionCount].QuestionText[i];//更新多选题的每个按钮的文本
                        Choosemorequestionbanks[i].DOScale(1, 0.5f); // 逐个放大显示答案选项，放大时间逐渐增加
                        float drictionTime = 1f;//动画时间
                        for (int j = 0; j < MultipleQuestionAnserbuttons.Length; j++) // 遍历单选题的每个按钮
                        {
                            MultipleQuestionAnserbuttons[j].DOScale(1, drictionTime += 0.1f); // 逐个放大显示答案选项，放大时间逐渐增加
                                                                                              // Ratioizehint.transform.DOScale(0, 0f); // 比大小游戏开始提示隐藏
                        }
                    };
                    canAnswer = true;
                });
            });

            Debug.Log("多题目动画完成，可以开始答题！");
        }



    }

    private void Onclick()
    {
        if (!canAnswer)
        {
            Debug.Log("请等待动画完成后再答题！");
            return;
        }
        canAnswer = false;
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>(); //获取当前点击的按钮
        //单选题按钮
        if (button.name == "Blue_Button")
        {
            CheckAnswer("蓝色", 0);
        }
        else if (button.name == "Green_Button")
        {
            CheckAnswer("绿色", 1);
        }
        else if (button.name == "Red_Button")
        {

            CheckAnswer("红色", 2);
        }
        else if (button.name == "Yellow_Button")
        {
            CheckAnswer("黄色", 3);
        }

        //多选题按钮
        if (button.name == "Blue_Button2")
        {
            CheckAnswer("蓝色", 0);
            Debug.Log("蓝色按钮按下！");
        }
        else if (button.name == "Green_Button2")
        {
            CheckAnswer("绿色", 1);
            Debug.Log("绿色按钮按下！");
        }
        else if (button.name == "Red_Button2")
        {

            CheckAnswer("红色", 2);
            Debug.Log("红色按钮按下！");
        }
        else if (button.name == "Yellow_Button2")
        {
            CheckAnswer("黄色", 3);
            Debug.Log("黄色按钮按下！");
        }
    }
    void CheckAnswer(string answer, int index)
    {
        GetComponent<AudioSource>().PlayOneShot(bottonAudio);
        if (currentMathQuestionType == MathQuestionsType.Single) // 检查当前问题类型是否为单选题
        {
            if (index == mathQuestionOrde.singleQuestions[currentLevel].CorrectAnswerIndex) // 检查用户选择的答案索引是否与正确答案索引匹配
            {
                Debug.Log("正确");// 输出正确的日志
                correctAnswersCount++;//正确数量++；
                correctAnswersCountDisplay(); //文本UI显示
                Correctprompt.transform.DOScale(1, 0.5f); // 正确提示动画
                DOVirtual.DelayedCall(1f, () =>
                {
                    Correctprompt.transform.DOScale(0, 0.5f);
                });
            }
            else
            {
                Debug.Log("错误");// 输出错误的日志

                Wrongprompt.transform.DOScale(1, 0.5f);
                Correctanswerdisplay.transform.DOScale(1, 0.5f);
                correctAnswersCountDisplay();
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    Wrongprompt.transform.DOScale(0, 0.5f);
                    Correctanswerdisplay.transform.DOScale(0, 0.5f);


                });
            }
            DOVirtual.DelayedCall(1.5f, () =>//前一题动画消失
            {
                Levelstart.transform.DOScale(0, 0);
                Levelstart.Find("Text").GetComponent<Text>().text = mathQuestionOrde.singleQuestions[currentLevel].QuestionText;
                float drictionTime = 0.1f;//动画时间
                for (int i = 0; i < SingleQuestionAnserbuttons.Length; i++)
                {
                    SingleQuestionAnserbuttons[i].Find("Text").GetComponent<Text>().text = mathQuestionOrde.singleQuestions[currentLevel].AnswerText[i];
                    SingleQuestionAnserbuttons[i].DOScale(0, drictionTime);
                    Levelstartanimation();
                };
            });
        }


        else if (currentMathQuestionType == MathQuestionsType.Multiple) // 检查当前问题类型是否为多选题
        {

            if (index == mathQuestionOrde.multipleQuestions[currentLevel - singleQuestionCount].CorrectAnswerIndex)//按钮与正确答案索引匹配
            {
                Debug.Log("正确");
                correctAnswersCount++;//正确数量++；
                Correctprompt.transform.DOScale(1, 0.5f);
                correctAnswersCountDisplay(); //文本UI显示
                DOVirtual.DelayedCall(1f, () =>
               {
                   Correctprompt.transform.DOScale(0, 0.5f);
               });
            }
            else
            {
                Debug.Log("错误");
                Wrongprompt.transform.DOScale(1, 0.5f);
                Correctanswerdisplay.transform.DOScale(1, 0.5f);
                correctAnswersCountDisplay(); //文本UI显示
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    Wrongprompt.transform.DOScale(0, 0.5f);
                    Correctanswerdisplay.transform.DOScale(0, 0.5f);
                });

            }
            //前一题动画消失
            DOVirtual.DelayedCall(1.5f, () =>
            {

                float drictionTime = 0.1f;//动画时间
                for (int i = 0; i < SingleQuestionAnserbuttons.Length; i++)
                {
                    Choosemorequestionbanks[i].Find("Text").GetComponent<Text>().text = mathQuestionOrde.multipleQuestions[currentLevel - singleQuestionCount].QuestionText[i];//更新多选题的每个按钮的文本
                    Choosemorequestionbanks[i].DOScale(0, drictionTime); // 逐个放大显示答案选项，放大时间逐渐增加
                    Levelstartanimation();
                };
            });
        }
        DOVirtual.DelayedCall(1.0f, () =>//下一题动画
        {
            currentLevel++; //关卡++；



        });


        checkpointnumber++;//关卡数++；
        correctAnswersCountDisplay(); //文本UI显示
        currentMathQuestionType = currentLevel >= singleQuestionCount - 1 ? MathQuestionsType.Multiple : MathQuestionsType.Single; //当前问题类型
        if (checkpointnumber >= 4)//加减法游戏开始提示隐藏
        {
            Debug.Log("加减法游戏开始提示隐藏");
            Addandsubtractgameips.transform.DOScale(0, 0.5f);
        }

        if (currentLevel >= 9)
        {
            Ratioizehint.transform.DOScale(0, 0f); // 比大小游戏开始提示隐藏
            Endprompt.transform.DOScale(1, 0.8f);
            Finalscore.transform.DOScale(1, 0.8f); //最终回答正确数量显示
            correctAnswersCountDisplay();//文本UI显示

            DOVirtual.DelayedCall(5f, () =>
            {
                Finalscore.transform.DOScale(0, 0.8f);
                SceneManager.LoadScene("主页面"); //加载主页面
            });
        }
    }
    private void correctAnswersCountDisplay() //文本UI显示
    {
        correctAnswers.text = correctAnswersCount.ToString();//正确数量显示
        checkpoint.text = Math.Clamp(checkpointnumber, 1, 10).ToString();//关卡显示
        //Finalscore.Find("Text").GetComponent<Text>().text = "你的正确数量是：" + correctAnswersCount.ToString();//最终回答正确数量显示
        // 计算正确率并显示
        float accuracyRate = (float)correctAnswersCount / (checkpointnumber-1 ) * 100;
        Finalscore.Find("Text").GetComponent<Text>().text = "你的正确率是：" + accuracyRate.ToString("F1") + "%";
        Debug.Log("正确率：" + accuracyRate.ToString("F1") + "%");
        if (currentMathQuestionType == MathQuestionsType.Single)
        {
            Correctanswerdisplay.transform.Find("Text").GetComponent<Text>().text = "正确答案是：" + mathQuestionOrde.singleQuestions[currentLevel].AnswerText[mathQuestionOrde.singleQuestions[currentLevel].CorrectAnswerIndex];
            Debug.Log($"正确答案索引: {mathQuestionOrde.singleQuestions[currentLevel].CorrectAnswerIndex}");
        }
        else if (currentMathQuestionType == MathQuestionsType.Multiple)
        {
            Correctanswerdisplay.transform.Find("Text").GetComponent<Text>().text = "正确答案是：" + mathQuestionOrde.multipleQuestions[currentLevel - singleQuestionCount].QuestionText[mathQuestionOrde.multipleQuestions[currentLevel - singleQuestionCount].CorrectAnswerIndex];
            Debug.Log($"正确答案索引: {mathQuestionOrde.multipleQuestions[currentLevel - singleQuestionCount].CorrectAnswerIndex}");
        }

    }
    void BindButton()
    {
        // 为单选题的每个按钮添加点击事件监听器
        foreach (var button in SingleQuestionAnserbuttons)
        {
            button.GetComponent<Button>().onClick.AddListener(Onclick);
        }
        // 为多选题的每个按钮添加点击事件监听器
        foreach (var button in MultipleQuestionAnserbuttons)
        {
            button.GetComponent<Button>().onClick.AddListener(Onclick);
        }
    }

    public void OnDataReceived(int buttonIndex)
    {
        if (!canAnswer)
            return;
        CheckAnswer("", buttonIndex);
        canAnswer = false;
    }
}
