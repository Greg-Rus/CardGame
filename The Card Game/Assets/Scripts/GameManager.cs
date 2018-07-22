using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum Participatns
{
    Player,
    Oponent
}

public class GameManager : MonoBehaviour
{

    public UiView UiView;
    public GameObject PlayersGrid;
    public GameObject OponentsGrid;
    public CardView CardPrefab;

    private ReactiveProperty<bool> PlayerStands = new ReactiveProperty<bool>(true);
    private ReactiveProperty<bool> OponentStands = new ReactiveProperty<bool>(true);
    private ReactiveProperty<bool> PlayerTurn = new ReactiveProperty<bool>(true);
    private ReactiveProperty<bool> OponentTurn = new ReactiveProperty<bool>(true);

    private ReactiveProperty<int> PlayersScore = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> OponentsScore = new ReactiveProperty<int>(0);

    private ReactiveCommand PlayerStand;
    private ReactiveCommand PlayerEndTurn;
    private ReactiveCommand OponentStand;
    private ReactiveCommand OponentEndTurn;

    private ReactiveCommand<Participatns> DrawCardForParticipant;
    // Use this for initialization
    void Start ()
    {

        SetupUiSubscriptions();
        SetupEndRoundConditions();
        SetupEndMatchSubscriptions();


    }

    private void SetupEndRoundConditions()
    {
        PlayerTurn.CombineLatest(OponentTurn, (playerTurn, oponentTurn) => !playerTurn && !oponentTurn)
            .Where(endRoundCriteriaMet => endRoundCriteriaMet)
            .Subscribe(_ => EndRound());
    }

    private void SetupEndMatchSubscriptions()
    {
        DrawCardForParticipant = new ReactiveCommand<Participatns>();
        DrawCardForParticipant.Subscribe(SpawnCardForPaticipant);
        DrawCardForParticipant.Where(partcipant => partcipant == Participatns.Player).Skip(8).Subscribe(_ => EndMatch());
        DrawCardForParticipant.Where(partcipant => partcipant == Participatns.Oponent).Skip(8).Subscribe(_ => EndMatch());
    }

    private void EndMatch()
    {
        SceneManager.LoadScene(0);
    }

    private void EndRound()
    {
        PlayerTurn.Value = PlayerStands.Value;
        OponentTurn.Value = OponentStands.Value;
        Debug.Log("End Round");
    }

    private void SetupUiSubscriptions()
    {
        PlayerStand = new ReactiveCommand(PlayerStands);
        PlayerStand.Subscribe(_ =>
        {
            OnPlayerStands();
            OnPlayerEndedTurn();
        });
        PlayerStand.BindTo(UiView.PlayerStand);

        OponentStand = new ReactiveCommand(OponentStands);
        OponentStand.Subscribe(_ =>
        {
            OnOponentStands();
            OnOponentEndedTurn();
        });
        OponentStand.BindTo(UiView.OponentStand);

        PlayerEndTurn = new ReactiveCommand(PlayerTurn);
        PlayerEndTurn.Subscribe(_ => OnPlayerEndedTurn());
        PlayerEndTurn.BindTo(UiView.PlayerEndTurn);

        OponentEndTurn = new ReactiveCommand(OponentTurn);
        OponentEndTurn.Subscribe(_ => OnOponentEndedTurn());
        OponentEndTurn.BindTo(UiView.OponentEndTurn);

        PlayersScore.Subscribe(score => UiView.PlayerScoreText = score);
        OponentsScore.Subscribe(score => UiView.OponentScoreText = score);
    }

    // Update is called once per frame
	void Update () {
		
	}

    private void OnPlayerStands()
    {
        PlayerStands.Value = false;
    }
    private void OnOponentStands()
    {
        OponentStands.Value = false;
    }

    private void OnPlayerEndedTurn()
    {
        PlayerTurn.Value = false;
        DrawCardForParticipant.Execute(Participatns.Player);

    }
    private void OnOponentEndedTurn()
    {
        OponentTurn.Value = false;
        DrawCardForParticipant.Execute(Participatns.Oponent);
    }

    private void SpawnCardForPaticipant(Participatns participant)
    {
        var newCard = Instantiate(CardPrefab);
        newCard.CardText = Random.Range(1, 10);
        switch (participant)
        {
            case Participatns.Player:
                newCard.SetParent(PlayersGrid);
                PlayersScore.Value += newCard.CardText;
                break;
            case Participatns.Oponent:
                newCard.SetParent(OponentsGrid);
                OponentsScore.Value += newCard.CardText;
                break;
            default:
                throw new ArgumentOutOfRangeException("participant", participant, null);
        }
    }


}
