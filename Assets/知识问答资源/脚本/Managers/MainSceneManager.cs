using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour, _ISeriportInput
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnDestroy()
    {
        
    }

    public void OnDataReceived(int buttonIndex)
    {
       SceneManager.LoadScene(buttonIndex+1);
    }
}
