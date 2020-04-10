using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public Sprite[] cardImage;
    private GameState currentGameState;
    [SerializeField] private GameObject playingCard;
    // public Cards[] 
    public List<GameObject> unusedTopDeck;
    public List<GameObject> unusedKings;

    public List<GameObject> unusedBottomDeck;
    public List<GameObject> usedBottomDeck;

    public List<GameObject> CardsClaimedByPlayer;
    public List<GameObject> CardsClaimedByDummy;

    public static GameObject[,] LootArea;
    Stack<GameObject>[] unwantedStack;

    public List<GameObject> cardsReadyToBEPickedByDummy;

    public List<GameObject> KingsInTheUnwanted;

    public GameObject DummyActiveCard = null;
    public int[] KingCardsInEachColumn;
    [SerializeField] private Transform TopDeckTransform;
    [SerializeField] private Transform BottomDeckTransform;

    public static int highlightSuit = 0;
    public static int highlightCol = -1;

    public static bool canInteract = true;
    // Start is called before the first frame update
    void Start()
    {
        unusedTopDeck = new List<GameObject>();
        unusedBottomDeck = new List<GameObject>();
        usedBottomDeck = new List<GameObject>();
        unusedKings = new List<GameObject>();
        unwantedStack = new Stack<GameObject>[4];
        KingCardsInEachColumn = new int[4];
        for (int i = 0; i < 4; i++)
        {
            unwantedStack[i] = new Stack<GameObject>();
        }
        KingsInTheUnwanted = new List<GameObject>();

        CardsClaimedByDummy = new List<GameObject>();
        CardsClaimedByPlayer = new List<GameObject>();
        cardsReadyToBEPickedByDummy = new List<GameObject>();
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
        for (int i = 1; i < 5; i++)
        {
            for (int j = 1; j < 14; j++)
            {
                GameObject currentCard = Instantiate(playingCard);
                currentCard.GetComponent<PlayingCards>().Ranking = j;
                currentCard.GetComponent<PlayingCards>().Suit = i;
                currentCard.GetComponent<PlayingCards>().cardImage = cardImage[(i - 1) * 13 + j];
                currentCard.GetComponent<PlayingCards>().cardCode = (i - 1) * 13 + j;
                if (j <= 10)
                {
                    unusedTopDeck.Add(currentCard);
                    currentCard.transform.position = TopDeckTransform.position;
                    currentCard.SetActive(false);
                }
                else if (j > 10 && j < 13)
                {
                    unusedBottomDeck.Add(currentCard);
                    currentCard.transform.position = BottomDeckTransform.position;
                    currentCard.SetActive(false);
                }
                else if (j == 13)
                {
                    unusedTopDeck.Add(currentCard);
                    currentCard.transform.position = TopDeckTransform.position;
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
        for (ColumnNum = 0; ColumnNum < 4; ColumnNum++)
        {
            for (RowNum = 0; RowNum < 4; RowNum++)
            {
                if (LootArea[ColumnNum, RowNum] == null)
                {
                    fullLoot = false;
                    if (minimumEmptySpaceCol < 0)
                    {
                        minimumEmptySpaceCol = ColumnNum;
                        minimumEmptySpaceRow = RowNum;
                    }
                }

            }
        }

        if (unusedTopDeck.Count == 0 && unusedKings.Count == 0)
        {
            return false;
        }
        else if (fullLoot)
        {
            return false;
        }
        else
        {
            int randomIndex = Random.Range(0, unusedTopDeck.Count);
            unusedTopDeck[randomIndex].SetActive(true);
            LootArea[minimumEmptySpaceCol, minimumEmptySpaceRow] = unusedTopDeck[randomIndex];
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().StartMoving(new Vector3(-2.55f + 2.0f * minimumEmptySpaceCol, 2.38f - 1f * minimumEmptySpaceRow, 0 + minimumEmptySpaceRow * 0.1f));
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().orderInLayer = minimumEmptySpaceRow;
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().CurrentCol = minimumEmptySpaceCol;
            unusedTopDeck.RemoveAt(randomIndex);
            return true;
        }
    }

    public void FlipANewDummyActiveCard()
    {
        if (DummyActiveCard != null)
        {
            DummyActiveCard.SetActive(false);
        }

        if (unusedBottomDeck.Count == 0)
        {
            unusedBottomDeck = usedBottomDeck;
            usedBottomDeck.Clear();
        }
        int randomIndex = Random.Range(0, unusedBottomDeck.Count);
        DummyActiveCard = unusedBottomDeck[randomIndex];
        DummyActiveCard.SetActive(true);
        DummyActiveCard.GetComponent<PlayingCards>().StartMoving(new Vector3(-8.5f, -2.5f, 0));
        usedBottomDeck.Add(unusedBottomDeck[randomIndex]);
        unusedBottomDeck.RemoveAt(randomIndex);
    }

    public List<GameObject> DummyAI()
    {
        KingsInEachColumn();
        List<GameObject> cardsPlanningToTake = new List<GameObject>();
        //top priority save king from the unwanted pile
        if (KingsInTheUnwanted.Count > 0)
        {

            foreach (GameObject king in KingsInTheUnwanted)
            {
                if (king.GetComponent<PlayingCards>().Suit == DummyActiveCard.GetComponent<PlayingCards>().Suit)
                {
                    //claim the king and the card in the unwanted;

                    cardsPlanningToTake.Add(king);
                    // add cards fro the unwanted too;

                    return cardsPlanningToTake;
                }
            }
        }

        
        List<int> thisIndex = new List<int>();
        for (int i = 0; i < KingCardsInEachColumn.Length; i++)
        {
            if (KingCardsInEachColumn[i] == 2)
            {
                thisIndex.Add(i);
                
            }
        }
        int theColumn = ColumnToTake(DummyActiveCard.GetComponent<PlayingCards>().Suit, thisIndex);
                //take the most amount of cards from the loot or the unwanted
              //  if (theColumn > -1)
             //   {
              //      Debug.Log("picked column: " + theColumn);
            if (cardsWithSuitInThatColumn(DummyActiveCard.GetComponent<PlayingCards>().Suit, theColumn).Count != 0)
            {
            Debug.Log("return 2 king");
                return cardsWithSuitInThatColumn(DummyActiveCard.GetComponent<PlayingCards>().Suit, theColumn);
            }
                    

            //    }
        thisIndex.Clear();
        for (int i = 0; i < KingCardsInEachColumn.Length; i++)
        {
            if (KingCardsInEachColumn[i] == 1)
            {
                thisIndex.Add(i);

            }
        }
        theColumn = ColumnToTake(DummyActiveCard.GetComponent<PlayingCards>().Suit, thisIndex);
        //take the most amount of cards from the loot or the unwanted
       // if (theColumn > -1)
       // {
           // Debug.Log("picked column: " + theColumn);

            if (cardsWithSuitInThatColumn(DummyActiveCard.GetComponent<PlayingCards>().Suit, theColumn).Count != 0)
            {
            Debug.Log("return 1 king");
                return cardsWithSuitInThatColumn(DummyActiveCard.GetComponent<PlayingCards>().Suit, theColumn);
            }
            

       // }


        List<int> allAvaibleOption = new List<int> { 0, 1, 2, 3, 4 };

        int theColumnToPick = ColumnToTake(DummyActiveCard.GetComponent<PlayingCards>().Suit, allAvaibleOption);
        //take the most amount of cards from the loot or the unwanted
       //  if (theColumnToPick != -1)
       //   {
        Debug.Log("picked column: " + theColumnToPick);

            if (cardsWithSuitInThatColumn(DummyActiveCard.GetComponent<PlayingCards>().Suit, theColumnToPick).Count != 0)
            {
            Debug.Log("return no king");
            return cardsWithSuitInThatColumn(DummyActiveCard.GetComponent<PlayingCards>().Suit, theColumnToPick);
            }
            

       // }
       Debug.Log("cannnot return");
        return cardsPlanningToTake;
    }

    public void KingsInEachColumn()
    {
        for (int i = 0; i < 4; i++)
        {
            KingCardsInEachColumn[i] = 0;
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (LootArea[i, j] != null && LootArea[i, j].GetComponent<PlayingCards>().Ranking == 13)
                {
                    Debug.Log("king in the column");
                    KingCardsInEachColumn[i]++;
                }

            }
        }

       
    }

    //determine what column to take based on 
    public int ColumnToTake(int suit, List<int> whichColumnsToEnter)
    {
        //go through all the column listed, if there is one column with the most amount of that suit return the column index(0-4) 4 stands for the unwanted
        if (whichColumnsToEnter.Count == 0)
        {
            Debug.Log("columns avaiable is too short");
            return -1; //return -1 if cannot make a valid move;

        }
        else if (whichColumnsToEnter.Count == 1)
        {
            Debug.Log("columns avaiable is 1 length");
            return whichColumnsToEnter[0]; // return the choice if there is only one walid choice
        }
        else
        {
            int highestAmountOfSuit = SuitOfAColumn(suit, whichColumnsToEnter[0]);
            List<int> highestAmountOfSuitIndex = new List<int>();
            highestAmountOfSuitIndex.Add(whichColumnsToEnter[0]);
            for (int i = 1; i < whichColumnsToEnter.Count; i++)
            {
                if (SuitOfAColumn(suit, whichColumnsToEnter[i]) > highestAmountOfSuit)
                {
                    highestAmountOfSuitIndex.Clear();
                    highestAmountOfSuitIndex.Add(whichColumnsToEnter[i]);
                    Debug.Log(highestAmountOfSuitIndex);// clear the list and add it to the list if it is higher;
                }
                else if (SuitOfAColumn(suit, whichColumnsToEnter[i]) == highestAmountOfSuit)
                {
                    highestAmountOfSuitIndex.Add(whichColumnsToEnter[i]); //add it to the list if it is a tie;
                    Debug.Log(highestAmountOfSuitIndex);
                }

            }
            if (highestAmountOfSuit == 0)
            {
                //novalid move
                Debug.Log("all have no suits ");
                return -1;
            }
            else if (highestAmountOfSuitIndex.Count == 1)
            {
                //we have a column that have the most amout of suit;
                Debug.Log("one column has more suit");
                return (highestAmountOfSuitIndex[0]);
            }
            else
            {
                Debug.Log("check ranking");
                //return the column with the highest ranking of that card 
                return RankingOfColumn(suit, highestAmountOfSuitIndex);
            }
        }
    }

    public int SuitOfAColumn(int suit, int columnIndex)
    {
        if (columnIndex < 4) //count the loot area
        {
            int SuitCount = 0;

            if (LootArea[columnIndex, 0] != null) //only count if the column has card
            {
                for (int i = 0; i < 4; i++)
                {
                    if (LootArea[columnIndex, i].GetComponent<PlayingCards>().Suit == suit && LootArea[columnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        SuitCount++;
                    }
                }
 
            }
            return SuitCount;
           
        }
        else
        {
            int suitCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (unwantedStack[i].Count != 0)
                {
                    if (unwantedStack[i].Peek().GetComponent<PlayingCards>().Suit == suit)
                    {
                        suitCount++;
                    }
                }
            }
            //return the count of the unwanted area
            return suitCount;
        }
    }

    public int RankingOfColumn(int suit, List<int> columnIndex)
    {
        List<GameObject> firstlist = new List<GameObject>();
        int highestRanking = 0;
        int highestRankingIndex = -1;
        firstlist = cardsWithSuitInThatColumn(suit, columnIndex[0]);
        foreach (GameObject card in firstlist)
        {
            if(card.GetComponent<PlayingCards>().Ranking > highestRanking && card.GetComponent<PlayingCards>().Ranking != 13)
            {
                highestRanking = card.GetComponent<PlayingCards>().Ranking;
                highestRankingIndex = 0;
            }
        }
        for (int i = 1; i < columnIndex.Count; i++) {

            firstlist = cardsWithSuitInThatColumn(suit, columnIndex[i]);

            foreach (GameObject card in firstlist)
            {
                if (card.GetComponent<PlayingCards>().Ranking > highestRanking && card.GetComponent<PlayingCards>().Ranking != 13)
                {
                    highestRanking = card.GetComponent<PlayingCards>().Ranking;
                    highestRankingIndex = i;
                }
            }


        }
        Debug.Log("highest ranking column " + highestRankingIndex);
        return highestRankingIndex;// return the column with the highest ranking card except court cards;
    }

    public List<GameObject> cardsWithSuitInThatColumn(int suit, int ColumnIndex)
    {
        List<GameObject> allCardsWithThisSuit = new List<GameObject>();

        if (ColumnIndex<0)
        {
            return allCardsWithThisSuit;
        }
        else if (ColumnIndex < 4) //count the loot area
        {


            if (LootArea[ColumnIndex, 0] != null) //only count if the column has card
            {
                for (int i = 0; i < 4; i++)
                {
                    if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit == suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        allCardsWithThisSuit.Add(LootArea[ColumnIndex, i]);
                    }
                }

            }
            return allCardsWithThisSuit;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (unwantedStack[i].Count != 0)
                {
                    if (unwantedStack[i].Peek().GetComponent<PlayingCards>().Suit == suit)
                    {
                        allCardsWithThisSuit.Add(unwantedStack[i].Peek());
                    }
                }
            }

            return allCardsWithThisSuit;
        }
    }

    public void ClaimCard(int suit, int ColumnIndex, bool Player)
    {
        Vector3 TargetDummyLocation = new Vector3(-10, 0, 0);
        Vector3 TargetPlayerLocation = new Vector3(6.58f, -3.72f, 0);
        Vector3 KingArea = new Vector3(9.62f, -0.65f, 0);
        if (Player)
        {
            if (ColumnIndex < 4) // claim fro the loot
            {

                for (int i = 3; i >= 0; i--)
                {
                    if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit == suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(TargetPlayerLocation);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = -3;
                        CardsClaimedByPlayer.Add(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i] = null;

                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit != suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        int stackIndex = AddToTheUnwanted(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(new Vector3(2 * stackIndex, -3.41f, 0));
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().orderInLayer = 2;
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        LootArea[ColumnIndex, i] = null;
                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking == 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(KingArea);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        KingsInTheUnwanted.Add(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i] = null;
                    }
                }



            }
            else if (ColumnIndex == 4)
            {
                // claim cards from the unwanted
            }
        }
        else //dummy claim
        {
            if (ColumnIndex < 4) // claim fro the loot
            {

                for (int i = 3; i >= 0; i--)
                {
                    if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit == suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(TargetDummyLocation);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = -3;
                        CardsClaimedByDummy.Add(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i] = null;

                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit != suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        int stackIndex = AddToTheUnwanted(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(new Vector3(2 * stackIndex, -3.41f, 0));
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().orderInLayer = 2;
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        LootArea[ColumnIndex, i] = null;
                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking == 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(KingArea);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        KingsInTheUnwanted.Add(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i] = null;
                    }
                }



            }
            else if (ColumnIndex == 4)
            {
                // dummy claim cards from the unwanted
            }
        }
    }


    public int AddToTheUnwanted(GameObject card)
    {
        int thinestStackIndex = 0;
        int LowestAmountOfCard = unwantedStack[0].Count;
        Debug.Log(unwantedStack[0].Count);
        for (int i = 1; i < 4; i++)
        {
            if (unwantedStack[i].Count < LowestAmountOfCard)
            {
                thinestStackIndex = i;
                LowestAmountOfCard = unwantedStack[i].Count;
            }
        }
        if (unwantedStack[thinestStackIndex].Count != 0)
        {
            unwantedStack[thinestStackIndex].Peek().SetActive(false);
        }

        unwantedStack[thinestStackIndex].Push(card);
        return thinestStackIndex;
    }

    public void RefreshDummyAI()
    {
        for (int i = 0; i < 4; i++)
        {
            if (unwantedStack[i].Count != 0)
            {
                unwantedStack[i].Peek().GetComponent<PlayingCards>().readyToBePickedByDummy = false;
                

            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (LootArea[i, j] != null)
                {
                    LootArea[i, j].GetComponent<PlayingCards>().readyToBePickedByDummy = false;
                   
                }
            }
        }
        CardsClaimedByDummy.Clear();
        cardsReadyToBEPickedByDummy = DummyAI();
        if (cardsReadyToBEPickedByDummy.Count == 0)
        {

            Debug.Log("nothing to do");
        }
        else
        {
            foreach (GameObject card in cardsReadyToBEPickedByDummy)
        {
            card.GetComponent<PlayingCards>().readyToBePickedByDummy = true;
            
        }
        }
        
        
    }
}
