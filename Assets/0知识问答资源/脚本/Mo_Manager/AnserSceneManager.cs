using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using System.Threading;
using UnityEngine.SceneManagement;
[Serializable]
public class AnserQuestion
{
    public string question;
    public string[] anser;
    public int rightAnserIndex;
}
public class AnserSceneManager : MonoBehaviour, _ISerialPortInput
{
    // Start is called before the first frame update
    [SerializeField] Text TitleText;
    [SerializeField] Text LevelTimeText;
    [SerializeField] float LevelTimeDown = 25f;
    [SerializeField] Button[] buttons;
    [SerializeField] GameObject EndPanel;
    [SerializeField] AnserQuestion[] anserQuestions; //题目数组
    [SerializeField] AudioClip[] anserAudioClips;
    [HideInInspector] public int score = 0;
    [HideInInspector] public int LevelIndex = 0;
    List<AnserQuestion> currentQuestions = new List<AnserQuestion>();
    bool canAnser = false;
    public UnityAction<AnserQuestion, bool> OnAnserQuestion;
    public UnityAction<AnserQuestion> OnLevelStart;
    CancellationTokenSource LevelTimeDownTokenSource;
    void Start()
    {
        GameStartInit().Forget();
    }
    async UniTaskVoid GameStartInit()
    {
        await UniTask.Delay(1);
        RandomQuestion();
        ButtonInit();
        QuestionDisplayInit();
    }
    void RandomQuestion()
    {
        List<AnserQuestion> tempQuestions = new List<AnserQuestion>(anserQuestions);

        // 随机打乱列表
        for (int i = tempQuestions.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            AnserQuestion temp = tempQuestions[i];
            tempQuestions[i] = tempQuestions[randomIndex];
            tempQuestions[randomIndex] = temp;
        }
        currentQuestions.Clear();  // 清空当前列表
        currentQuestions.AddRange(tempQuestions);
    }
    void ButtonInit()
    {
        foreach (var item in buttons)
        {
            item.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == button.name)
                AnserQuestion(i);
        }
    }

    void QuestionDisplayInit()
    {
        TitleTextLevelInit();
        buttonLevelInit();
        ContDownInit();
        canAnser = true;
        OnLevelStart?.Invoke(currentQuestions[LevelIndex]);
    }
    void ContDownInit()
    {
        LevelTimeDown = 25f;
        LevelTimeDownTokenSource = new CancellationTokenSource();
        var _LinkTokenSource = CancellationTokenSource.CreateLinkedTokenSource(LevelTimeDownTokenSource.Token, this.GetCancellationTokenOnDestroy());
        LevelCountDown(_LinkTokenSource.Token).Forget();
    }
    void TitleTextLevelInit()
    {
        var text = LevelIndex + 1 + "." + currentQuestions[LevelIndex].question;
        TitleText.text = "";
        this.GetComponent<AudioSource>().PlayOneShot(anserAudioClips[3]);
        TitleText.DOText(text, 1f)  // 1.5秒内完成显示
        .SetEase(Ease.Linear)  // 使用线性缓动
        .SetUpdate(true);
    }
    void buttonLevelInit()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<Text>().text = currentQuestions[LevelIndex].anser[i];
            buttons[i].image.DOColor(Color.white, 0f);
            var initPosX = buttons[i].transform.position.x;
            buttons[i].transform.DOMoveX(initPosX, 0.6f).From(initPosX - 10f).SetEase(Ease.OutSine).SetDelay(0.05f * i);
        }
    }
    async UniTaskVoid LevelCountDown(CancellationToken token)
    {
        LevelTimeText.text = LevelTimeDown.ToString();
        while (LevelTimeDown > 0)
        {
            LevelTimeDown -= 1;
            bool isCancel = await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token).SuppressCancellationThrow();
            if (isCancel)
                return;
            LevelTimeText.text = LevelTimeDown.ToString();
        }
        AnserQuestion(currentQuestions[LevelIndex].rightAnserIndex + 10);
    }
    // Update is called once per frame
    void AnserQuestion(int buttonIndex)
    {
        if (!canAnser)
            return;
        canAnser = false;

        bool isRight = currentQuestions[LevelIndex].rightAnserIndex == buttonIndex;
        score += isRight ? 10 : 0;
        this.GetComponent<AudioSource>().PlayOneShot(anserAudioClips[isRight ? 0 : 1]);
        OnAnserQuestion?.Invoke(currentQuestions[LevelIndex], isRight);
        ButtonOnAnserAnimation(buttonIndex, isRight);
        LevelEnd(isRight).Forget();
    }
    void ButtonOnAnserAnimation(int buttonIndex, bool isRight)
    {
        if (buttonIndex < 0 || buttonIndex >= buttons.Length)
            return;
        buttons[buttonIndex].transform.DOScale(0.95f, 0.1f);
        buttons[buttonIndex].transform.DOScale(1f, 0.1f).SetDelay(0.1f);
        buttons[buttonIndex].image.DOColor(isRight ? Color.green : Color.red, 0.5f);
    }
    async UniTaskVoid LevelEnd(bool isRight)
    {
        LevelTimeDownTokenSource.Cancel();
        var AnimationTime = isRight ? 1.5f : 3f;
        await UniTask.Delay(TimeSpan.FromSeconds(AnimationTime)); //等待动画完成
        LevelIndex++;
        if (LevelIndex >= anserQuestions.Length)
        {
            GameOver().Forget();
            return;
        }
        QuestionDisplayInit();
    }
    async UniTaskVoid GameOver()
    {
        this.GetComponent<AudioSource>().PlayOneShot(anserAudioClips[2]);
        var text = EndPanel.GetComponentInChildren<Text>();
        text.text = "认知得分";
        DOTween.To(() => 0, x => text.text = "认知得分：" + x.ToString(), score, 1f);
        EndPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            EndPanel.transform.DOScale(1.05f, 0.6f).SetEase(Ease.InOutSine).SetLoops(8, LoopType.Yoyo);
        });
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        SceneManager.LoadScene("主页面");
    }
    public void OnDataReceived(int buttonIndex)
    {
        AnserQuestion(buttonIndex);
    }
}
