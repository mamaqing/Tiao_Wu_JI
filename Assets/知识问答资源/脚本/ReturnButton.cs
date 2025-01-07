using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour, _ISeriportInput
{
    List<int> clickedbuttonIndex = new List<int>();
    float timeout = 0;
    // Start is called before the first frame update

    private void Update() { //更新函数
        if(Time.time - timeout > 1) //如果时间大于1秒
        {
            clickedbuttonIndex.Clear(); //清空列表
        }
        if(clickedbuttonIndex.Count == 2)
        {
            SceneManager.LoadScene(0);//返回主菜单
        }
    }
    // Update is called once per frame
   
    public void OnDataReceived(int buttonIndex) //串口输入时处理函数
    {
        if(clickedbuttonIndex.Contains(buttonIndex)) //如果列表中包含这个索引   
        {
            return;
        }
        clickedbuttonIndex.Add(buttonIndex); //添加到列表
        timeout = Time.time; //设置时间
    }

}
