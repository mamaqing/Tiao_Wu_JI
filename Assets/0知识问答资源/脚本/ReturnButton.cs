using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour, _ISerialPortInput
{
    List<int> clickedbuttonIndex = new List<int>();
    float timeout = 0;
    // Start is called before the first frame update
    private void Update()
    { //更新函数
        if (Time.time - timeout > 2f) //如果时间大于2秒
        {
            clickedbuttonIndex.Clear(); //清空列表
        }
        checkButton();
    }
    // Update is called once per frame

    public void OnDataReceived(int buttonIndex) //串口输入时处理函数
    {
        if (clickedbuttonIndex.Contains(buttonIndex)) //如果列表中包含这个索引   
        {
            return;

        }
        clickedbuttonIndex.Add(buttonIndex); //添加到列表
        timeout = Time.time; //设置时间
    }
    public void checkButton()
    {
        if (clickedbuttonIndex.Count == 4)
        {
            SceneManager.LoadScene(0);
        }
    }
}
