using ArduinoSerialAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SeriportManager : MonoBehaviour
{
    public static SeriportManager instance;
    [SerializeField] string Com = "COM5"; // 序列化字段，用于在外面面板中设置串口名称
    SerialHelper helper; // 声明SerialHelper对象，用于管理串口通信
    public UnityAction<byte[]> OnDataReceived;  // 声明一个UnityAction，用于在其他地方调用
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        SeriportInit(); // 在Start方法中调用SeriportInit，初始化串口
    }

    void SeriportInit()
    {
        try
        {
            helper = SerialHelper.CreateInstance(Com,115200); // 创建SerialHelper实例，使用指定的COM端口
            helper.setTerminatorBasedStream("~");  // 设置终止符为换行符（ASCII码0x0A） 相当于帧尾  接收到就把之前的数据拼接起来 当做一帧 并执行下面的OnDataReceived

            helper.OnConnected += () =>
            {
                SerilShowLog("串口已连接"); // 当串口连接成功时，记录日志
                helper.SendData("Sstart\n"); // 发送启动命令到串口
            };

            helper.OnConnectionFailed += () =>
            {
                SerilShowLog("串口连接失败"); // 当串口连接失败时，记录日志
            };

            helper.OnDataReceived += () =>
            {
                //TypeEventSystem.Global.Send<AndroSeriPortEvent>(new AndroSeriPortEvent(helper.ReadBytes())); // 注释掉的代码，可能用于发送接收到的数据事件
                byte[] receivedBytes = helper.ReadBytes();   //转化成字节数组
                string byteString = BitConverter.ToString(receivedBytes); // 将字节数组转换为字符串
                OnDataReceived?.Invoke(receivedBytes); // 调用OnDataReceived事件，传递接收到的字节数组
                Debug.Log("Received bytes: " + byteString); // 将接收到的字节数组打印出来
            };  //lenda表达式 当接收到上面设置的终止符时，执行里面的函数

            helper.OnPermissionNotGranted += () =>
            {
                SerilShowLog("没有权限"); // 当没有足够的权限时，记录日志
            };

            helper.Connect(); // 尝试连接串口
        }
        catch (Exception ex)
        {
            SerilShowLog(ex.ToString()); // 如果在初始化过程中发生异常，记录异常信息
        }
    }

    void SerilShowLog(String log, float duration = 2f)
    {
        //MessageboxTool.ShowInfomation(log,duration); // 注释掉的代码，可能用于显示消息框
        Debug.Log(log); // 将日志信息打印到Unity控制台
    }
}
