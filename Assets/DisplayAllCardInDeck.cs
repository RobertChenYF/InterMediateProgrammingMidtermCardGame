using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAllCardInDeck : MonoBehaviour
{
    public List<GameObject> ThisDeck;
    public GameObject backGround;
    public Transform FirstCard;
    public int PileCode = -1;
    public Color Disabled;
    public Color Abled; 
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (PileCode == 0)
        {
            ThisDeck = GameObject.Find("GameStateManager").GetComponent<GameStateManager>().unusedBottomDeck;
        }
        else if (PileCode == 1)
        {
            ThisDeck = GameObject.Find("GameStateManager").GetComponent<GameStateManager>().usedBottomDeck;
        }
        else if (PileCode == 2)
        {
            ThisDeck = GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CardsClaimedByDummy;
        }
        else if (PileCode == 3)
        {
            ThisDeck = GameStateManager.KingsClaimedByDummy;

        }
        else if (PileCode == 4)
        {
            ThisDeck = GameObject.Find("GameStateManager").GetComponent<GameStateManager>().unusedTopDeck;
        }
        else if (PileCode == 5)
        {
            ThisDeck = GameObject.Find("GameStateManager").GetComponent<GameStateManager>().KingsInTheUnwanted;
        }
        else if (PileCode == 6)
        {
            ThisDeck = GameObject.Find("GameStateManager").GetComponent<GameStateManager>().CardsClaimedByPlayer;
        }

        if (backGround.activeSelf == false && GameStateManager.canInteract == true&&ThisDeck.Count>0)
        {
            GetComponent<SpriteRenderer>().color = Abled;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Disabled;
        }
    }


    private void OnMouseDown()
    {
        if (backGround.activeSelf == false && GameStateManager.canInteract == true)
        {
            PrintAllCard();
        }

    }

    private void PrintAllCard()
    {
        int count = 0;
        GameStateManager.CurrentDisplayCard = PileCode;
        foreach (GameObject card in ThisDeck)
        {
            card.transform.localScale =new Vector3  (0.8f,0.8f,1); 
            

            card.transform.position = new Vector3(FirstCard.position.x+(count%10)*1.6f,- (int)(count/10)*2+FirstCard.position.y,0);
            GameStateManager.displayedCard.Add(card);
            card.GetComponent<PlayingCards>().orderInLayer +=11;
           count++;
            GameStateManager.canInteract = false;
            backGround.SetActive(true);
            card.SetActive(true);
        }
    }
}
