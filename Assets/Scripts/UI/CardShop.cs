using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardShop : MonoBehaviour {

    [Header("Buy Buttons")]
    public Button buyCard1;
    public Button buyCard2;
    public Button buyCard3;

    int playerEnergy;
    int card1Price;
    int card2Price;
    int card3Price;

    [HideInInspector]
    public List<int> cardsBought = new List<int>();

    [Header("Card Prices Texts")]
    public TextMeshProUGUI card1PriceText;
    public TextMeshProUGUI card2PriceText;
    public TextMeshProUGUI card3PriceText;

    public void ToggleBuyButtons(PlayerController player)
    {
        
        if(GameManager.instance.turnCount <= 3)
        {
            card1Price = 1;
            card2Price = 2;
            card3Price = 3;
        }
        else
        {
            card1Price = 2;
            card2Price = 4;
            card3Price = 6;
        }

        if (player.hasDiscount)
        {
            card1Price = 0;
        }

        card1PriceText.text = card1Price.ToString() + " PE";
        card2PriceText.text = card2Price.ToString() + " PE";
        card3PriceText.text = card3Price.ToString() + " PE";

        playerEnergy = player.energyPoints;

        if (player.hasBought)
        {
            buyCard3.enabled = false;
            buyCard3.image.color = Color.red;
            buyCard2.enabled = false;
            buyCard2.image.color = Color.red;
            buyCard1.enabled = false;
            buyCard1.image.color = Color.red;
        }else
        if (playerEnergy >= card3Price)
        {
            buyCard3.enabled = true;
            buyCard3.image.color = Color.green;
            buyCard2.enabled = true;
            buyCard2.image.color = Color.green;
            buyCard1.enabled = true;
            buyCard1.image.color = Color.green;
        }
        else
        if (playerEnergy >= card2Price)
        {
            buyCard3.enabled = false;
            buyCard3.image.color = Color.red;

            buyCard2.enabled = true;
            buyCard2.image.color = Color.green;
            buyCard1.enabled = true;
            buyCard1.image.color = Color.green;
        }
        else
        if (playerEnergy >= card1Price)
        {
            buyCard3.enabled = false;
            buyCard3.image.color = Color.red;
            buyCard2.enabled = false;
            buyCard2.image.color = Color.red;

            buyCard1.enabled = true;
            buyCard1.image.color = Color.green;
        }
        else
        {
            buyCard3.enabled = false;
            buyCard3.image.color = Color.red;
            buyCard2.enabled = false;
            buyCard2.image.color = Color.red;
            buyCard1.enabled = false;
            buyCard1.image.color = Color.red;
        }
        
    }

    //aggiunge carte a cardsBought e gestisce pagamento
    public void BuyCard(int cardType)
    {
        switch (cardType)
        {
            case 1:
                cardsBought.Add(1);
                GameManager.instance.currentActivePlayer.energyPoints -= card1Price;
                if (GameManager.instance.currentActivePlayer.hasDiscount)
                {
                    GameManager.instance.currentActivePlayer.hasDiscount = false;
                }
                break;
            case 2:
                cardsBought.Add(2);
                GameManager.instance.currentActivePlayer.energyPoints -= card2Price;
                break;
            case 3:
                cardsBought.Add(3);
                GameManager.instance.currentActivePlayer.energyPoints -= card3Price;
                break;
        }

        GameManager.instance.currentActivePlayer.hasBought = true;

        ToggleBuyButtons(GameManager.instance.currentActivePlayer);
    }

}
