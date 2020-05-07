using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCards : MonoBehaviour
{
    
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color white;
    [SerializeField] private Color blue;
    [SerializeField] private Color red;
    [SerializeField] private Color yellow;
    private GameObject gameStateManager;
    private Color currentColor;
    private Color unhighlishtedColor;
    public int Suit;
    public int Ranking;
    public int cardCode;
    public Sprite cardImage;
    public int orderInLayer;
    
    
    
    
    public int CurrentCol = -2;
    public bool readyToBePickedByDummy = false;
    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = GameObject.Find("GameStateManager");
        GetComponent<SpriteRenderer>().sprite = cardImage;
        unhighlishtedColor = white;
        currentColor = unhighlishtedColor;
        if (Ranking == 13)
        {
            highlightColor = red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;

        if (readyToBePickedByDummy)
        {
            unhighlishtedColor = blue;
            currentColor = unhighlishtedColor;
        }
        else
        {
            unhighlishtedColor = white;
        }
       

        if (GameStateManager.canInteract && CurrentCol >= 0 && Ranking != 13)
        {
            if (CurrentCol == GameStateManager.highlightCol && Suit == GameStateManager.highlightSuit)
            {
                currentColor = highlightColor;
            }
            else currentColor = unhighlishtedColor;
        }

        if ((GameStateManager.SuitFromTheUnwantedPlayerChoose != Suit || CurrentCol != 4) && GameStateManager.SuitFromTheUnwantedPlayerChoose != -1)
        {
            highlightColor = red;
        }
        else if(Ranking != 13)
        {
            highlightColor = yellow;
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
        else if (Ranking ==13 && GameStateManager.canInteract && CurrentCol >= 0) {


            currentColor = highlightColor;
        }
        else if (GameStateManager.specialSituation && GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CardsClaimedByDummy.Contains(gameObject))
        {
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
       else if (GameStateManager.specialSituation)
        {
            currentColor = unhighlishtedColor;
        }
        
    }
    public void StartMoving(Vector3 TargetLocation)
    {
        readyToBePickedByDummy = false;
        currentColor = white;
        StartCoroutine(MoveToLocation(transform.position,TargetLocation));
    }
    IEnumerator MoveToLocation(Vector3 startPosition, Vector3 targetPosition)
    {
        float lerpFloat = 0;
        while (lerpFloat < 1)
        {
        lerpFloat += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, targetPosition, lerpFloat);
        yield return null;
        }
        
        
    }

    private void OnMouseDown()
    {
        if (Ranking != 13 && CurrentCol>-1 && CurrentCol<4 && GameStateManager.canInteract && highlightColor != red)
        {
            unhighlishtedColor = white;
            currentColor = unhighlishtedColor;
            readyToBePickedByDummy = false;
            gameStateManager.GetComponent<GameStateManager>().ClaimCard(Suit,CurrentCol,true);
            GameStateManager.SelectedCard = true;
            GameStateManager.timer = 2;
        }
        else if (Ranking != 13 && CurrentCol == 4 && GameStateManager.canInteract &&(Suit == GameStateManager.SuitFromTheUnwantedPlayerChoose || GameStateManager.SuitFromTheUnwantedPlayerChoose == -1))
        {
            unhighlishtedColor = white;
            currentColor = unhighlishtedColor;
            readyToBePickedByDummy = false;
            GameStateManager.SuitFromTheUnwantedPlayerChoose = Suit;
            if (gameStateManager.GetComponent<GameStateManager>().ClaimFromTheUnwanted(GameStateManager.SuitFromTheUnwantedPlayerChoose, true,gameObject) == false) {


            GameStateManager.SelectedCard = true;
            GameStateManager.timer = 2;

             }
            
        }
        else if(GameStateManager.specialSituation && GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CardsClaimedByDummy.Contains(gameObject))
        {
            unhighlishtedColor = white;
            currentColor = unhighlishtedColor;
            GameStateManager.specialSituation = false;
            StartMoving(GameObject.Find("GameStateManager").GetComponent<GameStateManager>().PlayerClaimedCardLocation.position);
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().AddToList(gameObject, GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CardsClaimedByPlayer);
            gameStateManager.GetComponent<GameStateManager>().RefreshScore();
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CardsClaimedByDummy.Remove(gameObject);
        }
        Debug.Log(cardCode);
    }
    
}
