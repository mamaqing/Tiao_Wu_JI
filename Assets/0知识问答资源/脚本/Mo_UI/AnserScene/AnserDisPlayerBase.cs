using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnserDisPlayerBase : MonoBehaviour
{
    // Start is called before the first frame update
    protected AnserSceneManager anserSceneManager;
    protected virtual void Start()
    {
        anserSceneManager = FindObjectOfType<AnserSceneManager>();
        if (anserSceneManager == null)
        return;
        anserSceneManager.OnAnserQuestion += OnAnserQuestion;
    }

    protected virtual void OnAnserQuestion(AnserQuestion anserQuestion, bool isRight)
    {

    }
    private void OnDestroy() {
        if (anserSceneManager != null)
        {
            anserSceneManager.OnAnserQuestion -= OnAnserQuestion;
        }
      
    }
}
