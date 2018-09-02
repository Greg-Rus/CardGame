using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotsView : MonoBehaviour
{
    public RectTransform[] Slots;

    public int OccupiedSlots = 0;
    public bool MaxedOut = false;

    public void AddCard(RectTransform card)
    {
        if (MaxedOut)
        {
            Debug.LogError("Slots full on " + gameObject.name);
            return;
        }

        if (OccupiedSlots == Slots.Length -1) MaxedOut = true;
        card.SetParent(Slots[OccupiedSlots++],false);
        Debug.Log("Assigned card to " + gameObject.name);
    }
}
