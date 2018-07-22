using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {

    [SerializeField] private Text _cardText;
    private int _cardValue;
    public int CardText
    {
        get { return _cardValue; }
        set {
            _cardText.text = value.ToString();
            _cardValue = value;
        }
    }

    public void SetParent(GameObject parent)
    {
        transform.parent = parent.transform;
    }

}
