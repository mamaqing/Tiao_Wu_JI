using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening; // 需要先引入DOTween 显示缩放UI
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



[System.Serializable]
public class QuestionOrder
{
    public List<int> levelConfig; //关卡配置
    public int QuestionIndex; //正确答案索引
}


public class MemoryChallengeManager : MonoBehaviour, _ISerialPortInput
{
    [Header("音效")]
    [SerializeField] AudioClip 翻牌音效; //翻牌音效
    [SerializeField] AudioClip bottonAudio; //按钮音效
    //4张图，图片
    [SerializeField] AudioClip pictures;  //图片
    


    //等待提示
    [SerializeField] Transform Waitforprompt;  //等待提示UI
    [SerializeField] Transform Endprompt;  //结束提示UI
    [SerializeField] Transform Startanswering;  //开始答题UI
    [SerializeField] Transform Nextquestion;  //下一题UI

    //关卡
    [SerializeField] Text checkpoint;  //关卡UI
    private int checkpointCount = 1;  //关卡计数

    //选项卡
    [SerializeField] Transform TAB01;  //选项卡1    
  
                                          //[SerializeField] Transform TAB02;  //选项卡2


    //正确错误UI显示


    [SerializeField] Text correctAnswers;  //正确数量
    [SerializeField] Transform accuracy;  //正确率
    private int correctAnswersCount = 0; // 记录正确答案的数量



    public List<QuestionOrder> questionOrders;//问题库
    [SerializeField] GameObject[] options; //答案选项
   

    private bool canAnswer = false;
    private bool Gameover = true; //游戏未结束

    int currentLevel = 0; //当前关卡
    /// <summary>
    /// 1 红色
    /// 2 绿色
    /// 3 蓝色
    /// 4 黄色
    /// </summary>
    [SerializeField] Button[] buttons;//按钮
    void Start()
    {
        BingButton();
        Levelstartanimation();
        correctAnswersCountDisplay();
    }

    void BingButton()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(Onclick);
        }
    }

    private void Levelstartanimation()// 开始渐出动画
    {
        foreach (var optionIndex in questionOrders[currentLevel].levelConfig) //遍历当前关卡的所有选项索引
        {

            options[optionIndex].SetActive(true); //设置选项为激活状态
            options[optionIndex].transform.DOScale(1, 0.5f).SetEase(Ease.OutBack); //设置选项缩放为1，动画时间为1.5秒


        }
        var anserOption = options[questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]];//获取正确答案选项

        List<int> wrongOptions = new List<int>();
        for (int i = 0; i < questionOrders[currentLevel].levelConfig.Count; i++)
        {
            if (i != questionOrders[currentLevel].QuestionIndex)
                wrongOptions.Add(questionOrders[currentLevel].levelConfig[i]);
        }


        DOVirtual.DelayedCall(0.5f, () =>   //设置选  项卡位置为正确答案选项的位置
           {
               TAB01.transform.position = anserOption.transform.position;
           });   

        Waitforprompt.transform.DOScale(1, 0.5f); //设置等待提示UI缩放为1，动画时间为1.5秒

        Debug.Log("正确答案选项的位置: " + anserOption.transform.position); //输出正确答案选项的位置

        DOVirtual.DelayedCall(2f, () =>
        { //延迟2s，退出动画
           
                    // 首先将正确答案选项的缩放设置为 0
           // anserOption.transform.localScale = Vector3.zero;
                    // 将正确答案选项旋转-90度，添加缓动效0
            GetComponent<AudioSource>().PlayOneShot(翻牌音效);
            anserOption.transform.DORotate(new Vector3(0, 90, 0), 0.3f).SetEase(Ease.InSine).OnComplete(() => //正确答案选项旋转90度
            {
                TAB01.transform.DOScale(1, 0f); 
                TAB01.transform.DORotate(new Vector3(0, 90, 0), 0f);
            });
                   
            foreach (var wrongOption in wrongOptions) // 确保 wrongOption 是 Transform 类型
                {
                     
                    options[wrongOption].transform.DORotate(new Vector3(0, 90, 0), 0.3f).SetEase(Ease.InSine).OnComplete(() => //
                    {
                          options[wrongOption].transform.Find("UnknownCard").transform.localScale = new Vector3(1, 1, 1);
                          options[wrongOption].transform.Find("UnknownCard").transform.DORotate(new Vector3(0, 90, 0), 0f).OnComplete(() =>
                          {

                            
                          });   
                       // options[wrongOption].transform.DORotate(new Vector3(0, 0, 0), 0f).OnComplete(() =>
                          //  {
                            // options[wrongOption].transform.Find("UnknownCard").transform.localScale = new Vector3(1, 1, 1);
                            // options[wrongOption].transform.Find("UnknownCard").transform.DORotate(new Vector3(0, 90, 0), 3f);
                          //  });
      
                    });  
                }


      
            // foreach (var wrongOption in wrongOptions) //错误答案选项缩放为0
            //     {
            //         options[wrongOption].transform.DOScale(0, 0.5f);
            //     }
            // Bulepicture.transform.DOScale(0, 0.5f);
            Waitforprompt.transform.DOScale(0, 1f);//等待提示UI缩放为0
            //UnknownTAB.transform.DOScale(0, 0.1f);//未知选项卡缩放为0
            // 先获取 anserOption 的当前旋转角度
            Vector3 currentRotation = anserOption.transform.eulerAngles;

            DOVirtual.DelayedCall(0.3f, () =>
            {
               
              // TAB01.transform.DOScale(1, 2f);//选项卡缩放为1
                // foreach (var wrongOption in wrongOptions) //错误答案选项缩放为1
                // {
                //     options[wrongOption].transform.DOScale(1, 0.5f);
                // }
                // foreach (var wrongOption in wrongOptions) //错误答案选项缩放为1
                // {
                //     options[wrongOption].transform.Find("UnknownCard").transform.DOScale(1,0.5f);
                // }
                TAB01.transform.DORotate(new Vector3(0,  0, 0), 0.3f).SetEase(Ease.OutSine).OnComplete(() => //选项卡旋转为0
                    {
                        // 动画完成后启用按钮
                        canAnswer = true;
                        Debug.Log("加载完成50%，可以开始答题！");
                        DOVirtual.DelayedCall(1f, () =>
                        { //延迟2s
                            Startanswering.transform.DOScale(0, 0.8f);
                        });
                    });

                 foreach (var wrongOption in wrongOptions) //错误答案选项缩放为0
                    {
                        //options[wrongOption].transform.DORotate(new Vector3(0, 0, 0), 1f);
                        options[wrongOption].transform.Find("UnknownCard").transform.localScale = new Vector3(1, 1, 1);
                        options[wrongOption].transform.Find("UnknownCard").transform.DORotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
                        {
                           options[wrongOption].transform.DORotate(new Vector3(0, 0, 0), 0f);
                            options[wrongOption].transform.Find("UnknownCard").transform.DORotate(new Vector3(0, 0, 0), 0f);
                        });
                        // options[wrongOption].transform.Find("UnknownCard").transform.DOScale(new Vector3(0,0,0),1f).OnComplete(() =>
                        // {
                            
                        //     options[wrongOption].transform.Find("UnknownCard").transform.localScale = new Vector3(1, 1, 1);
                        //     canAnswer = true;
                        //     Debug.Log("加载完成100%，可以开始答题！");
                        //     DOVirtual.DelayedCall(1f, () =>
                        //     { //延迟2s
                        //         Startanswering.transform.DOScale(0, 0.8f);
                        //     });
                        // });
                    }

            });
        });

    }
    private void FlyOutAndDisappear(Transform picture)
    {
        // 定义飞出去的目标位置
        // Vector3 targetPosition = new Vector3(0, 693, 0); // 例如，飞到 y=5 的位置，您可以根据需要调整

        // 先将对象移动到目标位置，然后缩放到 0
        picture.DOLocalMoveY(693, 2.5f) // 移动到目标位置，持续时间为 1.5 秒
            .OnComplete(() =>
            {
                // 移动完成后，缩放到 0，消失
                picture.DOScale(0, 2f).OnComplete(() =>
                {
                    // 这里可以添加其他逻辑，比如禁用对象或其他动画
                    Debug.Log(picture.name + " 已消失");
                });
            });
    }
    void Onclick()
    {
        if (!canAnswer)
        {
            Debug.Log("请等待动画完成后再答题！");
            return;
        }
        canAnswer = false;
        // if (SetButtonsInteractable== false)
        // return;
        //   SetButtonsInteractable = false; //按钮状态变为无法按下，防止多次按下

        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>(); //获取当前点击的按钮
        Debug.Log(button.name);
        if (button.name == "Red_Button")
        {

            CheckAnswer("红色", 0);
        }
        else if (button.name == "Green_Button")
        {

            CheckAnswer("绿色", 1);

        }
        else if (button.name == "Blue_Button")
        {

            CheckAnswer("蓝色", 2);
            //TAB02.transform.DOScale(0, 0.5f);
        }
        else if (button.name == "Yellow_Button")
        {

            CheckAnswer("黄色", 3);
        }
    }

    public void CheckAnswer(string userAnswer, int anserIndex) // 定义一个方法，用于检查用户的答案
    {
        
        GetComponent<AudioSource>().PlayOneShot(bottonAudio);//播放按钮音效
        var correctAnswer = questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex];//获取正确答案


        // 根据用户选择的答案索引与正确答案索引进行比较，决定显示 "Yes" 还是 "No"
        string disCardName = anserIndex == correctAnswer ? "Yes" : "No"; // 如果用户选择的答案正确，设置为 "Yes"，否则为 "No"

        // 在 "Card" 前缀后拼接 "Yes" 或 "No"，形成完整的卡片名称
        disCardName = "Card" + disCardName;

        // 获取当前关卡中正确答案对应的选项对象
        var currentOption = options[correctAnswer];

        // 将 TAB01 的缩放设置为 (0, 0, 0)，隐藏 TAB01
        TAB01.transform.localScale = new Vector3(0, 0, 0);

        List<int> wrongOptions = new List<int>();
        for (int i = 0; i < questionOrders[currentLevel].levelConfig.Count; i++)
        {
            if (i != questionOrders[currentLevel].QuestionIndex)
                wrongOptions.Add(questionOrders[currentLevel].levelConfig[i]);
        }

        foreach (var wrongOption in wrongOptions) //隐藏错误答案

        {
            options[wrongOption].transform.Find("UnknownCard").transform.DOScale(0,0.3f);
        }
       
        // 将 UnknownTAB 的缩放设置为 (0, 0, 0)，隐藏 UnknownTAB



        // 将当前选项的缩放设置为 (1, 1, 1)，显示当前选项
        currentOption.transform.localScale = new Vector3(1, 1, 1);

        // 在当前选项中查找对应的答题卡片（"CardYes" 或 "CardNo"）
        var disCard = currentOption.transform.Find(disCardName);

        // 将找到的答题卡片的缩放设置为 1，持续 0.8 秒，显示答题卡片 YES/NO
        disCard.DOScale(1, 0.3f).OnComplete(() =>
        {
            // 在答题卡片显示完成后，延迟 2 秒后将其缩放设置为 0，隐藏答题卡片
            disCard.DOScale(0, 0f).SetDelay(0.5f);
        });

        // 调用 EndGame 方法，传入用户选择的答案索引是否与正确答案索引相同的布尔值
        EndGame(anserIndex == correctAnswer);
    }
    void EndGame(bool isRight) // 定义一个方法，用于处理游戏结束逻辑，参数 isRight 表示用户的答案是否正确
    {
        if (Gameover == false)
        {
            return;
        }

        if (isRight) // 如果用户的答案是正确的
        {
            correctAnswersCount++; // 增加正确答案计数

            var anserOption = options[questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]];//获取正确答案
            anserOption.transform.DORotate(new Vector3(0, 0, 0), 0f);//获取正确答案缩放调整为初始值
            options[questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]].transform.DOScale(1, 1f); //设置选项缩放为1，动画时间为1.5秒
        }
        else
        {
            var anserOption = options[questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]];//获取正确答案
            anserOption.transform.DORotate(new Vector3(0, 0, 0), 0f);//获取正确答案缩放调整为初始值
            options[questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]].transform.DOScale(0, 1f); //设置选项缩放为0，动画时间为1.5秒

                List<int> wrongOptions = new List<int>(); //获取错误答案
                for (int i = 0; i < questionOrders[currentLevel].levelConfig.Count; i++)
                {
                    if (i != questionOrders[currentLevel].QuestionIndex)
                        wrongOptions.Add(questionOrders[currentLevel].levelConfig[i]);//获取错误答案赋值到wrongOptions
                }

                foreach (var wrongOption in wrongOptions) //错误答案选项缩放为1
                {
                    options[wrongOption].transform.DOScale(0, 1f);
                }
                
                
        }

        DOVirtual.DelayedCall(1f, () =>
        {
            if(checkpointCount < 10)
            {
                checkpointCount++; // 增加关卡计数
            }
            correctAnswersCountDisplay(); // 更新 UI 显示正确答案的数量和关卡信息
        });

        
        if (currentLevel >= 9)
        {
            
            Endprompt.transform.DOScale(1, 0.8f); //结束提示UI显示
            accuracy.transform.DOScale(1, 0.8f); //正确率UI显示
            GetComponent<AudioSource>().PlayOneShot(pictures); //播放图片音效
            DOVirtual.DelayedCall(8f, () =>
                {
                    SceneManager.LoadScene("主页面"); //加载主页面
                });
            Debug.Log("题目答完");
            Gameover = false;
        }

        // 延迟 2 秒后执行以下代码
        DOVirtual.DelayedCall(1.5f, () =>
        {
            // 遍历所有选项
            foreach (var option in options)
            {
                option.transform.DOScale(0, 0.5f); // 将选项的缩放设置为 0，隐藏选项
                option.SetActive(false); // 将选项设置为不激活状态
            }

            // 再次延迟 1 秒后执行以下代码
            DOVirtual.DelayedCall(1f, () =>
            {
                currentLevel++; // 增加当前关卡计数
                Levelstartanimation(); // 调用方法开始下一关卡的动画
                var anserOption = options[questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]];//获取正确答案
                anserOption.transform.DORotate(new Vector3(0, 0, 0), 0f);//获取正确答案缩放调整为初始值

                List<int> wrongOptions = new List<int>(); //获取错误答案
                for (int i = 0; i < questionOrders[currentLevel].levelConfig.Count; i++)
                {
                    if (i != questionOrders[currentLevel].QuestionIndex)
                        wrongOptions.Add(questionOrders[currentLevel].levelConfig[i]);//获取错误答案赋值到wrongOptions
                }

                foreach (var wrongOption in wrongOptions) //错误答案选项缩放为1
                {
                    options[wrongOption].transform.DOScale(0, 0f);
                }

                // foreach (var wrongOption in wrongOptions) //错误答案选项缩放为1
                // {
                //     options[wrongOption].transform.Find("UnknownCard").transform.DOScale(1,0.5f);
                // }
            });
        });
    }

    // 在适当的位置调用这个方法，例如在游戏结束时


    private void correctAnswersCountDisplay() //文本UI显示
    {
        correctAnswers.text = correctAnswersCount.ToString();//正确数量显示
        checkpoint.text = checkpointCount.ToString();//关卡显示
        if (checkpointCount == 0)
            return;
        accuracy.transform.Find("Text").GetComponent<Text>().text = "您的正确率："+((float)correctAnswersCount / (float)(checkpointCount) * 100).ToString("F0") + "%";//正确率显示

        Debug.Log("正确率："+((float)correctAnswersCount / ((float)checkpointCount) * 100).ToString("F0") + "%");
    }


    void Update()
    {

    }

    public void OnDataReceived(int buttonIndex)
    {
        if (!canAnswer)
            return;
        CheckAnswer("", buttonIndex);
        canAnswer = false;
    }
}

// private void CheckAnswers(string userAnswer, int optionIndex) // 答案对比
// {

//     var correctAnswer = questionOrders[currentLevel].levelConfig[questionOrders[currentLevel].QuestionIndex]; //获取正确答案

//     // 检查用户的答案是否在正确答案列表中
//     if (optionIndex == correctAnswer) // 直接比较选项索引
//     {
//         switch (optionIndex)
//         {
//             case 0:
//                 Card1Yes.transform.position = new Vector3(Greenpicture.transform.position.x, Greenpicture.transform.position.y - 50, Greenpicture.transform.position.z);
//                 Card1Yes.transform.DOScale(1, 0.8f); // 显示 "YES"
//                 Card1NO.transform.DOScale(0, 0.8f); // 隐藏 "NO"
//                 break;
//             case 1:
//                 Card2Yes.transform.position = new Vector3(Redpicture.transform.position.x, Redpicture.transform.position.y - 50, Redpicture.transform.position.z);
//                 Card2Yes.transform.DOScale(1, 0.8f); // 显示 "YES"
//                 Card2NO.transform.DOScale(0, 0.8f); // 隐藏 "NO"
//                 break;
//             case 2:
//                 Card3Yes.transform.position = new Vector3(Bluepicture.transform.position.x, Bluepicture.transform.position.y - 50, Bluepicture.transform.position.z);
//                 Card3Yes.transform.DOScale(1, 0.8f); // 显示 "YES"
//                 Card3NO.transform.DOScale(0, 0.8f); // 隐藏 "NO"
//                 break;
//             case 3:
//                 Card4Yes.transform.position = new Vector3(Yellowpicture.transform.position.x, Yellowpicture.transform.position.y - 50, Yellowpicture.transform.position.z);
//                 Card4Yes.transform.DOScale(1, 0.8f); // 显示 "YES"
//                 Card4NO.transform.DOScale(0, 0.8f); // 隐藏 "NO"
//                 break;

//         }
//         Debug.Log("答案正确！");
//         correctAnswersCount++; // 增加正确答案计数
//         // 检查是否达到通关条件
//         if (correctAnswersCount >= requiredCorrectAnswers)
//         {
//             Debug.Log("通关！你答对了足够的题目。");

//             checkpointCount++;//关卡++

//             checkpoint.transform.DOScale(1, 0.8f); //关卡UI显示
//             Nextquestion.transform.DOScale(1, 0.8f); //下一题UI显示
//             DOVirtual.DelayedCall(2f, () =>
//             { //延迟2s
//                 Nextquestion.transform.DOScale(0, 0.8f);
//             });
//         }
//     }
//     else
//     {
//         switch (optionIndex)
//         {
//             case 0:
//                 Card1NO.transform.position = new Vector3(Greenpicture.transform.position.x, Greenpicture.transform.position.y - 50, Greenpicture.transform.position.z);
//                 Card1NO.transform.DOScale(1, 0.8f); // 显示 "NO"
//                 Card1Yes.transform.DOScale(0, 0.8f); // 隐藏 "YES"
//                 break;
//             case 1:
//                 Card2NO.transform.position = new Vector3(Redpicture.transform.position.x, Redpicture.transform.position.y - 50, Redpicture.transform.position.z);
//                 Card2NO.transform.DOScale(1, 0.8f); // 显示 "NO"
//                 Card2Yes.transform.DOScale(0, 0.8f); // 隐藏 "YES"
//                 break;
//             case 2:
//                 Card3NO.transform.position = new Vector3(Bluepicture.transform.position.x, Bluepicture.transform.position.y - 50, Bluepicture.transform.position.z);
//                 Card3NO.transform.DOScale(1, 0.8f); // 显示 "NO"
//                 Card3Yes.transform.DOScale(0, 0.8f); // 隐藏 "YES"
//                 break;
//             case 3:
//                 Card4NO.transform.position = new Vector3(Yellowpicture.transform.position.x, Yellowpicture.transform.position.y - 50, Yellowpicture.transform.position.z);
//                 Card4NO.transform.DOScale(1, 0.8f); // 显示 "NO"
//                 Card4Yes.transform.DOScale(0, 0.8f); // 隐藏 "YES"
//                 break;
//         }
//         Debug.Log("答案错误！");
//         TAB01.transform.DOScale(0, 1.5f);
//         Greenpicture.transform.DOScale(1, 1.5f);
//         FlyOutAndDisappear(Greenpicture);
//         Card1NO.transform.DOScale(1, 0.8f);

//         DOVirtual.DelayedCall(2f, () =>
//         { //延迟2s
//             Card1NO.transform.DOScale(0, 0.8f);
//         });

//         SetButtonsInteractable = false;
//     }
// }