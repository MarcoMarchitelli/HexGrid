using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour {

    public List<CardController> PlacedCards = new List<CardController>();

    public void RotationPhase()
    {
        foreach (CardController card in PlacedCards)
        {
            card.RotateRight();
        }
    }

}
