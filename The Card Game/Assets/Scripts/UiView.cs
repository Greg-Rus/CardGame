using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiView : MonoBehaviour
{

    public Button TopPlayerEndTurn;
    public Button TopPlayerStand;
    public Button BottomPlayerEndTurn;
    public Button BottomPlayerStand;

    public Text TopPlayerScore;
    public int TopPlayerScoreText
    {
        set { TopPlayerScore.text = value.ToString(); }
    }

    public Text BottomPlayerScore;
    public int BottomPlayerScoreText
    {
        set { BottomPlayerScore.text = value.ToString(); }
    }
}
