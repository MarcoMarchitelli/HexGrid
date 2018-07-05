using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CardsManager : MonoBehaviour
{

    public List<CardController> PlacedCards = new List<CardController>();

    int energyPointsContainer;
    int actionsContainer;
    int bonusMoveActionsContainer;

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
        energyPointsContainer = 0;
        actionsContainer = 0;
        bonusMoveActionsContainer = 0;

        foreach (CardController card in PlacedCards)
        {
            if (card.player == player)
            {
                card.ResourcesPopUpAnimation();
                energyPointsContainer += card.extractableEnergy;
                bonusMoveActionsContainer += card.moveHexTouched;
                actionsContainer += card.abilityHexTouched;
                GameManager.instance.hudManager.Refresh();
                yield return StartCoroutine(card.WaitForResourcePopUp());
            }
        }
        player.EnergyPoints += energyPointsContainer;
        player.BonusMoveActions += bonusMoveActionsContainer;
        player.Actions += actionsContainer;
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

    public void BlockAllPaths()
    {
        foreach (var placedCard in PlacedCards)
        {
            placedCard.BlockPaths(placedCard.hexImOn);
        }
    }

}
