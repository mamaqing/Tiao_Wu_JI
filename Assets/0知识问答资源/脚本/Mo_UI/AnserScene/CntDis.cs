using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CntDis : AnserDisPlayerBase
{
    // Start is called before the first frame update
    Text CntText;
    [SerializeField] bool isRightDisplay = false;
    protected override void Start()
    {
        base.Start();
        CntText = GetComponentInChildren<Text>();
        CntText.text = "0";
    }

    // Update is called once per frame
    protected override void OnAnserQuestion(AnserQuestion anserQuestion, bool isRight)
    {
        if (isRightDisplay == isRight)
        {
            transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo);
            if (isRight)
            {
                CntText.text = anserSceneManager.score / 10 + "";
            }
            else
            {
                CntText.text = anserSceneManager.LevelIndex - (anserSceneManager.score / 10 - 1) + "";
            }
        }
    }
}

