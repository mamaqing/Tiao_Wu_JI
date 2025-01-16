using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SerialPortSetPanel : MonoBehaviour
{
    // Start is called before the first frame update
    InputField portInputField;
    InputField baudRateInputField;
    Button confirmButton;
    SerialPortUtilityManager serialPortUtilityManager;
    void Start()
    {
        portInputField = transform.Find("PortInputField").GetComponent<InputField>();
        baudRateInputField = transform.Find("BaudRateInputField").GetComponent<InputField>();
        confirmButton = transform.Find("ConfirmButton").GetComponent<Button>();
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        serialPortUtilityManager = FindObjectOfType<SerialPortUtilityManager>();
        serialPortUtilityManager.RegisterSerialPortSetPanel(true);
       // SerialPortUtilityManager.Instance.RegisterSerialPortSetPanel(true);
    }

    private void OnClickConfirmButton()
    {
        if (int.TryParse(baudRateInputField.text, out int baudRate))
        {
            serialPortUtilityManager.SetBaudRate(baudRate);
        }
        if (int.TryParse(portInputField.text, out int port))
        {
            serialPortUtilityManager.SetComPortName(port);
        }
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        serialPortUtilityManager.RegisterSerialPortSetPanel(false);
    }

}
