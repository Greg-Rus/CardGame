using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

public enum Players
{
    Top,
    Bottom

}

public class GameManager : MonoBehaviour
{

    public UiView UiView;
    public CardView CardPrefab;

    public CardSlotsView TopSlots;
    public CardSlotsView BottomSlots;

    public int Goal = 21;

    [Inject]
    private PlayerFsm _topPlayerFsm;
    [Inject]
    private PlayerFsm _bottomPlayerFsm;

    private ReactiveCommand TopPlayerEndTurn;
    private ReactiveCommand BottomPlayerEndTurn;
    private ReactiveCommand TopPlayerStand;
    private ReactiveCommand BottomPlayerStand;

    private Dictionary<Players, PlayerFsm> _playerFsMs;
    private Dictionary<Players, CardSlotsView> _cardSlots;
    private Dictionary<Players, ReactiveProperty<int>> _scores;
    private Players _currentPlayer;

    // Use this for initialization
    void Start ()
    {
        _cardSlots = new Dictionary<Players, CardSlotsView>();
        _cardSlots.Add(Players.Top, TopSlots);
        _cardSlots.Add(Players.Bottom, BottomSlots);

        _scores = new Dictionary<Players, ReactiveProperty<int>>();
        _scores.Add(Players.Top, new ReactiveProperty<int>(0));
        _scores.Add(Players.Bottom, new ReactiveProperty<int>(0));

        _topPlayerFsm.Player = Players.Top;
        _bottomPlayerFsm.Player = Players.Bottom;

        _playerFsMs = new Dictionary<Players, PlayerFsm>();
        _playerFsMs.Add(Players.Top, _topPlayerFsm);
        _playerFsMs.Add(Players.Bottom,  _bottomPlayerFsm);

        TopPlayerEndTurn = new ReactiveCommand(_topPlayerFsm.CurrentStateReactiveProperty
            .Select(state => state.ID == PlayerFSMStates.PlayerTurn && !TopSlots.MaxedOut));
        TopPlayerStand = new ReactiveCommand(_topPlayerFsm.CurrentStateReactiveProperty
            .Select(state => state.ID == PlayerFSMStates.PlayerTurn && !TopSlots.MaxedOut));
        BottomPlayerEndTurn = new ReactiveCommand(_bottomPlayerFsm.CurrentStateReactiveProperty
            .Select(state => state.ID == PlayerFSMStates.PlayerTurn && !BottomSlots.MaxedOut));
        BottomPlayerStand = new ReactiveCommand(_bottomPlayerFsm.CurrentStateReactiveProperty
            .Select(state => state.ID == PlayerFSMStates.PlayerTurn & !BottomSlots.MaxedOut));

        TopPlayerEndTurn.BindTo(UiView.TopPlayerEndTurn);
        TopPlayerStand.BindTo(UiView.TopPlayerStand);
        BottomPlayerEndTurn.BindTo(UiView.BottomPlayerEndTurn);
        BottomPlayerStand.BindTo(UiView.BottomPlayerStand);

        TopPlayerEndTurn.Subscribe(_ => EndPlayerTurn());
        TopPlayerStand.Subscribe(_ => Stand());
        BottomPlayerEndTurn.Subscribe(_ => EndPlayerTurn());
        BottomPlayerStand.Subscribe(_ => Stand());

        _topPlayerFsm.CurrentStateReactiveProperty.Subscribe(val => { Debug.Log("Tops state changed to: " + val); });
        _bottomPlayerFsm.CurrentStateReactiveProperty.Subscribe(val => { Debug.Log("Bottoms state changed to: " + val); });

        _scores[Players.Top].Subscribe(score => UiView.TopPlayerScoreText = score);
        _scores[Players.Bottom].Subscribe(score => UiView.BottomPlayerScoreText = score);
        _scores[Players.Top].Merge(_scores[Players.Bottom]).Subscribe(_ => EvaluateEndGame());

        SetupFsmSubscriptions(_topPlayerFsm);
        SetupFsmSubscriptions(_bottomPlayerFsm);

        _topPlayerFsm.SetStartingState(PlayerFSMStates.PlayerTurn);
        _currentPlayer = Players.Top;
        Debug.Log("Tops starting state set to: " +PlayerFSMStates.PlayerTurn);
        _bottomPlayerFsm.SetStartingState(PlayerFSMStates.OtherPlayerTurn);
        Debug.Log("Bottoms starting state set to: " + PlayerFSMStates.PlayerTurn);
    }

    private void SetupFsmSubscriptions(PlayerFsm playersFsm)
    {
        playersFsm.CurrentStateReactiveProperty
            .Where(state => state.ID.Equals(PlayerFSMStates.PlayerTurn))
            .Subscribe(_ =>
            {
                DealCardToPlayer(playersFsm.Player);
            });

        playersFsm.CurrentStateReactiveProperty
            .Where(state => state.ID.Equals(PlayerFSMStates.Stand))
            .Subscribe(_ => SwitchPlayers());

        playersFsm.CurrentStateReactiveProperty
            .Where(state => state.ID.Equals(PlayerFSMStates.EndTurn))
            .Subscribe(_ => SwitchPlayers());
    }

    // Update is called once per frame
	void Update () {
		
	}

    private void SwitchPlayers()
    {
        if (CurrentPlayerState.Equals(PlayerFSMStates.Stand) && NextPlayerState.Equals(PlayerFSMStates.Stand))
        {
            EvaluateEndGame();
        }
        else if (CurrentPlayerState.Equals(PlayerFSMStates.Stand))
        {
            _currentPlayer = NextPlayer;
            CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PlayerTurn);
        }
        else if (NextPlayerState.Equals(PlayerFSMStates.Stand))
        {
            CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PlayerTurn);
        }
        else
        {
            CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PassControllToOtherPlayer);
            _currentPlayer = NextPlayer;
            CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PlayerTurn);
        }
    }

    private PlayerFsm CurrentPlayerFsm
    {
        get { return _playerFsMs[_currentPlayer]; }
    }
    private PlayerFsm NextPlayerFsm
    {
        get { return _playerFsMs[NextPlayer]; }
    }

    private PlayerFSMStates CurrentPlayerState
    {
        get { return CurrentPlayerFsm.CurrentStateID; }
    }

    private PlayerFSMStates NextPlayerState
    {
        get { return NextPlayerFsm.CurrentStateID; }
    }

    private Players NextPlayer
    {
        get { return _currentPlayer.Equals(Players.Top) ? Players.Bottom : Players.Top; }
    }

    private void EndPlayerTurn()
    {
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.EndTurn);
    }

    private void PassControllToOtherPlayer()
    {
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PassControllToOtherPlayer);
    }

    private void Stand()
    {
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.Stand);
    }

    private void DealCardToCurrentPlayer()
    {
        DealCardToPlayer(_currentPlayer);
    }

    private void DealCardToPlayer(Players player)
    {
        Debug.Log("Dealing to " + player);
        var card = Instantiate(CardPrefab);
        var cardValue = Random.Range(2, 9);
        card.CardText = cardValue;
        _cardSlots[player].AddCard(card.Rect);
        _scores[player].Value += cardValue;
    }

    private void EvaluateEndGame()
    {
        if (_scores[Players.Top].Value == Goal && _scores[Players.Bottom].Value == Goal)
        {
            Debug.Log("Ending. Tied!");
        }
        else if (_scores[_currentPlayer].Value == Goal && NextPlayerState.Equals(PlayerFSMStates.Stand))
        {
            Debug.Log("Ending. Won by: " + _currentPlayer);
        }
        else if (CurrentPlayerState.Equals(PlayerFSMStates.Stand) && NextPlayerState.Equals(PlayerFSMStates.Stand))
        {
            DeclareWinnerForBestScore();
        }
        else if (TopSlots.MaxedOut && BottomSlots.MaxedOut)
        {
            DeclareWinnerForBestScore();
        }
    }

    private void DeclareWinnerForBestScore()
    {
        var topDif = Math.Abs(_scores[Players.Top].Value - Goal);
        var bottomDif = Math.Abs(_scores[Players.Bottom].Value - Goal);
        if (topDif == bottomDif)
        {
            Debug.Log("Ending. Tied!");
        }
        else
        {
            var winner = topDif < bottomDif ? Players.Top : Players.Bottom;
            Debug.Log("Ending. Won by: " + winner);
        }
    }
}
