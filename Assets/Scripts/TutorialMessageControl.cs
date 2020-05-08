using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialMessageControl : MonoBehaviour
{
    RectTransform Canvas;
    RectTransform textBoxUI;

    public Canvas messageCanvas;
    public GameObject textBoxContainer;

    public Text massage1;

    public TextAsset textFile1;

    private string[] textLines;

    private int currentLine;//the line counter
    private int endLineAt;//the number for the last line

    private float typeWriterDelay = 0.02f;
    private string fullText;
    private string currentText;

    public bool activate = false;
    public int messageCode;
    public float delayBeforeNextMessage;



    //public int coinCounts;


    void Start()
    {
        

    }

    

    void Update()
    {
        if (activate)
        {
            textBoxUI = textBoxContainer.GetComponent<RectTransform>();
            Canvas = messageCanvas.GetComponent<RectTransform>();
            messageCanvas.enabled = true;


            textLines = (textFile1.text.Split('\n'));//splitting the text file in to lines
            if (endLineAt == 0)
            {
                endLineAt = textLines.Length - 1; // set the number for the last line by how many lines the file has
            }
            
        }
        

        if (messageCanvas.enabled == true)
        {
            if (activate)
            {
                TutorialManager.TutorialOn = true;
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
                Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * Canvas.sizeDelta.x) - (Canvas.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * Canvas.sizeDelta.y) - (Canvas.sizeDelta.y * 0.5f))); //fix the UI problem code credit to unity forum

                //now you can set the position of the ui element
                textBoxUI.anchoredPosition = WorldObject_ScreenPosition;
                
                
                 if ((Input.GetMouseButtonDown(0)))
            {
                if (currentLine > endLineAt-1)
                {
                    
                    messageCanvas.enabled = false;
                    activate = false;
                    currentLine = 0;
                    TutorialManager.TutorialOn = false;

                    Invoke("LoadNextMessage",delayBeforeNextMessage);

                       
                }

               
               
                
                
                currentLine += 1;
            }
               
               fullText = textLines[currentLine];
                massage1.text = fullText;
            }

            


           
        }


    }

 

    private IEnumerator TypeWriter()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            massage1.text = currentText;
            yield return new WaitForSeconds(typeWriterDelay);
        }
    }

  private void LoadNextMessage()
    {
        GameObject.Find("TutorialManager").GetComponent<TutorialManager>().LoadNextMessage(messageCode);
    }
}
