using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SeriportManager.instance.OnDataReceived += OnDataReceived;
    }

    private void OnDataReceived(byte[] recivedBytes)
    {
        if (recivedBytes.Length < 8)

            return;
       // Debug.Log("recivedBytes[7]: " + recivedBytes[4]-48);
        SceneManager.LoadScene(recivedBytes[7]);

    }
    void OnDestroy()
    {
        SeriportManager.instance.OnDataReceived -= OnDataReceived;
    }
}
