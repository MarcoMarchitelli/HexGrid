using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandDisplay : MonoBehaviour
{
    [Header("Cards Buttons")]
    public ButtonController card1Button;
    public ButtonController card2Button;
    public ButtonController card3Button;

    [Header("Cards Quantities")]
    public TextMeshProUGUI card1QuantityText;
    public TextMeshProUGUI card2QuantityText;
    public TextMeshProUGUI card3QuantityText;

    public void RefreshHandDisplay()
    {
        PlayerController player = GameManager.instance.currentActivePlayer;
        player.SetNumberOfCardTypesInHand();

        int card1Number = player.numberOfCards1InHand;
        int card2Number = player.numberOfCards2InHand;
        int card3Number = player.numberOfCards3InHand;

        card1QuantityText.text = card1Number.ToString();
        card2QuantityText.text = card2Number.ToString();
        card3QuantityText.text = card3Number.ToString();

        if (card1Number <= 0)
        {
            card1Button.SetUsability(false);
            card1Button.SetSprite(ButtonController.SpriteType.inactive);
        }
        else
        {
            card1Button.SetUsability(true);
            card1Button.SetSprite(ButtonController.SpriteType.active);
        }

        if (card2Number <= 0 || player.selectedCard)
        {
            card2Button.SetUsability(false);
            card2Button.SetSprite(ButtonController.SpriteType.inactive);
        }
        else
        {
            card2Button.SetUsability(true);
            card2Button.SetSprite(ButtonController.SpriteType.active);
        }

        if (card3Number <= 0 || player.selectedCard)
        {
            card3Button.SetUsability(false);
            card3Button.SetSprite(ButtonController.SpriteType.inactive);
        }
        else
        {
            card3Button.SetUsability(true);
            card3Button.SetSprite(ButtonController.SpriteType.active);
        }
    }

    public void SelectCard(int cardType)
    {
        PlayerController currentPlayer = GameManager.instance.currentActivePlayer;
        List<CardController> cards = currentPlayer.cardsInHand;

        foreach (CardController card in cards)
        {
            switch (cardType)
            {
                case 1:
                    if (card.type == CardController.Type.card1)
                    {
                        currentPlayer.SelectCard(card);
                        return;
                    }
                    break;
                case 2:
                    if (card.type == CardController.Type.card2)
                    {
                        currentPlayer.SelectCard(card);
                        return;
                    }
                    break;
                case 3:
                    if (card.type == CardController.Type.card3)
                    {
                        currentPlayer.SelectCard(card);
                        return;
                    }
                    break;
            }
        }

    }

}
