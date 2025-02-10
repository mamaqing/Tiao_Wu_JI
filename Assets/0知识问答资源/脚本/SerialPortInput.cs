using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface _ISerialPortInput //串口输入接口
{
    void OnDataReceived(int buttonIndex); //串口输入时处理函数
}

public class SerialPortInput : MonoBehaviour
{
    // Start is called before the first frame update
    SerialPortUtilityManager serialPortUtilityManager;
    SeriportManager seriportManager;
    void Start()
    {
    //    serialPortUtilityManager = FindObjectOfType<SerialPortUtilityManager>();
        seriportManager = FindObjectOfType<SeriportManager>();
        if (seriportManager != null)
        {
            seriportManager.OnDataReceived += OnDataReceived;
        }
        // if (serialPortUtilityManager == null)
        //     return;
        // serialPortUtilityManager.OnSerialDataReceived += OnDataReceived;
    }

    private void OnDataReceived(byte[] recivedBytes)
    {
        if (recivedBytes.Length < 8)
            return;
        var serialPortInput = GetComponent<_ISerialPortInput>();
        if (serialPortInput != null)
        {
            serialPortInput.OnDataReceived(recivedBytes[7] - 1);
        }
    }
    private void OnDestroy()
    {
        // if (serialPortUtilityManager != null)
        // {
        //     serialPortUtilityManager.OnSerialDataReceived -= OnDataReceived;
        // }
        if (seriportManager != null)
        {
            seriportManager.OnDataReceived -= OnDataReceived;
        }
    }
    // Update is called once per fra
}

