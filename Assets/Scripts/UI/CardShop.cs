using UnityEngine;
using TMPro;

public class CardShop : MonoBehaviour
{

    [Header("Buy Buttons")]
    public ButtonController buyCard1;
    public ButtonController buyCard2;
    public ButtonController buyCard3;

    int playerEnergy;
    int card1Price;
    int card2Price;
    int card3Price;

    [Header("Card Prices Texts")]
    public TextMeshProUGUI card1PriceText;
    public TextMeshProUGUI card2PriceText;
    public TextMeshProUGUI card3PriceText;

    private void OnEnable()
    {
        ToggleBuyButtons(GameManager.instance.currentActivePlayer);
    }

    public void ToggleBuyButtons(PlayerController player)
    {

        if (GameManager.instance.turnCount <= 3)
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
            buyCard3.SetUsability(false);
            buyCard3.SetSprite(ButtonController.SpriteType.inactive);
            buyCard2.SetUsability(false);
            buyCard2.SetSprite(ButtonController.SpriteType.inactive);
            buyCard1.SetUsability(false);
            buyCard1.SetSprite(ButtonController.SpriteType.inactive);
        }
        else
        if (playerEnergy >= card3Price)
        {
            buyCard3.SetUsability(true);
            buyCard3.SetSprite(ButtonController.SpriteType.active);
            buyCard2.SetUsability(true);
            buyCard2.SetSprite(ButtonController.SpriteType.active);
            buyCard1.SetUsability(true);
            buyCard1.SetSprite(ButtonController.SpriteType.active);
        }
        else
        if (playerEnergy >= card2Price)
        {
            buyCard3.SetUsability(false);
            buyCard3.SetSprite(ButtonController.SpriteType.inactive);

            buyCard2.SetUsability(true);
            buyCard2.SetSprite(ButtonController.SpriteType.active);
            buyCard1.SetUsability(true);
            buyCard1.SetSprite(ButtonController.SpriteType.active);
        }
        else
        if (playerEnergy >= card1Price)
        {
            buyCard3.SetUsability(false);
            buyCard3.SetSprite(ButtonController.SpriteType.inactive);
            buyCard2.SetUsability(false);
            buyCard2.SetSprite(ButtonController.SpriteType.inactive);

            buyCard1.SetUsability(true);
            buyCard1.SetSprite(ButtonController.SpriteType.active);
        }
        else
        {
            buyCard3.SetUsability(false);
            buyCard3.SetSprite(ButtonController.SpriteType.inactive);
            buyCard2.SetUsability(false);
            buyCard2.SetSprite(ButtonController.SpriteType.inactive);
            buyCard1.SetUsability(false);
            buyCard1.SetSprite(ButtonController.SpriteType.inactive);
        }

    }

    //aggiunge carte a cardsBought e gestisce pagamento
    public void BuyCard(int cardType)
    {
        switch (cardType)
        {
            case 1:
                Transform instantiatedCard1 = Instantiate(GameManager.instance.prefabCard1, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
                CardController card1Controller = instantiatedCard1.GetComponent<CardController>();
                if (card1Controller)
                {
                    GameManager.instance.currentActivePlayer.cardsInHand.Add(card1Controller);
                    card1Controller.player = GameManager.instance.currentActivePlayer;
                    GameManager.instance.currentActivePlayer.numberOfCards1InHand++;
                }
                GameManager.instance.currentActivePlayer.energyPoints -= card1Price;
                if (GameManager.instance.currentActivePlayer.hasDiscount)
                {
                    GameManager.instance.currentActivePlayer.hasDiscount = false;
                }
                break;
            case 2:
                Transform instantiatedCard2 = Instantiate(GameManager.instance.prefabCard2, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
                CardController card2Controller = instantiatedCard2.GetComponent<CardController>();
                if (card2Controller)
                {
                    GameManager.instance.currentActivePlayer.cardsInHand.Add(card2Controller);
                    card2Controller.player = GameManager.instance.currentActivePlayer;
                    GameManager.instance.currentActivePlayer.numberOfCards1InHand++;
                }
                GameManager.instance.currentActivePlayer.energyPoints -= card2Price;
                break;
            case 3:
                Transform instantiatedCard3 = Instantiate(GameManager.instance.prefabCard1, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
                CardController card3Controller = instantiatedCard3.GetComponent<CardController>();
                if (card3Controller)
                {
                    GameManager.instance.currentActivePlayer.cardsInHand.Add(card3Controller);
                    card3Controller.player = GameManager.instance.currentActivePlayer;
                    GameManager.instance.currentActivePlayer.numberOfCards1InHand++;
                }
                GameManager.instance.currentActivePlayer.energyPoints -= card3Price;
                break;
        }

        GameManager.instance.currentActivePlayer.hasBought = true;

        GameManager.instance.SetPlayerToStart(true);

        ToggleBuyButtons(GameManager.instance.currentActivePlayer);
    }

}
