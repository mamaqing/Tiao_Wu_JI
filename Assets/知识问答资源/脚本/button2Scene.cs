using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class button2Scene : MonoBehaviour
{
    // Start is called before the first frame update
    Button button01;
    [SerializeField] int sceneIndex;
    
    void Start()
    {
       button01 =  this.GetComponent<Button>();
      
       button01.onClick.AddListener(Change2TargetScene);
    }

    void Change2TargetScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
    // Update is called once per frame
 
}
