using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;  // 需要先引入DOTween 显示缩放UI
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEditor;
[Serializable]
public class Question
{
    public string question;
    public string anser1;
    public string anser2;
    public string anser3;
    public string anser4;
    public string rightAnser_1;
}
public class SceneManagerment : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Sprite[] sprites;//0 ，1，2 改变Iimage图片状态
    [SerializeField] AudioClip muneAudioEffect; //音效
    [SerializeField] Question[] questions; //题目数组
    [SerializeField] GameObject rightAnserDispanle; //正确答案显示面板
    [SerializeField] GameObject Finalscore; //最终成绩显示面板
    [SerializeField] Text questionText; //题目
    [SerializeField] Text A_Text; //A按键
    [SerializeField] Text B_Text; //B
    [SerializeField] Text C_Text; //C
    [SerializeField] Text D_Text; //D


    [SerializeField] Text Countdown; //倒计时时间计数
    private float timeRemaining = 30f;
    private bool isTimerRunning = true;

    [SerializeField] Text CorrectQuantity; //真确数量计数
    private int correctCount = 0;
    [SerializeField] Text WrongQuantity; //错误数量计数
    private int wrongCount = 0;
    private bool hasWrongInCurrentQuestion = false; //当前题目是否已经答错
    [SerializeField] Text Grade; //错误数量计数
    private int GradeCount = 0;

    [SerializeField] Text Right_Text;
    [SerializeField] Text Finalscore_Text; //最终成绩文本


    [SerializeField] Button A_Button;
    [SerializeField] Button B_Button;
    [SerializeField] Button C_Button;
    [SerializeField] Button D_Button;
    /// <summary>
    /// Fasle 为无法按下按钮  Ture 可按下按钮
    /// </summary>
    bool buttonState = true; //按键状态
    int questionIndex = 0; //题目进度计数

  

    void Start()
    {
        DisQuestion();  //显示实时题目
        UpdateGradeDisplay(); // 初始化成绩显示
        UpdateCorrectQuantityDisplay(); //正确显示
        UpdateWrongQuantityDisplay(); // 初始化错误数量显示
        A_Button.onClick.AddListener(A_ButtonAnserQustion); //获取A按键状态并进入A_ButtonAnserQustion函数进行对应操作
        B_Button.onClick.AddListener(B_ButtonAnserQustion);
        C_Button.onClick.AddListener(C_ButtonAnserQustion);
        D_Button.onClick.AddListener(D_ButtonAnserQustion);
        Finalscore.transform.localScale = Vector3.zero; // 初始化时隐藏成绩面板
        SeriportManager.instance.OnDataReceived += OnDataReceived;

    }

    private void OnDataReceived(byte[] recivedBytes)
    {
        if (recivedBytes.Length < 8)
            return;
        switch (recivedBytes[7])
        {
            case 0x01:
                A_ButtonAnserQustion();
                break;
            case 0x02:
                B_ButtonAnserQustion();
                break;
            case 0x03:
                C_ButtonAnserQustion();
                break;
            case 0x04:
                D_ButtonAnserQustion();
                break;
        }
    
    }
    void OnDestroy()
    {
        SeriportManager.instance.OnDataReceived -= OnDataReceived;
    }
    void DisQuestion() //显示问题状态进度
    {
        questionText.text = questions[questionIndex].question; //题目进度
        A_Text.text = questions[questionIndex].anser1;  //A选项文本进度
        B_Text.text = questions[questionIndex].anser2;
        C_Text.text = questions[questionIndex].anser3;
        D_Text.text = questions[questionIndex].anser4;
        Right_Text.text = "正确答案\r\n \r\n" + questions[questionIndex].rightAnser_1;//答案回答UI
        hasWrongInCurrentQuestion = false; // 重置当前题目的错误状态
        buttonState = true; // 新题目时重置按钮状态为可点击
    }
    // 添加显示最终成绩的函数
    private void ShowFinalScore()
    {
        Finalscore.transform.DOScale(1, 0.5f); // 显示成绩面板
        Finalscore_Text.text = "你的成绩\r\n \r\n" + GradeCount.ToString() + "分";
        DOVirtual.DelayedCall(2f, () =>
        {
            Finalscore.transform.DOScale(0, 0.5f); // 隐藏成绩面板
            SceneManager.LoadScene("主页面"); //加载场景

        });

    }
    void A_ButtonAnserQustion() //按下A按钮函数
    {
        if (buttonState == false)
            return;
        buttonState = false; //按钮状态变为无法按下，防止多次按下
        string anseringText = A_Button.transform.GetComponentInChildren<Text>().text; //获取Button组件下面的Text的内容
        if (anseringText == questions[questionIndex].rightAnser_1) //组件内容与答案进行对比
        {

            isTimerRunning = false; //时间暂停计数
            questionIndex++; //题目刷新
            correctCount++;//正确数量++
            GradeCount += 10; //成绩+10分
            UpdateGradeDisplay(); //更新成绩显示
            UpdateCorrectQuantityDisplay(); //更新数量显示
            A_Button.GetComponent<Image>().sprite = sprites[1]; // 假设sprites[1]是正确答案的颜色

            DOVirtual.DelayedCall(2f, () =>
            { //延迟2s
                A_Button.GetComponent<Image>().sprite = sprites[0]; // 恢复默认颜色
                                                                    //questionIndex++;
                if (questionIndex < questions.Length)
                {
                    DisQuestion();
                    StartTimer();
                }
                buttonState = true; //按钮状态变为可以按下
            });
            Debug.Log("正确"); //打印
            if (questionIndex >= questions.Length) //题目答完显示成绩
            {
                ShowFinalScore();
                Debug.Log("题目答完");
            }
        }
        else
        {

            if (!hasWrongInCurrentQuestion && wrongCount < 10) //错误数量不大于10个
            {
                wrongCount++; //错误加加
                hasWrongInCurrentQuestion = true;
                UpdateWrongQuantityDisplay();
            }
            A_Button.GetComponent<Image>().sprite = sprites[2];
            isTimerRunning = false; // 停止计时器
            rightAnserDispanle.transform.DOScale(1, 0.5f).OnComplete(() =>  //DOScale=缩放显示(1=大小, 0.5f=速度)
            {
                rightAnserDispanle.transform.DOScale(0, 0.5f).SetDelay(2).OnComplete(() => //延迟2s之后隐藏UI
                {
                    isTimerRunning = false;
                    questionIndex++;
                    if (questionIndex < questions.Length)
                    {
                        DisQuestion();
                        StartTimer(); // 开始新一题的计时
                    }
                    else if (questionIndex >= questions.Length) //题目答完显示成绩
                    {
                        ShowFinalScore();
                        Debug.Log("题目答完");
                    }
                    buttonState = true;//按钮状态变为可以按下
                    A_Button.GetComponent<Image>().sprite = sprites[0];
                });
            });
            Debug.Log("错误");

        }
        GetComponent<AudioSource>().PlayOneShot(muneAudioEffect); //添加按钮音效
        Debug.Log("回答A"); //打印

    }


    void B_ButtonAnserQustion() //按下B按钮函数
    {
        if (buttonState == false)
            return;
        buttonState = false;  //按钮状态变为无法按下，防止多次按下
        string anseringText = B_Button.transform.GetComponentInChildren<Text>().text;
        if (anseringText == questions[questionIndex].rightAnser_1)
        {
            questionIndex++;
            correctCount++;//正确数量++
            GradeCount += 10; //成绩+10分
            UpdateGradeDisplay(); //更新成绩显示
            UpdateCorrectQuantityDisplay(); //更新数量显示
            B_Button.GetComponent<Image>().sprite = sprites[1]; // 假设sprites[1]是正确答案的颜色

            DOVirtual.DelayedCall(2f, () =>
            {
                B_Button.GetComponent<Image>().sprite = sprites[0]; // 恢复默认颜色
                if (questionIndex < questions.Length)
                {
                    DisQuestion();
                    StartTimer();
                }

                buttonState = true; //按钮状态变为可以按下
            });
            Debug.Log("正确");
            if (questionIndex >= questions.Length) //题目答完显示成绩
            {
                ShowFinalScore();
                Debug.Log("题目答完");
            };

        }
        else
        {
            if (!hasWrongInCurrentQuestion && wrongCount < 10) //错误数量不大于10个
            {
                wrongCount++; //错误加加
                hasWrongInCurrentQuestion = true;
                UpdateWrongQuantityDisplay();
            }

            isTimerRunning = false; // 停止计时器
            B_Button.GetComponent<Image>().sprite = sprites[2];
            rightAnserDispanle.transform.DOScale(1, 0.5f).OnComplete(() =>  //DOScale=缩放显示(1=大小, 0.5f=速度)
          {
              rightAnserDispanle.transform.DOScale(0, 0.5f).SetDelay(2).OnComplete(() => //延迟2s之后隐藏UI
              {
                  isTimerRunning = false;
                  questionIndex++;
                  if (questionIndex < questions.Length)
                  {
                      DisQuestion();
                      StartTimer(); // 开始新一题的计时
                  }
                  else if (questionIndex >= questions.Length) //题目答完显示成绩
                  {
                      ShowFinalScore();
                      Debug.Log("题目答完");
                  }
                  buttonState = true;//按钮状态变为可以按下
                  B_Button.GetComponent<Image>().sprite = sprites[0];
              });
          });
            Debug.Log("错误");
        }
        Debug.Log("回答B");
        GetComponent<AudioSource>().PlayOneShot(muneAudioEffect); //添加按钮音效


    }
    void C_ButtonAnserQustion() //按下C按钮函数
    {

        if (buttonState == false)
            return;
        buttonState = false; //按钮状态变为无法按下，防止多次按下
        string anseringText = C_Button.transform.GetComponentInChildren<Text>().text;
        if (anseringText == questions[questionIndex].rightAnser_1)
        {
            questionIndex++;
            correctCount++;
            GradeCount += 10; //成绩+10分
            UpdateGradeDisplay(); //更新成绩显示
            UpdateCorrectQuantityDisplay(); //更新数量显示
            C_Button.GetComponent<Image>().sprite = sprites[1]; // 假设sprites[1]是正确答案的颜色

            DOVirtual.DelayedCall(2f, () =>
            {
                C_Button.GetComponent<Image>().sprite = sprites[0]; // 恢复默认颜色
                if (questionIndex < questions.Length)
                {
                    DisQuestion();
                    StartTimer();
                }
                buttonState = true; //按钮状态变为可以按下
            });
            Debug.Log("正确");
            if (questionIndex >= questions.Length) //题目答完显示成绩
            {
                ShowFinalScore();
                Debug.Log("题目答完");
            }
        }
        else
        {
            if (!hasWrongInCurrentQuestion && wrongCount < 10) //错误数量不大于10个
            {
                wrongCount++; //错误加加
                hasWrongInCurrentQuestion = true;
                UpdateWrongQuantityDisplay();
            }
            isTimerRunning = false; // 停止计时器
            C_Button.GetComponent<Image>().sprite = sprites[2];

            rightAnserDispanle.transform.DOScale(1, 0.5f).OnComplete(() =>  //DOScale=缩放显示(1=大小, 0.5f=速度)
        {
            rightAnserDispanle.transform.DOScale(0, 0.5f).SetDelay(2).OnComplete(() => //延迟2s之后隐藏UI
            {
                isTimerRunning = false;
                questionIndex++;
                if (questionIndex < questions.Length)
                {
                    DisQuestion();
                    StartTimer(); // 开始新一题的计时
                }
                else if (questionIndex == questions.Length) //题目答完显示成绩
                {
                    ShowFinalScore();
                    Debug.Log("题目答完");
                }
                buttonState = true;//按钮状态变为可以按下
                C_Button.GetComponent<Image>().sprite = sprites[0];
            });
        });
            Debug.Log("错误");

        }
        Debug.Log("回答C");
        GetComponent<AudioSource>().PlayOneShot(muneAudioEffect); //添加按钮音效
    }
    void D_ButtonAnserQustion() //按下B按钮函数
    {
        if (buttonState == false)
            return;
        buttonState = false; //按钮状态变为无法按下，防止多次按下
        string anseringText = D_Button.transform.GetComponentInChildren<Text>().text;
        if (anseringText == questions[questionIndex].rightAnser_1)
        {
            questionIndex++;
            correctCount++;
            GradeCount += 10; //成绩+10分
            UpdateGradeDisplay(); //更新成绩显示
            UpdateCorrectQuantityDisplay(); //更新数量显示
            D_Button.GetComponent<Image>().sprite = sprites[1]; // 假设sprites[1]是正确答案的颜色

            DOVirtual.DelayedCall(2f, () =>
            {
                D_Button.GetComponent<Image>().sprite = sprites[0]; // 恢复默认颜色
                if (questionIndex < questions.Length)
                {
                    DisQuestion();
                    StartTimer();
                }
                buttonState = true;//按钮状态变为可以按下
            });

            Debug.Log("正确");
            if (questionIndex >= questions.Length) //题目答完显示成绩
            {
                ShowFinalScore();
                Debug.Log("题目答完");
            }
        }
        else
        {
            if (!hasWrongInCurrentQuestion && wrongCount < 10) //错误数量不大于10个
            {
                wrongCount++; //错误加加
                hasWrongInCurrentQuestion = true;
                UpdateWrongQuantityDisplay();
            }

            isTimerRunning = false; // 停止计时器
            D_Button.GetComponent<Image>().sprite = sprites[2];
            rightAnserDispanle.transform.DOScale(1, 0.5f).OnComplete(() =>  //DOScale=缩放显示(1=大小, 0.5f=速度)
       {
           rightAnserDispanle.transform.DOScale(0, 0.5f).SetDelay(2).OnComplete(() => //延迟2s之后隐藏UI
           {
               isTimerRunning = false;
               questionIndex++;
               if (questionIndex < questions.Length)
               {
                   DisQuestion();
                   StartTimer(); // 开始新一题的计时
               }
               else if (questionIndex >= questions.Length) //题目答完显示成绩
               {
                   ShowFinalScore();
                   Debug.Log("题目答完");
               }
               buttonState = true;//按钮状态变为可以按下
               D_Button.GetComponent<Image>().sprite = sprites[0];
           });
       });
            Debug.Log("错误");

        }
        Debug.Log("回答D");
        GetComponent<AudioSource>().PlayOneShot(muneAudioEffect); //添加按钮音效
    }
    void Update() //倒计时
    {

        if (timeRemaining > 0 && isTimerRunning == true) // 如果还有剩余时间
        {
            timeRemaining -= Time.deltaTime; // 减去上一帧到这一帧的时间差
            UpdateTimerDisplay(); // 更新UI显示

        }
        else if (timeRemaining <= 1)
        {
            timeRemaining = 0;
            isTimerRunning = false; // 停止计时器
            TimeUp(); // 执行时间到的处理
        }
    }
    // 更新倒计时显示
    void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        // Mathf.CeilToInt 将浮点数向上取整，比如29.1会显示为30
        Countdown.text = seconds.ToString();
        // 更新UI文本显示
    }

    // 添加更新成绩显示的函数
    private void UpdateGradeDisplay()
    {
        Grade.text = GradeCount.ToString();
    }


    // 开始计时器
    void StartTimer()
    {
        timeRemaining = 30f;
        isTimerRunning = true;
    }
    // 时间用完时的处理
    void TimeUp()
    {
        Debug.Log("时间到！");
        // 自动进入下一题
        if (questionIndex + 1 < questions.Length) //题目数量对比
        {
            questionIndex++; //题目刷新
            DisQuestion(); //问题刷新
            StartTimer(); // 重新开始计时
        }
        else
        {
            // 所有题目已完成
            Debug.Log("问答完成！");
            // 这里可以添加游戏结束逻辑
        }
    }

    //答题数量
    private void UpdateCorrectQuantityDisplay()
    {
        CorrectQuantity.text = correctCount.ToString();
        // 或者简单显示数字
        // CorrectQuantity.text = correctCount.ToString();
    }

    // 添加错误数量显示更新函数
    private void UpdateWrongQuantityDisplay()
    {
        WrongQuantity.text = wrongCount.ToString();
    }


}
