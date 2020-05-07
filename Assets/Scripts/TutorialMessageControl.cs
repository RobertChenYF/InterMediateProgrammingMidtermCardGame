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
   




    //public int coinCounts;


    void Start()
    {
        textBoxUI = textBoxContainer.GetComponent<RectTransform>();
        Canvas = messageCanvas.GetComponent<RectTransform>();
        messageCanvas.enabled = false;//turn off canvas at the beginning


        textLines = (textFile1.text.Split('\n'));//splitting the text file in to lines
        if (endLineAt == 0)
        {
            endLineAt = textLines.Length - 1; // set the number for the last line by how many lines the file has
        }

    }


    void Update()
    {
        if (activate)
        {
            TurnOnMessage();
        }
        else
        {
            TurnOffMessage();
        }
        if (messageCanvas.enabled == true)
        {
            if (activate)
            {
                
                Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
                Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * Canvas.sizeDelta.x) - (Canvas.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * Canvas.sizeDelta.y) - (Canvas.sizeDelta.y * 0.5f))); //fix the UI problem code credit to unity forum

                //now you can set the position of the ui element
                textBoxUI.anchoredPosition = WorldObject_ScreenPosition;
            }

            //massage1.text = textLines[currentLine];


            if ((Input.GetMouseButtonDown(0) && activate))
            {
                if (currentLine > endLineAt)
                {
                    
                    messageCanvas.enabled = false;
                    activate = false;
                    currentLine = 0;
                    return;
                }

               
               // StopAllCoroutines();
                fullText = textLines[currentLine];
                massage1.text = fullText;
                //StartCoroutine(TypeWriter());
                currentLine += 1;
            }
        }


    }

    private void TurnOnMessage()
    {
        
        messageCanvas.enabled = true;

    }

   

    private void TurnOffMessage()
    {
        messageCanvas.enabled = false; //function for disable the canvas
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

  
}
