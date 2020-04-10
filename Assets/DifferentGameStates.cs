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

        

    }

    public override void Enter()
    {
        base.Enter();
        gameStateManager.FlipANewDummyActiveCard();
        while (gameStateManager.DealCardToLoot())
        {
            Debug.Log("DealCardToLoot");
        }
        gameStateManager.RefreshDummyAI();
        

    }
    public override void Leave()
    {
        base.Leave();

    }

}



