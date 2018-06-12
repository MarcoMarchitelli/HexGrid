using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CardsManager : MonoBehaviour
{

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

    public IEnumerator GainPhase(PlayerController player)
    {
        foreach (CardController card in PlacedCards)
        {
            if (card.player == player)
            {
                card.ResourcesPopUpAnimation();
                player.EnergyPoints += card.extractableEnergy;
                player.bonusMoveActions += card.moveHexTouched;
                player.actions += card.abilityHexTouched;
                GameManager.instance.hudManager.Refresh();
                yield return StartCoroutine(card.WaitForResourcePopUp());
            }
        }
        GameManager.instance.gainPhaseEnded = true;
    }

    public void HighlightPlacedCards(List<Hexagon> nearHexagons, bool flag)
    {
        if (flag)
            foreach (var hexagon in nearHexagons)
            {
                if (PlacedCards.Contains(hexagon.card))
                    hexagon.card.outlineController.EnableOutline(true);
            }
        else
            foreach (var hexagon in nearHexagons)
            {
                if (PlacedCards.Contains(hexagon.card))
                    hexagon.card.outlineController.EnableOutline(false);
            }
    }

}
