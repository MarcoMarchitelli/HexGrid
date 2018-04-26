using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Public Variables

    [Header("Print Sections")]
    public TextMeshProUGUI topLeftSection;
    public TextMeshProUGUI topRightSection;
    public TextMeshProUGUI leftMediumSection;
    public TextMeshProUGUI bigCentralSection;

    public GameObject[] cardButtons;
    [Header("Action Buttons")]
    public Button moveButton;
    public Button buyCardButton;
    public Button sellCardButton;
    public Button placeCardButton;
    public Button rotateCardButton;
    public Button betButton;
    [Header("Other Buttons")]
    public Button undoMovesButton;
    public Button endTurnButton;
    public Button confirmButton;
    public Button undoButton;
    [Header("Other UI Elements")]
    public RectTransform modifiersSection;
    public GameObject winOverlay;
    public GameObject cardShop;
    public GameObject handDisplay;

    [HideInInspector]
    public CardShop cardShopScript;
    [HideInInspector]
    public HandDisplay handDisplayScript;

    #endregion

    void Start()
    {
        cardShopScript = cardShop.GetComponent<CardShop>();
        handDisplayScript = handDisplay.GetComponent<HandDisplay>();
    }

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

    #region Action Buttons Toggle

    public void ToggleMoveButton(PlayerController player)
    {
        //set color
        if (player.actions <= 0)
        {
            moveButton.image.color = Color.red;
        }
        else
        {
            moveButton.image.color = Color.green;
        }

        //set usability
        if(player.currentAction == PlayerController.Action.start && moveButton.image.color == Color.green)
        {
            moveButton.enabled = true;
        }
        else
        {
            moveButton.enabled = false;
        }
    }

    public void ToggleBetButton(PlayerController player)
    {
        player.playersToRob = GameManager.instance.FindPlayersInRange(2, player);

        //setColor
        if (player.playersToRob.Count == 0 || player.actions <= 0)
        {
            betButton.image.color = Color.red;
            player.canBet = false;
        }
        else if (player.playersToRob.Count > 0 && player.energyPoints >= 1)
        {
            betButton.image.color = Color.green;
            player.canBet = true;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && betButton.image.color == Color.green)
        {
            betButton.enabled = true;
        }
        else
        {
            betButton.enabled = false;
        }

    }

    public void TogglePlaceCardButton(PlayerController player)
    {
        //set color
        if (player.actions <= 0 || player.cardsInHand.Count <= 0)
        {
            placeCardButton.image.color = Color.red;
        }
        else
        {
            placeCardButton.image.color = Color.green;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && placeCardButton.image.color == Color.green)
        {
            placeCardButton.enabled = true;
        }
        else
        {
            placeCardButton.enabled = false;
        }
    }

    public void ToggleRotateCardButton(PlayerController player)
    {
        //set color
        if (player.actions <= 0 || !player.HasCardInNearHexagons())
        {
            rotateCardButton.image.color = Color.red;
        }
        else
        {
            rotateCardButton.image.color = Color.green;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && rotateCardButton.image.color == Color.green)
        {
            rotateCardButton.enabled = true;
        }
        else
        {
            rotateCardButton.enabled = false;
        }
    }

    public void ToggleBuyCardButton(PlayerController player)
    {
        //set color
        if (player.actions <= 0 || player.energyPoints < 2)
        {
            buyCardButton.image.color = Color.red;
        }
        else
        {
            buyCardButton.image.color = Color.green;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && buyCardButton.image.color == Color.green)
        {
            buyCardButton.enabled = true;
        }
        else
        {
            buyCardButton.enabled = false;
        }
    }

    public void ToggleSellCardButton(PlayerController player)
    {
        //set color
        if (player.actions <= 0 || player.cardsInHand.Count <= 0)
        {
            sellCardButton.image.color = Color.red;
        }
        else
        {
            sellCardButton.image.color = Color.green;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && sellCardButton.image.color == Color.green)
        {
            sellCardButton.enabled = true;
        }
        else
        {
            sellCardButton.enabled = false;
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

    public void ToggleEndTurnButton(PlayerController player)
    {
        if (player.currentAction != PlayerController.Action.start)
        if (player.currentAction != PlayerController.Action.start && GameManager.instance.mainCamera.isMoving)
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

        EnterAction();

        GameManager.instance.ChoseAction(0);
    }

    public void OnBuyButton()
    {
        //specific UI stuff
        cardShop.SetActive(true);


        EnterAction();

        GameManager.instance.ChoseAction(1);
    }

    public void OnSellButton()
    {
        EnterAction();

        GameManager.instance.ChoseAction(1);
    }

    public void OnPlaceCardButton()
    {
        //specific UI stuff
        handDisplay.SetActive(true);

        EnterAction();

        GameManager.instance.ChoseAction(3);
    }

    public void OnRotateCardButton()
    {
        EnterAction();

        GameManager.instance.ChoseAction(4);
    }

    public void OnBetButton()
    {
        undoButton.gameObject.SetActive(true);

        GameManager.instance.ChoseAction(5);
    }

    #endregion

    public void OnConfirmButton()
    {
        ExitAction();

        //gameplay stuff
        GameManager.instance.ConfirmAction();
    }

    public void OnUndoButton()
    {
        ExitAction();

        //gameplay stuff
        GameManager.instance.UndoAction();
    }

    #region UI refresh event stuff

    public void SubscribeToPlayerUIRefreshEvent(PlayerController player)
    {
        //action buttons
        player.UIrefresh += ToggleBetButton;
        player.UIrefresh += TogglePlaceCardButton;
        player.UIrefresh += ToggleRotateCardButton;
        player.UIrefresh += ToggleBuyCardButton;
        player.UIrefresh += ToggleSellCardButton;
        player.UIrefresh += ToggleMoveButton;

        //other buttons
        player.UIrefresh += handDisplayScript.RefreshHandDisplay;
        player.UIrefresh += ToggleUndoMoves;
        player.UIrefresh += ToggleEndTurnButton;
        player.UIrefresh += cardShopScript.ToggleBuyButtons;

        //infos
        player.UIrefresh += RefreshAllPrintFunctions;
    }

    public void UnsubscribeToPlayerUIRefreshEvent(PlayerController player)
    {
        //action buttons
        player.UIrefresh -= ToggleBetButton;
        player.UIrefresh -= TogglePlaceCardButton;
        player.UIrefresh -= ToggleRotateCardButton;
        player.UIrefresh -= ToggleBuyCardButton;
        player.UIrefresh -= ToggleSellCardButton;
        player.UIrefresh -= ToggleMoveButton;

        //other buttons
        player.UIrefresh -= handDisplayScript.RefreshHandDisplay;
        player.UIrefresh -= cardShopScript.ToggleBuyButtons;
        player.UIrefresh -= ToggleUndoMoves;
        player.UIrefresh -= ToggleEndTurnButton;

        //infos
        player.UIrefresh -= RefreshAllPrintFunctions;
    }

    #endregion

    #region News UI stuff

    public void RefreshAllPrintFunctions(PlayerController player)
    {
        string msg = "It's the " + player.type.ToString() + " player's turn.";
        PrintTopLeft(msg);

        if (player.currentAction == PlayerController.Action.start)
        {
            msg = "Chose an action to make!\n" + player.actions + " actions remaining.";
            PrintLeft(msg);
        }
        else if (player.currentAction == PlayerController.Action.moving)
        {
            msg = "You have " + player.possibleMoves + " moves remaining!";
            PrintLeft(msg);
        }
        else if (player.currentAction == PlayerController.Action.placeCard)
        {
            if (!player.hasPlacedCard && player.selectedCard && player.selectedCard.state == CardController.State.selectedFromHand)
            {
                msg = "Use A/D to rotate the card. \nLeftclick to place it. \nRightclick to undo.";
            }
            else
                if (!player.hasPlacedCard && player.selectedCard && player.selectedCard.state == CardController.State.selectedFromMap)
            {
                msg = "Use A/D to rotate the card.\nLeftclick to place it.\nSpace to return it to it's owner's hand.\nRightclick to undo.";
            }
            else
                if (!player.hasPlacedCard && !player.selectedCard)
            {
                msg = "Select a card from your hand.";
            }
            else
            {
                msg = "Confirm or Undo your placement!";
            }

            PrintLeft(msg);
        }
        else if (player.currentAction == PlayerController.Action.bet)
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

    #endregion

    void EnterAction()
    {
        SetTrueConfirmUndo();
    }

    public void ExitAction()
    {
        SetFalseAllSpecificActionUI();
    }

    #region Action UI stuff

    void SetFalseAllSpecificActionUI()
    {
        undoMovesButton.gameObject.SetActive(false);
        cardShop.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        undoButton.gameObject.SetActive(false);
        handDisplay.SetActive(false);
    }

    void SetTrueConfirmUndo()
    {
        confirmButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);
    }

    #endregion

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
