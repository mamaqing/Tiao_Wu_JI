using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using DG.Tweening;
public class MainSceneManager : MonoBehaviour, _ISerialPortInput
{
    // Start is called before the first frame update
    public UnityAction<int> OnStartGame;
    void Start()
    {

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           OnDataReceived(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnDataReceived(1);
        }
    }
    void OnDestroy()
    {

    }

    public void OnDataReceived(int buttonIndex)
    {
        OnStartGame?.Invoke(buttonIndex);
        DOVirtual.DelayedCall(0.5f, () => SceneManager.LoadScene(buttonIndex + 1));
    }
}
