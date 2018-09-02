using System;
using System.Collections;
using System.Collections.Generic;
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

    private Queue<PlayerFsm> _playerQueue;

    private ReactiveCommand TopPlayerEndTurn;
    private ReactiveCommand BottomPlayerEndTurn;
    private ReactiveCommand TopPlayerStand;
    private ReactiveCommand BottomPlayerStand;

    private Dictionary<Players, CardSlotsView> CardSlots;
    private Dictionary<Players, ReactiveProperty<int>> Scores;

    // Use this for initialization
    void Start ()
    {
        CardSlots = new Dictionary<Players, CardSlotsView>();
        CardSlots.Add(Players.Top, TopSlots);
        CardSlots.Add(Players.Bottom, BottomSlots);

        Scores = new Dictionary<Players, ReactiveProperty<int>>();
        Scores.Add(Players.Top, new ReactiveProperty<int>(0));
        Scores.Add(Players.Bottom, new ReactiveProperty<int>(0));

        _topPlayerFsm.Player = Players.Top;
        _bottomPlayerFsm.Player = Players.Bottom;

        _playerQueue = new Queue<PlayerFsm>();
        _playerQueue.Enqueue(_topPlayerFsm);
        _playerQueue.Enqueue(_bottomPlayerFsm);

        TopPlayerEndTurn = new ReactiveCommand(_topPlayerFsm.CurrentStateReactiveProperty.Select(state => state.ID == PlayerFSMStates.PlayerTurn));
        TopPlayerStand = new ReactiveCommand(_topPlayerFsm.CurrentStateReactiveProperty.Select(state => state.ID == PlayerFSMStates.PlayerTurn));
        BottomPlayerEndTurn = new ReactiveCommand(_bottomPlayerFsm.CurrentStateReactiveProperty.Select(state => state.ID == PlayerFSMStates.PlayerTurn));
        BottomPlayerStand = new ReactiveCommand(_bottomPlayerFsm.CurrentStateReactiveProperty.Select(state => state.ID == PlayerFSMStates.PlayerTurn));

        TopPlayerEndTurn.BindTo(UiView.OponentEndTurn);
        TopPlayerStand.BindTo(UiView.OponentStand);
        BottomPlayerEndTurn.BindTo(UiView.PlayerEndTurn);
        BottomPlayerStand.BindTo(UiView.PlayerStand);

        TopPlayerEndTurn.Subscribe(_ => EndPlayerTurn());
        TopPlayerStand.Subscribe(_ => Stand());
        BottomPlayerEndTurn.Subscribe(_ => EndPlayerTurn());
        BottomPlayerStand.Subscribe(_ => Stand());

        _topPlayerFsm.CurrentStateReactiveProperty.Subscribe(val => { Debug.Log("Tops state changed to: " + val); });
        _bottomPlayerFsm.CurrentStateReactiveProperty.Subscribe(val => { Debug.Log("Bottoms state changed to: " + val); });

        Scores[Players.Top].Subscribe(score => UiView.OponentScoreText = score);
        Scores[Players.Bottom].Subscribe(score => UiView.PlayerScoreText = score);
        Scores[Players.Top].Merge(Scores[Players.Bottom]).Subscribe(_ => EndGameIfWon());

        _topPlayerFsm.CurrentStateReactiveProperty.Where(state => state.ID.Equals(PlayerFSMStates.PlayerTurn))
            .Subscribe(_ => DealCardToPlayer(Players.Top));

        _bottomPlayerFsm.CurrentStateReactiveProperty.Where(state => state.ID.Equals(PlayerFSMStates.PlayerTurn))
            .Subscribe(_ => DealCardToPlayer(Players.Bottom));

        _topPlayerFsm.SetStartingState(PlayerFSMStates.PlayerTurn);
        _bottomPlayerFsm.SetStartingState(PlayerFSMStates.OtherPlayerTurn);
    }

    // Update is called once per frame
	void Update () {
		
	}

    private void SwitchPlayers()
    {
        var currentPlayer = _playerQueue.Dequeue();
        _playerQueue.Enqueue(currentPlayer);

        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PlayerTurn);
    }

    private PlayerFsm CurrentPlayerFsm
    {
        get { return _playerQueue.Peek(); }
    }

    private Players CurrentPlayer
    {
        get { return CurrentPlayerFsm.Player; }
    }

    private void EndPlayerTurn()
    {
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.EndTurn);
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.PassControllToOtherPlayer);
        SwitchPlayers();
    }

    private void Stand()
    {
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.Stand);
    }

    private void DealCardToCurrentPlayer()
    {
        DealCardToPlayer(CurrentPlayer);
    }

    private void DealCardToPlayer(Players player)
    {
        var card = Instantiate(CardPrefab);
        var cardValue = Random.Range(2, 9);
        card.CardText = cardValue;
        CardSlots[player].AddCard(card.Rect);
        Scores[player].Value += cardValue;
        Debug.Log("Dealing to " + player);
    }

    private void EndGameIfWon()
    {
        if (TopSlots.MaxedOut && BottomSlots.MaxedOut)
        {
            var topDif = Math.Abs(Scores[Players.Top].Value - Goal);
            var BottomDif = Math.Abs(Scores[Players.Bottom].Value - Goal);
            var winner = topDif < BottomDif ? Players.Top : Players.Bottom;
            Debug.Log("Ending won by: " + winner);
        }
    }


}
