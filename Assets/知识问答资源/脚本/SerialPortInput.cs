using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface _ISeriportInput //串口输入接口
{
    void OnDataReceived(int buttonIndex); //串口输入时处理函数
}

public class SerialPortInput : MonoBehaviour
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
        GetComponent<_ISeriportInput>().OnDataReceived(recivedBytes[7]-1);

    }
    private void OnDestroy() {
        SeriportManager.instance.OnDataReceived -= OnDataReceived;
    }
    // Update is called once per fra
}
