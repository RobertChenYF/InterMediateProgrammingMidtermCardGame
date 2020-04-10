using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCards : MonoBehaviour
{
    
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color white;
    [SerializeField] private Color blue;
    private GameObject gameStateManager;
    private Color currentColor;
    private Color unhighlishtedColor;
    public int Suit;
    public int Ranking;
    public int cardCode;
    public Sprite cardImage;
    public int orderInLayer;
    private Vector3 lerpLocation;
    private bool Move = false;
    private float lerpFloat = 0;
    private Vector3 CurrentPosition;
    public int CurrentCol = -2;
    public bool readyToBePickedByDummy = false;
    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = GameObject.Find("GameStateManager");
        GetComponent<SpriteRenderer>().sprite = cardImage;
        unhighlishtedColor = white;
        currentColor = unhighlishtedColor;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;

        if (readyToBePickedByDummy)
        {
            unhighlishtedColor = blue;
        }
        else
        {
            unhighlishtedColor = white;
        }
        if (Move)
        {
            if (lerpFloat <= 1)
            {
                transform.position = Vector3.Lerp(CurrentPosition,lerpLocation,lerpFloat);
                lerpFloat += Time.deltaTime;
            }
            else
            {
                Move = false;
            }
            
        }

        if (GameStateManager.canInteract && CurrentCol >= 0 && Ranking != 13)
        {
            if (CurrentCol == GameStateManager.highlightCol && Suit == GameStateManager.highlightSuit)
            {
                currentColor = highlightColor;
            }
            else currentColor = unhighlishtedColor;
        }
        



        GetComponent<SpriteRenderer>().color = currentColor;
        
    }
    private void OnMouseEnter()
    {
        if (GameStateManager.canInteract && CurrentCol >=0 && Ranking != 13)
        {
        
            GameStateManager.highlightSuit = Suit;
            GameStateManager.highlightCol = CurrentCol;
            currentColor = highlightColor;
        }
        
    }
    private void OnMouseExit()
    {
        if (GameStateManager.canInteract)
        {
            GameStateManager.highlightSuit = 0;
            GameStateManager.highlightCol = -1;
            currentColor = unhighlishtedColor;
        }
       
        
    }
    public void StartMoving(Vector3 TargetLocation)
    {
        currentColor = white;
        lerpLocation = TargetLocation;
        Move = true;
        lerpFloat = 0;
        CurrentPosition = transform.position;
    }
    private void OnMouseDown()
    {
        if (Ranking != 13 && CurrentCol>-1 && CurrentCol<4 && GameStateManager.canInteract)
        {
            unhighlishtedColor = white;
            currentColor = unhighlishtedColor;
            readyToBePickedByDummy = false;
            gameStateManager.GetComponent<GameStateManager>().ClaimCard(Suit,CurrentCol,true);
        }
        Debug.Log(cardCode);
    }
    
}
