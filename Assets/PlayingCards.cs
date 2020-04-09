using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCards : MonoBehaviour
{
    
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color white;
    private Color currentColor;
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
    // Start is called before the first frame update
    void Start()
    {
       
        GetComponent<SpriteRenderer>().sprite = cardImage;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
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

        if (GameStateManager.canInteract && CurrentCol >= 0)
        {
            if (CurrentCol == GameStateManager.highlightCol && Suit == GameStateManager.highlightSuit)
            {
                currentColor = highlightColor;
            }
            else
            {
                currentColor = white;
            }
        }

        GetComponent<SpriteRenderer>().color = currentColor;
    }
    private void OnMouseEnter()
    {
        if (GameStateManager.canInteract && CurrentCol >=0)
        {
        currentColor = highlightColor;
            GameStateManager.highlightSuit = Suit;
            GameStateManager.highlightCol = CurrentCol;
        }
        
    }
    private void OnMouseExit()
    {
        if (GameStateManager.canInteract)
        {
            GameStateManager.highlightSuit = 0;
            GameStateManager.highlightCol = -1;
        }
        currentColor = white;
    }
    public void StartMoving(Vector3 TargetLocation)
    {
        lerpLocation = TargetLocation;
        Move = true;
        lerpFloat = 0;
        CurrentPosition = transform.position;
    }
    private void OnMouseDown()
    {
        Debug.Log(cardCode);
    }
}
