using Zenject;
using UnityEngine;
using System.Collections;

public class BasicInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerTurn>().AsTransient().NonLazy();
        Container.Bind<PlayCard>().AsTransient().NonLazy();
        Container.Bind<EndedTurn>().AsTransient().NonLazy();
        Container.Bind<Stands>().AsTransient().NonLazy();
        Container.Bind<OtherPlayerTurn>().AsTransient().NonLazy();
        Container.Bind<PlayerFsm>().AsTransient().NonLazy();
    }
}

//public class FsmFactory : IFactory<PlayerTurn, PlayCard, EndedTurn, Stands, OtherPlayerTurn, PlayerFsm>
//{
//    public PlayerFsm Create(PlayerTurn playerTurnState, PlayCard playCardState, EndedTurn endTurnState, Stands standState,
//        OtherPlayerTurn otherPlayerTurn)
//    {
//        PlayerFsm fsm = new PlayerFsm();

//        playerTurnState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
//        playerTurnState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
//        playerTurnState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
//        fsm.AddState(playerTurnState);

//        playCardState.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
//        fsm.AddState(playCardState);

//        endTurnState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
//        fsm.AddState(endTurnState);

//        standState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
//        fsm.AddState(standState);

//        otherPlayerTurn.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
//        fsm.AddState(otherPlayerTurn);

//        return fsm;
//    }
//}
