using System.Collections;
using UnityEngine;
using SerialPortUtility;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System;
public class SerialPortUtilityManager : MonoBehaviour
{
    // Start is called before the first frame update
    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int spapDeviceListAvailable();

    [DllImport("spap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int spapDeviceList(int deviceNum, [MarshalAs(UnmanagedType.LPStr)] StringBuilder deviceInfo, int buffer_size);
    [SerializeField] int ComPortName;
    public UnityAction<byte[]> OnSerialDataReceived;
    SerialPortUtilityPro serialPortUtilityPro;
    [HideInInspector] public bool serialPortSetPanelIsActive = false;
    bool isHandshake = false;
    int currentComPort;
    public int BaudRate = 115200;
    public int HANDSHAKE_TIMEOUT = 2;  // 握手超时时间
    public int PORT_OPEN_TIMEOUT = 1;  // 打开串口超时时间
    public byte[] HANDSHAKE_DATA = new byte[] { 0x77, 0x73, 0x3A, 0x0A };
    void Start()
    {
        serialPortUtilityPro = GetComponent<SerialPortUtilityPro>();
        ComPortInit().Forget();
    }
    async UniTaskVoid ComPortInit()
    {
        serialPortUtilityPro.OpenMethod = SerialPortUtilityPro.OpenSystem.NumberOrder;
        currentComPort = PlayerPrefs.GetInt("ComPortName", 0);
        if (currentComPort != 0 && await TryConnectPort($"COM{currentComPort}"))
        {
            Debug.Log("串口初始化成功");
        }
        else
        {
            PlayerPrefs.SetInt("ComPortName", 0);
            Debug.Log(currentComPort == 0 ? "无保存串口" : "串口初始化失败");
            await ScanComPort();
        }
    }
    async UniTask ScanComPort()
    {
        try
        {
            // 获取所有COM口
            var comPorts = await GetComPorts();
            if (comPorts.Count == 0)
            {
                Debug.Log("未找到任何串口");
                return;
            }
            // 测试每个COM口
            foreach (string comPort in comPorts)
            {
                Debug.Log($"正在测试串口: {comPort}");
                if (await TryConnectPort(comPort))
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"扫描串口时发生错误: {e}");
        }
    }

    async UniTask<List<string>> GetComPorts()
    {
        List<string> comPorts = new List<string>();
        int deviceNum = spapDeviceListAvailable();

        await UniTask.Yield();
        for (int i = 0; i < deviceNum; i++)
        {
            StringBuilder deviceInfo = new StringBuilder(1024);
            spapDeviceList(i, deviceInfo, 1024);
            string portInfo = deviceInfo.ToString();

            if (portInfo.Contains("COM"))
            {
                string comPort = portInfo.Substring(portInfo.IndexOf("COM"));
                if (comPort.Length < 6)
                {
                    comPorts.Add(comPort);
                    Debug.Log($"找到串口: {comPort}");
                }

            }
        }

        return comPorts;
    }
    async UniTask<bool> TryConnectPort(string comPort)
    {
        try
        {
            // 添加握手数据接收监听
            serialPortUtilityPro.ReadCompleteEventObject.AddListener(HandshakeDataReceived);
            // 尝试打开串口
            Debug.Log($"尝试打开串口: {comPort}");
            if (!await TryOpenPort(comPort))
            {
                return false;
            }
            // 尝试握手
            if (await TryHandshake())
            {
                PlayerPrefs.SetInt("ComPortName", int.Parse(comPort[3..]));
                OnConnectSuccess(comPort);
                return true;
            }
            Debug.Log($"串口 {comPort} 达到最大重试次数");
            return false;
        }
        finally
        {
            serialPortUtilityPro.ReadCompleteEventObject.RemoveListener(HandshakeDataReceived);
        }
    }
    async UniTask<bool> TryOpenPort(string comPort)
    {
        try
        {
            serialPortUtilityPro.Skip = int.Parse(comPort[3..]);
            serialPortUtilityPro.BaudRate = BaudRate;
            serialPortUtilityPro.Open();
            // 等待串口打开，带超时
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(PORT_OPEN_TIMEOUT));
            try
            {
                await UniTask.WaitUntil(
                    () => serialPortUtilityPro.IsOpened(),
                    cancellationToken: cts.Token
                );
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"串口 {comPort} 打开超时");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"串口 {comPort} 打开失败: {e.Message}");
            return false;
        }
    }
    async UniTask<bool> TryHandshake()
    {
        isHandshake = false;
        serialPortUtilityPro.Write(HANDSHAKE_DATA);

        // 等待握手响应，带超时
        var _cts = new CancellationTokenSource();
        _cts.CancelAfterSlim(TimeSpan.FromSeconds(HANDSHAKE_TIMEOUT));
        var _linkCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, this.GetCancellationTokenOnDestroy());
        try
        {
            await UniTask.WaitUntil(
                () => isHandshake,
                cancellationToken: _linkCancellationToken.Token
            );
            return true;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("握手超时");
            return false;
        }
    }


    void HandshakeDataReceived(object data)
    {

        byte[] bytes = (byte[])data;
        Debug.Log("握手数据接收" + bytes[0] + "原始数据" + HANDSHAKE_DATA[0]);
        if (bytes.Length >= 3 &&
        bytes[0] == HANDSHAKE_DATA[0] &&
        bytes[1] == HANDSHAKE_DATA[1] &&
        bytes[2] == HANDSHAKE_DATA[2])
        {
            isHandshake = true;
        }
    }

    void OnDataReceived(object data)
    {
        byte[] bytes = (byte[])data;
        string hexString = BitConverter.ToString(bytes).Replace("-", " ");
        Debug.Log("数据接收(HEX): " + hexString);
        OnSerialDataReceived?.Invoke(bytes);
    }
    void OnConnectSuccess(string comPort)
    {
        serialPortUtilityPro.ReadCompleteEventObject.AddListener(OnDataReceived);
        Debug.Log($"串口 {comPort} 初始化成功");
    }

    public void SetBaudRate(int baudRate)
    {
        if (serialPortSetPanelIsActive == false)
            return;
        serialPortUtilityPro.Close();
        serialPortUtilityPro.BaudRate = baudRate;
        PlayerPrefs.SetInt("BaudRate", baudRate);
        serialPortUtilityPro.Open();
    }
    public void SetComPortName(int comPortName)
    {
        if (serialPortSetPanelIsActive == false)
            return;
        serialPortUtilityPro.Close();
        serialPortUtilityPro.Skip = comPortName;
        PlayerPrefs.SetInt("ComPortName", comPortName);
        serialPortUtilityPro.Open();
    }
    public void RegisterSerialPortSetPanel(bool isActive)
    {
        serialPortSetPanelIsActive = isActive;
    }
    // Update is called once per frame

}
