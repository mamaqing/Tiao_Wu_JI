
using UnityEngine;

public class DontDestoryOnScenceload : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if( GameObject.Find(name) != gameObject )
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
