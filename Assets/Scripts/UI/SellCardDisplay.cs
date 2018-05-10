using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellCardDisplay : MonoBehaviour {

    [Header("Sell Buttons")]
    public Button sellButton1;
    public Button sellButton2;
    public Button sellButton3;

    [Header("Quantity Images")]
    public Image image1;
    public Image image2;
    public Image image3;

    int card1Value = 1;
    int card2Value = 2;
    int card3Value = 3;

    public void RefreshSellDisplay(PlayerController player)
    {
        image1.GetComponentInChildren<TextMeshProUGUI>().text = player.numberOfCards1InHand.ToString();
        image2.GetComponentInChildren<TextMeshProUGUI>().text = player.numberOfCards2InHand.ToString();
        image3.GetComponentInChildren<TextMeshProUGUI>().text = player.numberOfCards3InHand.ToString();

        image1.color = (player.numberOfCards1InHand <= 0 || player.hasSold) ? Color.red : Color.green;
        image2.color = (player.numberOfCards2InHand <= 0 || player.hasSold) ? Color.red : Color.green;
        image3.color = (player.numberOfCards3InHand <= 0 || player.hasSold) ? Color.red : Color.green;

        sellButton1.image.color = (player.numberOfCards1InHand <= 0 || player.hasSold) ? Color.red : Color.green;
        sellButton2.image.color = (player.numberOfCards2InHand <= 0 || player.hasSold) ? Color.red : Color.green;
        sellButton3.image.color = (player.numberOfCards3InHand <= 0 || player.hasSold) ? Color.red : Color.green;

        sellButton1.enabled = (player.numberOfCards1InHand <= 0 || player.hasSold) ? false : true;
        sellButton2.enabled = (player.numberOfCards2InHand <= 0 || player.hasSold) ? false : true;
        sellButton3.enabled = (player.numberOfCards3InHand <= 0 || player.hasSold) ? false : true;
    }

    public void SellCard(int cardIndex)
    {
        PlayerController player = GameManager.instance.currentActivePlayer;

        if (!player.hasSold)
        {
            foreach (CardController card in player.cardsInHand)
            {
                switch (cardIndex)
                {
                    case 1:
                        if (card.type == CardController.Type.card1)
                        {
                            player.cardToSell = card;
                            player.energyPoints += card1Value;
                            player.numberOfCards1InHand--;
                            player.hasSold = true;
                            RefreshSellDisplay(player);
                        }
                        break;
                    case 2:
                        if (card.type == CardController.Type.card2)
                        {
                            player.cardToSell = card;
                            player.energyPoints += card2Value;
                            player.numberOfCards2InHand--;
                            player.hasSold = true;
                            RefreshSellDisplay(player);
                        }
                        break;
                    case 3:
                        if (card.type == CardController.Type.card3)
                        {
                            player.cardToSell = card;
                            player.energyPoints += card3Value;
                            player.numberOfCards3InHand--;
                            player.hasSold = true;
                            RefreshSellDisplay(player);
                        }
                        break;
                }
            }
        }
    }

}
