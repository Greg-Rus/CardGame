public class PlayerFsm : FSMSystem<PlayerFSMTransitions, PlayerFSMStates>
{
    public Players Player;

    public PlayerFsm(PlayerTurn playerTurnState, PlayCard playCardState, EndedTurn endTurnState, Stands standState,
        OtherPlayerTurn otherPlayerTurn)
    {
        playerTurnState.AddTransition(PlayerFSMTransitions.PlayCard, PlayerFSMStates.PlayCard);
        playerTurnState.AddTransition(PlayerFSMTransitions.EndTurn, PlayerFSMStates.EndTurn);
        playerTurnState.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
        AddState(playerTurnState);

        playCardState.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
        AddState(playCardState);

        endTurnState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
        endTurnState.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
        AddState(endTurnState);

        standState.AddTransition(PlayerFSMTransitions.PassControllToOtherPlayer, PlayerFSMStates.OtherPlayerTurn);
        AddState(standState);

        otherPlayerTurn.AddTransition(PlayerFSMTransitions.PlayerTurn, PlayerFSMStates.PlayerTurn);
        otherPlayerTurn.AddTransition(PlayerFSMTransitions.Stand, PlayerFSMStates.Stand);
        AddState(otherPlayerTurn);
    }
}