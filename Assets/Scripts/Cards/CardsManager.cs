using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour {

    public List<CardController> PlacedCards = new List<CardController>();

    public void StartRotationAnimations()
    {
        foreach (CardController card in PlacedCards)
        {
            StartCoroutine(card.RotateRight());
        }
    }

    public bool AllRotationAnimationsFinished()
    {
        foreach (CardController card in PlacedCards)
        {
            if (!card.rotateRightFlowFinished)
                return false;
        }

        return true;

    }

    public void GainPhase(PlayerController player)
    {
        foreach (CardController card in PlacedCards)
        {
            if (card.player == player)
            {
                card.ResourcesPopUpAnimation();
                player.energyPoints += card.extractableEnergy;
                player.bonusMoveActions += card.moveHexTouched;
                player.actions += card.abilityHexTouched;
            }   
        }
        GameManager.instance.gainPhaseEnded = true;
    }

}
