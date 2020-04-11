using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentGameStates : MonoBehaviour
{
    
}
public class Setup: GameState
{
public Setup(GameStateManager theGameStateManager) : base(theGameStateManager)
{

}

    public override void stateBehavior()
    {
        gameStateManager.ChangeState(new Refill(gameStateManager));
    }

    public override void Enter()
    {
        base.Enter();
        gameStateManager.MakingAllCards();

    }
    public override void Leave()
    {
        base.Leave();
        
    }


}

public class Refill : GameState
{
    public Refill(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }
    public override void stateBehavior()
    {
        if (GameStateManager.timer <=0 )
        {
 while (gameStateManager.DealCardToLoot())
        {
            Debug.Log("DealCardToLoot");
        }
        

        


        gameStateManager.ChangeState(new PlayerTurn(gameStateManager));
        }
       

    }

    public override void Enter()
    {
        base.Enter();
        GameStateManager.timer = 2;
        

    }
    public override void Leave()
    {
        base.Leave();
        gameStateManager.FlipANewDummyActiveCard();
        gameStateManager.RefreshDummyAI();
    }

}

public class PlayerTurn : GameState
{
    
    public PlayerTurn(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        if (GameStateManager.SelectedCard == true)
        {
            gameStateManager.RefreshDummyAI();
            if (GameStateManager.timer<0)
            {
            gameStateManager.ChangeState(new DummyTurn(gameStateManager));
            }
            
        }
    }
    public override void Enter()
    {
        base.Enter();
        
        GameStateManager.canInteract = true;
    }

    public override void Leave()
    {
        base.Leave();
        gameStateManager.RefreshDummyAI();
        GameStateManager.SuitFromTheUnwantedPlayerChoose = -1;
        GameStateManager.SelectedCard = false;
        GameStateManager.canInteract = false;
    }
}

public class DummyTurn : GameState
{
   
    private List<GameObject> gameObjects;
    public DummyTurn(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        
        
       
        if (gameObjects.Count == 0)
        {
            //nothing to do let player pick a card
            Debug.Log("cannot take any cards");
        }
        else
        {
            
            gameStateManager.ClaimCard(GameStateManager.DummyActiveCard.GetComponent<PlayingCards>().Suit, gameObjects[0].GetComponent<PlayingCards>().CurrentCol, false);
            
            
            if (GameStateManager.KingsClaimedByDummy.Count == 4)

        {
            Debug.Log("game Ends");

                gameStateManager.ChangeState(new EndGameState(gameStateManager));
            }
        else
        {
                
                gameStateManager.ChangeState(new Refill(gameStateManager));
                
            
            //enter player turn
        }

        }

        

        

    }


    public override void Enter()
    {
        base.Enter();
        GameStateManager.timer = 8;
        gameObjects = new List<GameObject>();
        gameObjects = gameStateManager.DummyAI();
    }

    public override void Leave()
    {
        base.Leave();
    }
}


public class EndGameState : GameState
{
    public EndGameState(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }


    public override void stateBehavior()
    {
        throw new System.NotImplementedException();
    }
}

    



