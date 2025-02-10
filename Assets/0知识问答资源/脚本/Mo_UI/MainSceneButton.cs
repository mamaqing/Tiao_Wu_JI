using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MainSceneButton : MonoBehaviour
{
    // Start is called before the first frame update
    public float delayTime = 0f;
    public int buttonIndex;
    Button button;
    MainSceneManager mainSceneManager;
    void Start()
    {
        button = GetComponent<Button>();
        mainSceneManager = FindObjectOfType<MainSceneManager>();
        mainSceneManager.OnStartGame += OnStartGame;
        StartAnimation();
    }

    // Update is called once per frame
    void StartAnimation()
    {
        float startY = button.transform.position.y;
        button.transform.DOMoveY(startY + 0.05f, 0.9f)
        .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(delayTime);
    }
    void OnStartGame(int buttonIndex)
    {
        if (this.buttonIndex == buttonIndex)
        {
            button.transform.DOKill();
            Sequence buttonSequence = DOTween.Sequence();

            // 按下效果：缩小并变色
            buttonSequence.Append(button.transform.DOScale(0.9f, 0.2f));
            buttonSequence.Join(button.image.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.2f));

            // 恢复效果：返回原始大小和颜色
            buttonSequence.Append(button.transform.DOScale(1f, 0.2f));
            buttonSequence.Join(button.image.DOColor(Color.white, 0.2f));
        }
    }
    private void OnDestroy()
    {
        button.transform.DOKill();
    }
}
