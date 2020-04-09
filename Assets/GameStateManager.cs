using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public Sprite[] cardImage;
    private GameState currentGameState;
    [SerializeField]private GameObject playingCard;
    // public Cards[] 
    public List<GameObject> unusedTopDeck;
    public List<GameObject> unusedBottomDeck;

    public static GameObject[,] LootArea;

    [SerializeField]private Transform TopDeckTransform;
    [SerializeField]private Transform BottomDeckTransform;

    public static int highlightSuit = 0;
    public static int highlightCol = -1;

    public static bool canInteract = true;
    // Start is called before the first frame update
    void Start()
    {
        unusedTopDeck = new List<GameObject>();
        unusedBottomDeck = new List<GameObject>();
        LootArea = new GameObject[4, 4];
       ChangeState(new Setup(this));
    }

    // Update is called once per frame
    void Update()
    {
        currentGameState.stateBehavior();
    }

    public void ChangeState(GameState newGameState)
    {
        if (currentGameState != null) currentGameState.Leave();
        currentGameState = newGameState;
        currentGameState.Enter();
    }

    public void MakingAllCards()
    {
        for (int i = 1; i < 5; i ++ )
        {
            for (int j = 1; j < 14; j++)
            {
                GameObject currentCard = Instantiate(playingCard);
                currentCard.GetComponent<PlayingCards>().Ranking = j;
                currentCard.GetComponent<PlayingCards>().Suit = i;
                currentCard.GetComponent<PlayingCards>().cardImage = cardImage[(i-1)*13 + j];
                currentCard.GetComponent<PlayingCards>().cardCode = (i-1) * 13 + j;
                if (j <=10)
                {
                    unusedTopDeck.Add(currentCard);
                    currentCard.transform.position = TopDeckTransform.position;
                    currentCard.SetActive(false);
                }
                else if (j>10 && j < 13)
                {
                    unusedBottomDeck.Add(currentCard);
                    currentCard.transform.position = BottomDeckTransform.position;
                    currentCard.SetActive(false);
                }

            }
        }
    }

    public bool DealCardToLoot()
    {
        bool fullLoot = true;
        int minimumEmptySpaceCol = -99;
        int minimumEmptySpaceRow = -99;
        int ColumnNum;
        int RowNum;
        for (ColumnNum =0; ColumnNum < 4; ColumnNum ++)
        {
            for (RowNum = 0; RowNum < 4; RowNum++)
            {
                if (LootArea[ColumnNum,RowNum] == null)
                {
                    fullLoot = false;
                    if (minimumEmptySpaceCol< 0)
                    {
                        minimumEmptySpaceCol = ColumnNum;
                        minimumEmptySpaceRow = RowNum;
                    }
                }
                
            }
        }

        if (unusedTopDeck.Count == 0)
        {
            return false;
        }
        else if (fullLoot)
        {
            return false;
        }
        else
        {
            int randomIndex = Random.Range(0,unusedTopDeck.Count);
            unusedTopDeck[randomIndex].SetActive(true);
            LootArea[minimumEmptySpaceCol, minimumEmptySpaceRow] = unusedTopDeck[randomIndex];
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().StartMoving(new Vector3(-2.55f+2.0f*minimumEmptySpaceCol,2.38f-1f*minimumEmptySpaceRow,0+minimumEmptySpaceRow*0.1f));
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().orderInLayer = minimumEmptySpaceRow;
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().CurrentCol = minimumEmptySpaceCol;
            unusedTopDeck.RemoveAt(randomIndex);
            return true;
        }
    }
}
