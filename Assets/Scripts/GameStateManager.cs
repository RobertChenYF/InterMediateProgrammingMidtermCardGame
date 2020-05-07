using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameStateManager : MonoBehaviour
{
    public TextMeshProUGUI PileName;
    public GameObject exitButton;
    public GameObject backGround;
    public Sprite[] cardImage;
    private GameState currentGameState;
    [SerializeField] private GameObject playingCard;

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

    public static List<GameObject> KingsClaimedByDummy;

    public static float timer;
    public static GameObject DummyActiveCard = null;
    public int[] KingCardsInEachColumn;
    public Transform TopDeckTransform;
    public Transform BottomDeckTransform;

    public Transform PlayerClaimedCardLocation;
    public Transform DummyClaimedCardLocation;
    public Transform UnwantedKingLocation;
    public Transform ClaimedKings;
    public Transform DummyActionDiscardPile;
    public Transform UnwantedPile;
    public Transform LootPile;
    public Transform FirstCardOfTheWindow;

    public string[] PileNames;


    public static int SuitFromTheUnwantedPlayerChoose = -1;

    public static int highlightSuit = 0;
    public static int highlightCol = -1;
    public float gapBetweenCloumn;
    public static bool canInteract = false;

    public static bool SelectedCard = false;

    public static int CurrentDisplayCard;
    public Transform[] currentLyShowedDeck;
    public static List<GameObject> displayedCard;
    public GameObject backGroundWindow;

    public static bool specialSituation;

    public TextMeshProUGUI Score;

    private int[] scoreForStraight;

    public Button skipTurn;
    public GameObject skipTurnButton;
    public GameObject GameEndButton;
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
        KingsClaimedByDummy = new List<GameObject>();
        cardsReadyToBEPickedByDummy = new List<GameObject>();
        displayedCard = new List<GameObject>();
        LootArea = new GameObject[4, 4];
        ChangeState(new Setup(this));

        scoreForStraight = new int[] { 0, 0, 0, 3, 7, 12, 18, 25, 33, 42, 52 };
        skipTurn.onClick.AddListener(skipPlayerTurn);
    }

    // Update is called once per frame
    void Update()
    {
        currentGameState.stateBehavior();
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (canInteract == true)
        {
            skipTurnButton.SetActive(true);
        }
        else
        {
            skipTurnButton.SetActive(false);
        }
    }


    public void skipPlayerTurn()
    {
        if (SelectedCard == false)
        {
        SelectedCard = true;
        timer = 2;
        }
        
    }
    public void ChangeState(GameState newGameState)
    {
        if (currentGameState != null) currentGameState.Leave();
        currentGameState = newGameState;
        currentGameState.Enter();
    }

    //generate all the card at the beginning of the game
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

    //refill card to the loot;
    public bool DealCardToLoot()
    {
        CardsClaimedByDummy.Sort(SortByCardCode);
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
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().StartMoving(new Vector3(LootPile.position.x + gapBetweenCloumn * minimumEmptySpaceCol, LootPile.position.y - 1f * minimumEmptySpaceRow, 0 + minimumEmptySpaceRow * 0.1f));
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().orderInLayer = minimumEmptySpaceRow;
            unusedTopDeck[randomIndex].GetComponent<PlayingCards>().CurrentCol = minimumEmptySpaceCol;
            unusedTopDeck.RemoveAt(randomIndex);
            return true;
        }
    }

    //flip a new action card for dummy;
    public void FlipANewDummyActiveCard()
    {
      //  if (DummyActiveCard != null)
      //  {
       //     DummyActiveCard.SetActive(false);
      //  }

        if (unusedBottomDeck.Count == 0)
        {
            foreach (GameObject card in usedBottomDeck)
            {
                unusedBottomDeck.Add(card);
            }

            usedBottomDeck.Clear();
        }

        int randomIndex = Random.Range(0, unusedBottomDeck.Count);
        DummyActiveCard = unusedBottomDeck[randomIndex];
        DummyActiveCard.SetActive(true);
        DummyActiveCard.GetComponent<PlayingCards>().StartMoving(DummyActionDiscardPile.position);
        AddToList(DummyActiveCard,usedBottomDeck);
        unusedBottomDeck.RemoveAt(randomIndex);
    }

    //Dummy's AI determine dummy's action
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


                    // add cards fro the unwanted too;
                    cardsPlanningToTake = cardsWithSuitInThatColumn(king.GetComponent<PlayingCards>().Suit, 4);
                    cardsPlanningToTake.Add(king);
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

    //update the amount of king cards in each column
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

    //determine what column to take based on its priority of dummy
    public int ColumnToTake(int suit, List<int> whichColumnsToEnter)
    {
        //go through all the column listed, if there is one column with the most amount of that suit return the column index(0-4) 4 stands for the unwanted
        if (whichColumnsToEnter.Count == 0)
        {
          //  Debug.Log("columns avaiable is too short");
            return -1; //return -1 if cannot make a valid move;

        }
        else if (whichColumnsToEnter.Count == 1)
        {
           // Debug.Log("columns avaiable is 1 length");
            return whichColumnsToEnter[0]; // return the choice if there is only one walid choice
        }
        else
        {
            int highestAmountOfSuit = SuitOfAColumn(suit, whichColumnsToEnter[0]);
            List<int> highestAmountOfSuitIndex;
            highestAmountOfSuitIndex = new List<int>();
            highestAmountOfSuitIndex.Add(whichColumnsToEnter[0]);
            for (int i = 1; i < whichColumnsToEnter.Count; i++)
            {
                if (SuitOfAColumn(suit, whichColumnsToEnter[i]) > highestAmountOfSuit)
                {
                    highestAmountOfSuitIndex.Clear();
                    highestAmountOfSuit = SuitOfAColumn(suit, whichColumnsToEnter[i]);
                    highestAmountOfSuitIndex.Add(whichColumnsToEnter[i]);
                    // clear the list and add it to the list if it is higher;
                }
                else if (SuitOfAColumn(suit, whichColumnsToEnter[i]) == highestAmountOfSuit)
                {
                    highestAmountOfSuitIndex.Add(whichColumnsToEnter[i]); //add it to the list if it is a tie;

                }

            }
            if (highestAmountOfSuit == 0)
            {
                //novalid move
               // Debug.Log("all have no suits ");
                return -1;
            }
            else if (highestAmountOfSuitIndex.Count == 1)
            {
                //we have a column that have the most amout of suit;
               // Debug.Log("one column has more suit");
                return (highestAmountOfSuitIndex[0]);
            }
            else
            {
                //Debug.Log("check ranking");
                //return the column with the highest ranking of that card 
                return RankingOfColumn(suit, highestAmountOfSuitIndex);
            }
        }
    }

    //return the number of copies of suit in that column
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

    //return the column with the cards with the highest ranking
    public int RankingOfColumn(int suit, List<int> columnIndex)
    {
        List<GameObject> firstlist = new List<GameObject>();
        int highestRanking = 0;
        int highestRankingIndex = columnIndex[0];
        firstlist = cardsWithSuitInThatColumn(suit, columnIndex[0]);
        foreach (GameObject card in firstlist)
        {
            if (card.GetComponent<PlayingCards>().Ranking > highestRanking && card.GetComponent<PlayingCards>().Ranking != 13)
            {
                highestRanking = card.GetComponent<PlayingCards>().Ranking;
                highestRankingIndex = columnIndex[0];
            }
        }
        for (int i = 1; i < columnIndex.Count; i++)
        {

            firstlist = cardsWithSuitInThatColumn(suit, columnIndex[i]);

            foreach (GameObject card in firstlist)
            {
                if (card.GetComponent<PlayingCards>().Ranking > highestRanking && card.GetComponent<PlayingCards>().Ranking != 13)
                {
                    highestRanking = card.GetComponent<PlayingCards>().Ranking;
                    highestRankingIndex = columnIndex[i];
                }
            }


        }
        Debug.Log("highest ranking column " + highestRankingIndex);
        return highestRankingIndex;// return the column with the highest ranking card except court cards;
    }

    //return the list of Gameobject of all the cards of that suit in that column
    public List<GameObject> cardsWithSuitInThatColumn(int suit, int ColumnIndex)
    {
        List<GameObject> allCardsWithThisSuit = new List<GameObject>();

        if (ColumnIndex < 0)
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

    //claim card from the stack, for players and dummy
    public void ClaimCard(int suit, int ColumnIndex, bool Player)
    {

        
        if (Player)
        {
            if (ColumnIndex < 4) // claim fro the loot
            {

                for (int i = 3; i >= 0; i--)
                {
                    if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit == suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(PlayerClaimedCardLocation.position);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = -3;
                        
                        AddToList(LootArea[ColumnIndex, i],CardsClaimedByPlayer);
                        LootArea[ColumnIndex, i] = null;

                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit != suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        int stackIndex = AddToTheUnwanted(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(new Vector3(UnwantedPile.position.x + gapBetweenCloumn * stackIndex, UnwantedPile.position.y + unwantedStack[stackIndex].Count * 0.05f, 0));
                        if (unwantedStack[stackIndex].Count > 1)
                        {
                            GameObject tempCard = unwantedStack[stackIndex].Pop();
                            LootArea[ColumnIndex, i].GetComponent<PlayingCards>().orderInLayer = unwantedStack[stackIndex].Peek().GetComponent<PlayingCards>().orderInLayer + 1;
                            unwantedStack[stackIndex].Push(tempCard);

                        }
                        else
                        {
                            LootArea[ColumnIndex, i].GetComponent<PlayingCards>().orderInLayer = 1;
                        }
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        LootArea[ColumnIndex, i] = null;
                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking == 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(UnwantedKingLocation.position);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        AddToList(LootArea[ColumnIndex, i],KingsInTheUnwanted);
                        LootArea[ColumnIndex, i] = null;
                    }
                }



            }


        }
        else //dummy claim
        {
            if (ColumnIndex < 4) // claim from the loot
            {

                for (int i = 3; i >= 0; i--)
                {
                    if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit == suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(DummyClaimedCardLocation.position);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = -3;
                        AddToList(LootArea[ColumnIndex, i], CardsClaimedByDummy);
                        LootArea[ColumnIndex, i] = null;

                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Suit != suit && LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking != 13)
                    {
                        int stackIndex = AddToTheUnwanted(LootArea[ColumnIndex, i]);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(new Vector3(UnwantedPile.position.x + gapBetweenCloumn * stackIndex, UnwantedPile.position.y+ unwantedStack[stackIndex].Count*0.05f, 0));
                        if (unwantedStack[stackIndex].Count>1)
                        {
                           GameObject tempCard =  unwantedStack[stackIndex].Pop();
                            LootArea[ColumnIndex, i].GetComponent<PlayingCards>().orderInLayer = unwantedStack[stackIndex].Peek().GetComponent<PlayingCards>().orderInLayer +1;
                            unwantedStack[stackIndex].Push(tempCard);

                        }
                        else
                        {
                            LootArea[ColumnIndex, i].GetComponent<PlayingCards>().orderInLayer = 1;
                        }
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        LootArea[ColumnIndex, i] = null;
                    }
                    else if (LootArea[ColumnIndex, i].GetComponent<PlayingCards>().Ranking == 13)
                    {
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().StartMoving(UnwantedKingLocation.position);
                        LootArea[ColumnIndex, i].GetComponent<PlayingCards>().CurrentCol = 4;
                        AddToList(LootArea[ColumnIndex, i], KingsInTheUnwanted);
                        LootArea[ColumnIndex, i] = null;
                    }
                }

            }
            else if (ColumnIndex == 4)
            {
                if (KingsInTheUnwanted.Count != 0)
                {
                    foreach (GameObject kingCard in KingsInTheUnwanted)
                    {
                        if (kingCard.GetComponent<PlayingCards>().Suit == suit)
                        {
                            kingCard.GetComponent<PlayingCards>().CurrentCol = -2;
                            kingCard.GetComponent<PlayingCards>().StartMoving(ClaimedKings.position);
                            KingsInTheUnwanted.Remove(kingCard);
                            
                            AddToList(kingCard, KingsClaimedByDummy);
                            break;
                        }
                    }
                }

                while (DummyClaimFormTheUnwanted(suit))
                {

                }
                // dummy claim cards from the unwanted
            }
        }


        RefreshScore();
    }

    //adding cards to the unwanted after the claim
    public int AddToTheUnwanted(GameObject card)
    {
        int thinestStackIndex = 0;
        int LowestAmountOfCard = unwantedStack[0].Count;
       // Debug.Log(unwantedStack[0].Count);
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
            unwantedStack[thinestStackIndex].Peek().GetComponent<BoxCollider2D>().enabled = false;
        }

        unwantedStack[thinestStackIndex].Push(card);
        return thinestStackIndex;
    }

    public bool ClaimFromTheUnwanted(int suit, bool player, GameObject thisCard)
    {
        if (thisCard != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (unwantedStack[i].Count != 0)
                {
                    if (unwantedStack[i].Peek() == thisCard)
                    {

                        unwantedStack[i].Peek().GetComponent<PlayingCards>().StartMoving(PlayerClaimedCardLocation.position);
                        unwantedStack[i].Peek().GetComponent<PlayingCards>().CurrentCol = -2;
                        CardsClaimedByPlayer.Add(unwantedStack[i].Pop());
                        RefreshScore();
                        if (unwantedStack[i].Count != 0)
                        {
                            unwantedStack[i].Peek().GetComponent<BoxCollider2D>().enabled = true;
                        }

                    }
                }

            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (unwantedStack[i].Count != 0)
            {
                if (unwantedStack[i].Peek().GetComponent<PlayingCards>().Suit == suit)
                {

                    return true;
                }
            }
        }
        return false;
    }

    public bool DummyClaimFormTheUnwanted(int suit)
    {
        for (int i = 0; i < 4; i++)
        {
            if (unwantedStack[i].Count != 0)
            {
                if (unwantedStack[i].Peek().GetComponent<PlayingCards>().Suit == suit)
                {
                    unwantedStack[i].Peek().GetComponent<PlayingCards>().CurrentCol = -2;
                    unwantedStack[i].Peek().GetComponent<PlayingCards>().StartMoving(DummyClaimedCardLocation.position);
                    
                    AddToList(unwantedStack[i].Pop(), CardsClaimedByDummy);
                    if (unwantedStack[i].Count != 0)
                    {
                        unwantedStack[i].Peek().GetComponent<BoxCollider2D>().enabled = true;
                    }

                    return true;
                }
            }
        }
        return false;
    }


    //refresh the color of card to show the card dummy is going to take based on current situation
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

    public void CloseWindow()
    {
        if (displayedCard.Count != 0 && CurrentDisplayCard != -1)
        {
            foreach (GameObject card in displayedCard)
            {
                card.transform.position = currentLyShowedDeck[CurrentDisplayCard].position;
                card.GetComponent<PlayingCards>().orderInLayer -= 100;
                card.transform.localScale = new Vector3(1,1,1);
                if (CurrentDisplayCard!=1 && CurrentDisplayCard != 2&&CurrentDisplayCard != 3 && CurrentDisplayCard != 6 && CurrentDisplayCard != 5)
                {
                    card.SetActive(false);
                }
                
            }
            displayedCard.Clear();
            CurrentDisplayCard = -1;
            backGroundWindow.SetActive(false);
            canInteract = true;
        }

        skipTurnButton.SetActive(true);
        //move all card back
        //disable the cardif necessaaey
        //resetLayorder

    }

    public void OpenPlayerPickWindow()
    {
        GameStateManager.canInteract = false;
        backGround.SetActive(true);
        exitButton.SetActive(false);
        PileName.text = "Dummy cannot make a valid move. Pick a card from all cards it claimed as a bonus.";
        int count = 0;
        GameStateManager.CurrentDisplayCard = 2;
        foreach (GameObject card in CardsClaimedByDummy)
        {
            card.transform.localScale = new Vector3(0.8f, 0.8f, 1); 
            card.transform.position = new Vector3(FirstCardOfTheWindow.position.x + (count % 10) * 1.6f, -(int)(count / 10) * 2 + FirstCardOfTheWindow.position.y, 0);
            GameStateManager.displayedCard.Add(card);
            card.GetComponent<PlayingCards>().orderInLayer += 100;
            count++;
            card.SetActive(true);
        }
        skipTurnButton.SetActive(false);
    }

    public void closePlayerPickWindow()
    {
        if (displayedCard.Count != 0 && CurrentDisplayCard != -1)
        {
            foreach (GameObject card in displayedCard)
            {
                card.transform.position = currentLyShowedDeck[CurrentDisplayCard].position;
                card.GetComponent<PlayingCards>().orderInLayer -= 100;
                card.transform.localScale = new Vector3(1, 1, 1);
                if (CurrentDisplayCard != 1 && CurrentDisplayCard != 2 && CurrentDisplayCard != 3 && CurrentDisplayCard != 6 && CurrentDisplayCard != 5)
                {
                    card.SetActive(false);
                }

            }
            displayedCard.Clear();
            CurrentDisplayCard = -1;
            exitButton.SetActive(true);
            backGroundWindow.SetActive(false);
            
        }

        skipTurnButton.SetActive(true);
    }

    public void GameEndScreen()
    {
        GameStateManager.canInteract = false;
        backGround.SetActive(true);
        exitButton.SetActive(false);
        PileName.text = "Game End";
        int count = 0;
        GameStateManager.CurrentDisplayCard = 6;
        foreach (GameObject card in CardsClaimedByPlayer)
        {
            card.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            card.transform.position = new Vector3(FirstCardOfTheWindow.position.x + (count % 10) * 1.6f, -(int)(count / 10) * 2 + FirstCardOfTheWindow.position.y, 0);
            GameStateManager.displayedCard.Add(card);
            card.GetComponent<PlayingCards>().orderInLayer += 100;
            count++;
            card.SetActive(true);
        }
        SaveFile.currentScore += CalculateScore();
        
        GameEndButton.SetActive(true);
    }

    public int CalculateScore()//calculate the score based for the longest straight flush for each suit;
    {
        bool[,] ifCardCollected = new bool[4, 10];//default false
        
        
        foreach (GameObject card in CardsClaimedByPlayer)
        {
            ifCardCollected[card.GetComponent<PlayingCards>().Suit - 1, card.GetComponent<PlayingCards>().Ranking - 1] = true;
        }


        return CardsClaimedByPlayer.Count + scoreForStraight[getMaxLength(ifCardCollected, 0)] + scoreForStraight[getMaxLength(ifCardCollected, 1)]
            + scoreForStraight[getMaxLength(ifCardCollected, 2)] + scoreForStraight[getMaxLength(ifCardCollected, 3)]; //each card count as one point + the score for the straight flush
    }

    public void RefreshScore()
    {
        CardsClaimedByPlayer.Sort(SortByCardCode);
        Score.text = "Score: " + CalculateScore();
    }
    static int SortByCardCode(GameObject card1, GameObject card2)
    {
        return card1.GetComponent<PlayingCards>().cardCode.CompareTo(card2.GetComponent<PlayingCards>().cardCode);
    }

    static int getMaxLength(bool[,] array, int a)//get the longest consecutive true in anarray
    {

        int count = 0; 
        int result = 0; 

        for (int i = 0; i < 10; i++)
        {

            if (array[a,i] == false)
                count = 0;

            else
            {
                count++; 
                result = Mathf.Max(result, count);
            }
        }

        return result;
    }

    public void AddToList(GameObject card, List<GameObject> cardList)
    {
        if (cardList.Count!=0)
        {
            card.GetComponent<PlayingCards>().orderInLayer = cardList[cardList.Count - 1].GetComponent<PlayingCards>().orderInLayer + 1;
        }
        else
        {
            card.GetComponent<PlayingCards>().orderInLayer = 1;
        }
        cardList.Add(card);
    }
}
