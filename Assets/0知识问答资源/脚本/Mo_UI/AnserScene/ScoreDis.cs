using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ScoreDis : AnserDisPlayerBase
{
    // Start is called before the first frame update
    Text ScoreText;
    [SerializeField] GameObject rightFx;
    protected override void Start()
    {
        base.Start();
        ScoreText = GetComponentInChildren<Text>();
        ScoreText.text = "0";
    }

    // Update is called once per frame
    protected override void OnAnserQuestion(AnserQuestion anserQuestion, bool isRight)
    {
        if (isRight)
        {
            ScoreDisAnimation();
        }
    }
    void ScoreDisAnimation()
    {

        var curScore = int.Parse(ScoreText.text);
        var targetScore = anserSceneManager.score;
        transform.DOScale(1.03f, 0.5f).From(1f).SetDelay(0.5f);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            var fx = Instantiate(rightFx, transform.position, Quaternion.identity);
            fx.transform.localScale = 2f * Vector3.one;
            Destroy(fx, 2f);
        });
        transform.DOScale(1f, 0.5f).SetDelay(1.2f);
        DOTween.To((float value) =>
        {
            ScoreText.text = ((int)value).ToString();
        }, curScore, targetScore, 1f).SetDelay(0.5f);
    }
}
