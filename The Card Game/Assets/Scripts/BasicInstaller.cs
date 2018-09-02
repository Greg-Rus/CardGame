using Zenject;
using UnityEngine;
using System.Collections;

public class BasicInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<int>().FromInstance(1);
        Container.Bind<PlayerTurn>().AsTransient().NonLazy();
        Container.Bind<PlayCard>().AsTransient().NonLazy();
        Container.Bind<EndedTurn>().AsTransient().NonLazy();
        Container.Bind<Stands>().AsTransient().NonLazy();
        Container.Bind<OtherPlayerTurn>().AsTransient().NonLazy();

        Container.Bind<PlayerFsm>().FromFactory<FsmFactory>().AsTransient().NonLazy();
    }

    //private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> MakeFSM()
    //{
    //    var FSM = new FSMSystem<PlayerFSMTransitions, PlayerFSMStates>();
    //    var playerTurnState = new PlayerTurn();
    //    playerTurnState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
    //    playerTurnState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
    //    playerTurnState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
    //    FSM.AddState(playerTurnState);

    //    var playCardState = new PlayCard();
    //    playCardState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
    //    playCardState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
    //    playCardState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
    //    FSM.AddState(playCardState);

    //    var endTurnState = new EndedTurn();
    //    endTurnState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
    //    FSM.AddState(endTurnState);

    //    var standState = new Stands();
    //    standState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
    //    FSM.AddState(standState);

    //    var otherPlayerTurn = new OtherPlayerTurn();
    //    otherPlayerTurn.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
    //    FSM.AddState(otherPlayerTurn);

    //    return FSM;
    //}


}

public class FsmFactory : IFactory<PlayerFsm>
{

    public PlayerFsm Create()
    {
        var FSM = new PlayerFsm();
        var playerTurnState = new PlayerTurn();
        playerTurnState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
        playerTurnState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
        playerTurnState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
        FSM.AddState(playerTurnState);

        var playCardState = new PlayCard();
        playCardState.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
        FSM.AddState(playCardState);

        var endTurnState = new EndedTurn();
        endTurnState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
        FSM.AddState(endTurnState);

        var standState = new Stands();
        standState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
        FSM.AddState(standState);

        var otherPlayerTurn = new OtherPlayerTurn();
        otherPlayerTurn.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
        FSM.AddState(otherPlayerTurn);

        return FSM;
    }
}
