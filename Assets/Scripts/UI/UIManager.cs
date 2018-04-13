using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Public Variables

    public TextMeshProUGUI topLeftSection, topRightSection, leftMediumSection, bigCentralSection;
    public GameObject winOverlay, cardShop;
    public GameObject[] cardButtons;
    public Button moveButton, betButton, placeCardButton, rotateCardButton, sellCardButton, buyCardButton, undoMovesButton, endTurnButton, confirmButton, undoButton;
    public RectTransform modifiersSection;

    [HideInInspector]
    public CardShop cardShopScript;

    void Start()
    {
        cardShopScript = cardShop.GetComponent<CardShop>();
    }

    #endregion

    #region Print Functions

    public void PrintTopLeft(string msg)
    {
        topLeftSection.text = msg;
    }

    public void PrintTopRight(string msg)
    {
        topRightSection.text = msg;
    }

    public void PrintLeft(string msg)
    {
        leftMediumSection.text = msg;
    }

    public void PrintBigNews(string msg)
    {
        bigCentralSection.text = msg;
    }

    public void PrintPlayersModifiers()
    {
        PlayerController[] players = GameManager.instance.players;

        TextMeshProUGUI playerType, playerWeakness, playerStrength;

        for (int i = 0; i < players.Length; i++)
        {
            playerType = modifiersSection.GetChild(i).GetComponent<TextMeshProUGUI>();
            playerWeakness = playerType.rectTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerStrength = playerType.rectTransform.GetChild(1).GetComponent<TextMeshProUGUI>();

            playerType.text = players[i].type.ToString();
            playerWeakness.text = players[i].weaknessType.ToString();
            playerStrength.text = players[i].strenghtType.ToString();
        }
    }

    #endregion

    #region Action Button Toggle

    public void ToggleMove(PlayerController player)
    {
        if (player.actions <= 0)
        {
            moveButton.enabled = false;
            moveButton.image.color = Color.red;
        }
        else
        {
            moveButton.enabled = true;
            moveButton.image.color = Color.green;
        }
    }

    public void ToggleBet(PlayerController player)
    {
        player.playersToRob = GameManager.instance.FindPlayersInRange(2, player);

        if (player.playersToRob.Count == 0 || player.hasBet || player.actions <= 0)
        { 
            betButton.enabled = false;
            betButton.image.color = Color.red;
            player.canBet = false;
        }
        else if (player.playersToRob.Count > 0 && !player.hasBet && player.energyPoints >= 1)
        {
            betButton.enabled = true;
            betButton.image.color = Color.green;
            player.canBet = true;
        }

    }

    public void TogglePlaceCardButton(PlayerController player)
    {
        if(player.actions <= 0)
        {
            placeCardButton.enabled = false;
            placeCardButton.image.color = Color.red;
        }
        else
        {
            placeCardButton.enabled = true;
            placeCardButton.image.color = Color.green;
        }
    }

    public void ToggleRotateCardButton(PlayerController player)
    {
        if (player.actions <= 0)
        {
            rotateCardButton.enabled = false;
            rotateCardButton.image.color = Color.red;
        }
        else
        {
            rotateCardButton.enabled = true;
            rotateCardButton.image.color = Color.green;
        }
    }

    public void ToggleBuyCardButton(PlayerController player)
    {
        if (player.actions <= 0)
        {
            buyCardButton.enabled = false;
            buyCardButton.image.color = Color.red;
        }
        else
        {
            buyCardButton.enabled = true;
            buyCardButton.image.color = Color.green;
        }
    }

    public void ToggleSellCardButton(PlayerController player)
    {
        if (player.actions <= 0)
        {
            sellCardButton.enabled = false;
            sellCardButton.image.color = Color.red;
        }
        else
        {
            sellCardButton.enabled = true;
            sellCardButton.image.color = Color.green;
        }
    }

    #endregion

    #region Others Button Toggle

    public void ToggleUndoMoves(PlayerController player)
    {
        if (undoMovesButton.gameObject.activeSelf)
        {
            if (player.possibleMoves == player.beforeMoveActionMoves)
            {
                undoMovesButton.enabled = false;
                undoMovesButton.image.color = Color.red;
            }
            else
            {
                undoMovesButton.enabled = true;
                undoMovesButton.image.color = Color.green;
            }
        }
    }

    //public void DisplayHand(PlayerController activePlayer)
    //{
    //    for (int i = 0; i < activePlayer.cards.Length; i++)
    //    {
    //        if (activePlayer.cards[i].GetComponent<CardController>().state != CardController.State.inHand)
    //        {
    //            cardButtons[i].GetComponent<Image>().color = Color.red;
    //            cardButtons[i].GetComponent<Button>().enabled = false;
    //        }
    //        else
    //        {
    //            cardButtons[i].GetComponent<Image>().color = Color.green;
    //            cardButtons[i].GetComponent<Button>().enabled = true;
    //        }
    //    }

    //    if (activePlayer.selectedCard || activePlayer.hasUsedAbility || activePlayer.currentAction == PlayerController.Action.bet)
    //    {
    //        for (int i = 0; i < cardButtons.Length; i++)
    //        {
    //            cardButtons[i].GetComponent<Button>().enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < cardButtons.Length; i++)
    //        {
    //            if (activePlayer.cards[i].GetComponent<CardController>().state == CardController.State.inHand)
    //            {
    //                cardButtons[i].GetComponent<Button>().enabled = true;
    //            }
    //        }
    //    }
    //}

    public void ToggleEndTurnButton(PlayerController player)
    {
        if (player.currentAction == PlayerController.Action.bet || player.selectedCard != null && !player.hasUsedAbility)
        {
            endTurnButton.enabled = false;
        }
        else
        {
            endTurnButton.enabled = true;
        }
    }

    #endregion

    #region Action Button Press

    public void OnMoveButton()
    {
        //specific UI to activate
        undoMovesButton.gameObject.SetActive(true);


        SetTrueConfirmUndoButtons();

        GameManager.instance.ChoseAction(0);
    }

    public void OnBuyButton()
    {
        //specific UI stuff
        cardShop.SetActive(true);
        if(cardShopScript)
            cardShopScript.ToggleBuyButtons();


        SetTrueConfirmUndoButtons();

        GameManager.instance.ChoseAction(1);
    }

    public void OnBetButton()
    {
        SetTrueConfirmUndoButtons();

        GameManager.instance.ChoseAction(5);
    }

    #endregion

    public void OnConfirmButton()
    {
        SetFalseAllSpecificActionUI();

        //gameplay stuff
        GameManager.instance.ConfirmAction();
    }

    public void OnUndoButton()
    {
        SetFalseAllSpecificActionUI();

        //gameplay stuff
        GameManager.instance.UndoAction();
    }

    public void SubscribeToPlayerUIRefreshEvent(PlayerController player)
    {
        //action buttons
        player.UIrefresh += ToggleBet;
        player.UIrefresh += TogglePlaceCardButton;
        player.UIrefresh += ToggleRotateCardButton;
        player.UIrefresh += ToggleBuyCardButton;
        player.UIrefresh += ToggleSellCardButton;
        player.UIrefresh += ToggleMove;

        //other buttons
        //player.UIrefresh += DisplayHand;
        player.UIrefresh += ToggleUndoMoves;
        player.UIrefresh += ToggleEndTurnButton;

        //infos
        player.UIrefresh += RefreshAllPrintFunctions;
    }

    public void UnsubscribeToPlayerUIRefreshEvent(PlayerController player)
    {
        //action buttons
        player.UIrefresh -= ToggleBet;
        player.UIrefresh -= TogglePlaceCardButton;
        player.UIrefresh -= ToggleRotateCardButton;
        player.UIrefresh -= ToggleBuyCardButton;
        player.UIrefresh -= ToggleSellCardButton;
        player.UIrefresh -= ToggleMove;

        //other buttons
        //player.UIrefresh -= DisplayHand;
        player.UIrefresh -= ToggleUndoMoves;
        player.UIrefresh -= ToggleEndTurnButton;

        //infos
        player.UIrefresh -= RefreshAllPrintFunctions;
    }

    public void RefreshAllPrintFunctions(PlayerController player)
    {
        string msg = "It's the " + player.type.ToString() + " player's turn.";
        PrintTopLeft(msg);

        if(player.currentAction == PlayerController.Action.start)
        {
            msg = "Chose an action to make!\n" + player.actions + " actions remaining.";
            PrintLeft(msg);
        }
        else if (player.currentAction == PlayerController.Action.moving)
        {
            msg = "You have " + player.possibleMoves + " moves remaining!";
            if (player.canBet)
            {
                msg += "\nAnd you can bet!";
            }
            PrintLeft(msg);
        }
        else if (player.currentAction == PlayerController.Action.placeCard)
        {
            if (!player.hasUsedAbility && player.selectedCard && player.selectedCard.state == CardController.State.selectedFromHand)
            {
                msg = "Use A/D to rotate the card. \nLeftclick to place it. \nRightclick to undo.";
            }
            else
                if (!player.hasUsedAbility && player.selectedCard && player.selectedCard.state == CardController.State.selectedFromMap)
            {
                msg = "Use A/D to rotate the card.\nLeftclick to place it.\nSpace to return it to it's owner's hand.\nRightclick to undo.";
            }
            else
                if (!player.hasUsedAbility && !player.selectedCard)
            {
                msg = "Select a card from your hand,\nor from the map.";
            }
            else
                if (player.canBet)
            {
                msg = "You can bet!";
            }
            else
            {
                msg = "You've ended your actions for this turn. \nLet the other players have fun too!";
            }

            PrintLeft(msg);
        }
        else if(player.currentAction == PlayerController.Action.bet)
        {
            PrintLeft("Select a player to attack.\nRightclick to undo.");
        }

    }

    public void Win(PlayerController player)
    {
        winOverlay.SetActive(true);
        winOverlay.GetComponentInChildren<TextMeshProUGUI>().text = player.type.ToString() + " Wins !";
    }

    public void ToggleModifiersDisplay()
    {
        if (modifiersSection.gameObject.activeSelf)
            modifiersSection.gameObject.SetActive(false);
        else
            modifiersSection.gameObject.SetActive(true);
    }

    void SetFalseAllSpecificActionUI()
    {
        undoMovesButton.gameObject.SetActive(false);
        cardShop.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        undoButton.gameObject.SetActive(false);
    }

    void SetTrueConfirmUndoButtons()
    {
        confirmButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
