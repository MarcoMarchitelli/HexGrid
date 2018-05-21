using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour {

    public List<CardController> PlacedCards = new List<CardController>();

    private void Start()
    {
        GameManager.instance.OnRotationPhase += RotationPhase;
    }

    public void RotationPhase()
    {
        foreach (CardController card in PlacedCards)
        {
            card.RotateRight();
        }
    }

    public void GainPhase(PlayerController player)
    {
        foreach (CardController card in PlacedCards)
        {
            if (card.player == player)
            {
                player.energyPoints += card.extractableEnergy;
                player.bonusMoveActions += card.moveHexTouched;
                player.actions += card.abilityHexTouched;
            }   
        }
    }

}
