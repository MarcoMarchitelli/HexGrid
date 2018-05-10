using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandDisplay : MonoBehaviour
{

    [Header("Cards Buttons")]
    public Button card1Button;
    public Button card2Button;
    public Button card3Button;
    [Header("Cards Quantities")]
    public TextMeshProUGUI card1QuantityText;
    public TextMeshProUGUI card2QuantityText;
    public TextMeshProUGUI card3QuantityText;

    public void RefreshHandDisplay(PlayerController player)
    {
        player.SetNumberOfCardTypesInHand();

        int card1Number = player.numberOfCards1InHand;
        int card2Number = player.numberOfCards2InHand;
        int card3Number = player.numberOfCards3InHand;

        card1QuantityText.text = card1Number.ToString();
        card2QuantityText.text = card2Number.ToString();
        card3QuantityText.text = card3Number.ToString();

        if (card1Number <= 0 || player.selectedCard || player.hasPlacedCard)
        {
            card1Button.enabled = false;
            card1Button.image.color = Color.red;
        }
        else
        {
            card1Button.enabled = true;
            card1Button.image.color = Color.green;
        }

        if (card2Number <= 0 || player.selectedCard || player.hasPlacedCard)
        {
            card2Button.enabled = false;
            card2Button.image.color = Color.red;
        }
        else
        {
            card2Button.enabled = true;
            card2Button.image.color = Color.green;
        }

        if (card3Number <= 0 || player.selectedCard || player.hasPlacedCard)
        {
            card3Button.enabled = false;
            card3Button.image.color = Color.red;
        }
        else
        {
            card3Button.enabled = true;
            card3Button.image.color = Color.green;
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
