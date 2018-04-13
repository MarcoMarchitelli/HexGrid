using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardShop : MonoBehaviour {

    [Header("Buy Buttons")]
    public Button buyCard1;
    public Button buyCard2;
    public Button buyCard3;

    int playerEnergy;

    [HideInInspector]
    public List<int> cardsBought = new List<int>();

    [Header("Card Prices")]
    public int card1Price;
    public int card2Price;
    public int card3Price;

    public void ToggleBuyButtons()
    {
        playerEnergy = GameManager.instance.currentActivePlayer.energyPoints;
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

        ToggleBuyButtons();
    }

}
