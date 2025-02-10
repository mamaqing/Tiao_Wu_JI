using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class RightFxDis : AnserDisPlayerBase
{
    // Start is called before the first frame update\
    Animator animator;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void OnAnserQuestion(AnserQuestion anserQuestion, bool isRight)
    {
        if (isRight)
        {
            animator.SetInteger("RightAnserIndex", anserQuestion.rightAnserIndex+1);
            DOVirtual.DelayedCall(0.5f, () => animator.SetInteger("RightAnserIndex", 0));
        }
    }
}

