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

    [Inject]
    private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> _topPlayerFsm;
    [Inject]
    private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> _bottomPlayerFsm;

    private Queue<FSMSystem<PlayerFSMTransitions, PlayerFSMStates>> _playerQueue;

    private ReactiveCommand TopPlayerEndTurn;
    private ReactiveCommand BottomPlayerEndTurn;

    // Use this for initialization
    void Start ()
    {
        _playerQueue = new Queue<FSMSystem<PlayerFSMTransitions, PlayerFSMStates>>();
        _playerQueue.Enqueue(_topPlayerFsm);
        _playerQueue.Enqueue(_bottomPlayerFsm);

        TopPlayerEndTurn = new ReactiveCommand(_topPlayerFsm.CurrentStateReactiveProperty.Select(state => state.ID == PlayerFSMStates.PlayerTurn));
        BottomPlayerEndTurn = new ReactiveCommand(_bottomPlayerFsm.CurrentStateReactiveProperty.Select(state => state.ID == PlayerFSMStates.PlayerTurn));

        TopPlayerEndTurn.Subscribe(_ => { });
    }

    // Update is called once per frame
	void Update () {
		
	}

    private void SeitchPlayers()
    {
        var currentPlayer = _playerQueue.Dequeue();
        _playerQueue.Enqueue(currentPlayer);
    }

    private FSMSystem<PlayerFSMTransitions, PlayerFSMStates> CurrentPlayerFsm
    {
        get { return _playerQueue.Peek(); }
    }

    private void EndCurrentPlayerTurn()
    {
        CurrentPlayerFsm.PerformTransition(PlayerFSMTransitions.EndTurn);
    }
}
