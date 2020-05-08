using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static bool TutorialOn = true;
    public List<GameObject> MessageList;


    void Awake()
    {
        Random.InitState(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialOn)
        {
            Time.timeScale = 0;
            
        }
        else
        {
            Time.timeScale = 1;
            
        }
    }

    public void LoadNextMessage(int currentMessage)
    {

        
        if (currentMessage+1 < MessageList.Count)
        {
        MessageList[currentMessage + 1].GetComponent<TutorialMessageControl>().activate = true;
        }
        
        
    }
}
