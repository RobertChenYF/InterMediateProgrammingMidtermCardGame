using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CloseButtonController : MonoBehaviour //close the currenly open window the display all cards
{

    public Button CloseButton;
    // Start is called before the first frame update
    void Start()
    {
        CloseButton = GetComponent<Button>();
        CloseButton.onClick.AddListener(CloseCurrentWindow);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseCurrentWindow()
    {
        GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CloseWindow();
        Debug.Log("Close Window");
    }
}
