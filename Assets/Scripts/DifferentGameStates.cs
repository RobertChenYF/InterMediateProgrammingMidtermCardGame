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
        gameStateManager.AssignAllBadges();

    }
    public override void Leave()
    {
        base.Leave();
        
    }


}//make all cards in the beginning

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

}//refill the loot pile

public class PlayerTurn : GameState
{
    
    public PlayerTurn(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        if (GameStateManager.SelectedCard == true)
        {
            GameStateManager.canInteract = false;
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
}//player's turn

public class DummyTurn : GameState
{
   
    private List<GameObject> gameObjects;
    public DummyTurn(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {
        
        
       
        if (gameObjects.Count == 0 && gameStateManager.CardsClaimedByDummy.Count>0)
        {
            gameStateManager.ChangeState(new PlayerPickFromDummyCard(gameStateManager));
            Debug.Log("cannot take any cards");
        }
        else if (gameObjects.Count == 0 && gameStateManager.CardsClaimedByDummy.Count == 0)
        {
            gameStateManager.ChangeState(new Refill(gameStateManager));
            Debug.Log("cannot take any cards, but there is not card for player to take");
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
}//dummy's turn

public class PlayerPickFromDummyCard : GameState
{

    
    public PlayerPickFromDummyCard(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }

    public override void stateBehavior()
    {

        if (GameStateManager.specialSituation == false)// after player pick a card go to the refill state
        {
            gameStateManager.ChangeState(new Refill(gameStateManager));
        }
    }


    public override void Enter()
    {
        base.Enter();
        GameStateManager.specialSituation = true;
        gameStateManager.OpenPlayerPickWindow();
    }

    public override void Leave()
    {
        base.Leave();
        gameStateManager.closePlayerPickWindow();
    }
}//special situation which player can pick a card from dummy
public class EndGameState : GameState
{
    public EndGameState(GameStateManager theGameStateManager) : base(theGameStateManager)
    {

    }


    public override void stateBehavior()
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        gameStateManager.delayInvoke();
        
    }

    public void EndState()
    {
        
    }
}//end game window

    



