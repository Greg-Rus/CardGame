using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiView : MonoBehaviour
{

    public Button PlayerEndTurn;
    public Button PlayerStand;
    public Button OponentEndTurn;
    public Button OponentStand;

    public Text PlayerScore;
    public int PlayerScoreText
    {
        set { PlayerScore.text = value.ToString(); }
    }

    public Text OponentScore;
    public int OponentScoreText
    {
        set { OponentScore.text = value.ToString(); }
    }
}
