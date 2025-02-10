using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RightAnserDis : AnserDisPlayerBase 
{
    // Start is called before the first frame update
    Text RightText;
    protected override void Start()
    {
        base.Start();
        RightText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    protected override void OnAnserQuestion(AnserQuestion anserQuestion, bool isRight)
    {
        if (!isRight)
        {
            RightText.text = anserQuestion.anser[anserQuestion.rightAnserIndex];
            transform.DOScale(1f, 0.5f);
            transform.DOScale(0f, 0.5f).SetDelay(2f);
        }
    }
}
