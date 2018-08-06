using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class FsmTest
{

    private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> _fsm;

    [SetUp]
    public void Setup()
    {
        _fsm = MakeFSM();

    }

    [TearDown]
    public void TearDown()
    {

    }

	[Test]
	public void FsmTestSimplePasses() {
		Assert.AreEqual(1+1, 2);
	}

    [Test]
    public void FsmTestBasicCycle()
    {
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.PlayCard);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayCard);
        _fsm.PerformTransition(PlayerFSMTransitions.PlayCard);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayCard);
        _fsm.PerformTransition(PlayerFSMTransitions.EndTurn);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.EndTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.PassControllToOtherPlayer);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.OtherPlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.StartPlayerTurn);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.PlayerTurn);
        _fsm.PerformTransition(PlayerFSMTransitions.Stand);
        Assert.IsTrue(_fsm.CurrentState.ID == PlayerFSMStates.Stand);
    }

    private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> MakeFSM()
    {
        var FSM = new FSMSystem<PlayerFSMTransitions, PlayerFSMStates>();
        var playerTurnState = new PlayerTurn();
        playerTurnState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
        playerTurnState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
        playerTurnState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
        FSM.AddState(playerTurnState);

        var playCardState = new PlayCard();
        playCardState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
        playCardState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
        playCardState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
        FSM.AddState(playCardState);

        var endTurnState = new EndedTurn();
        endTurnState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
        FSM.AddState(endTurnState);

        var standState = new Stands();
        standState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
        FSM.AddState(standState);

        var otherPlayerTurn = new OtherPlayerTurn();
        otherPlayerTurn.AddTransition(PlayerFSMTransitions.StartPlayerTurn, PlayerFSMStates.PlayerTurn);
        FSM.AddState(otherPlayerTurn);

        return FSM;
    }
}
