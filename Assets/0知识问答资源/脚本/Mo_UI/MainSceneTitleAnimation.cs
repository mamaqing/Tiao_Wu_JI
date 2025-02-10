using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainSceneTitleAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Image titleImage;
    Sequence shineTweener;
    private void Awake()
    {
        titleImage = GetComponent<Image>();
        titleImage.transform.DOScale(1f, 0.5f).From(0f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            TileAnimation();
        });
    }

    void TileAnimation()
    {
        Material material = titleImage.material;
        Sequence shineSequence = DOTween.Sequence();

        // 添加闪光动画
        shineSequence.Append(DOTween.To(() => material.GetFloat("_ShineLocation"),
            x => material.SetFloat("_ShineLocation", x),
            1f, 2f)
            .SetEase(Ease.Linear));

        // 添加等待时间
        shineSequence.AppendInterval(4f);

        // 设置整个序列循环
        shineSequence.SetLoops(-1, LoopType.Restart);

        // 保存引用以便后续可以控制
        shineTweener = shineSequence;
        titleImage.transform.DOScale(1.02f, 1.5f).From(1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    void OnDestroy()
    {
        shineTweener.Restart();
        shineTweener.Kill();
        titleImage.transform.DOKill();
    }
}
