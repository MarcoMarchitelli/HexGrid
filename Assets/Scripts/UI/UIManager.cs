using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Public Variables
    
    Color activeColor = Color.green;
    Color inactiveColor = Color.red;
    Color specialColor = Color.yellow;

    [Header("Print Sections")]
    public TextMeshProUGUI topLeftSection;
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
    public Button bonusMoveButton;
    [Header("Other Buttons")]
    public Button undoMovesButton;
    public Button endTurnButton;
    public Button confirmButton;
    public Button undoButton;
    [Header("Other UI Elements")]
    public GameObject winOverlay;
    public GameObject cardShop;
    public GameObject handDisplay;
    public GameObject sellCardDisplay;
    public GameObject BonusMoveCounter;

    [HideInInspector]
    public CardShop cardShopScript;
    [HideInInspector]
    public HandDisplay handDisplayScript;
    [HideInInspector]
    public SellCardDisplay sellCardDisplayScript;

    #endregion

    [HideInInspector]
    public Animation bigNewsAnimation;

    void Start()
    {
        cardShopScript = cardShop.GetComponent<CardShop>();
        handDisplayScript = handDisplay.GetComponent<HandDisplay>();
        bigNewsAnimation =  bigCentralSection.GetComponentInParent<Animation>();
        sellCardDisplayScript = sellCardDisplay.GetComponent<SellCardDisplay>();
    }

    #region Print Functions

    public void PrintTopLeft(string msg)
    {
        topLeftSection.text = msg;
    }

    public void PrintLeft(string msg)
    {
        leftMediumSection.text = msg;
    }

    public void PrintBigNews(string msg)
    {
        bigCentralSection.text = msg;
        bigNewsAnimation.Play();
    }

    #endregion

    #region Action Buttons Toggle

    public void ToggleBonusMoveButton(PlayerController player)
    {
        //set color
        if (player.bonusMoveActions <= 0)
        {
            bonusMoveButton.image.color = inactiveColor;
        }
        else
        {
            bonusMoveButton.image.color = specialColor;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && bonusMoveButton.image.color == specialColor)
        {
            bonusMoveButton.enabled = true;
        }
        else
        {
            bonusMoveButton.enabled = false;
        }
    }

    public void ToggleMoveButton(PlayerController player)
    {
        //set color
        if (player.actions <= 0)
        {
            moveButton.image.color = inactiveColor;
        }
        else
        {
            moveButton.image.color = activeColor;
        }

        //set usability
        if(player.currentAction == PlayerController.Action.start && moveButton.image.color == activeColor)
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
            betButton.image.color = inactiveColor;
            player.canBet = false;
        }
        else if (player.playersToRob.Count > 0 && player.energyPoints >= 1)
        {
            betButton.image.color = activeColor;
            player.canBet = true;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && betButton.image.color == activeColor)
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
            placeCardButton.image.color = inactiveColor;
        }
        else
        {
            placeCardButton.image.color = activeColor;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && placeCardButton.image.color == activeColor)
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
            rotateCardButton.image.color = inactiveColor;
        }
        else
        {
            rotateCardButton.image.color = activeColor;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && rotateCardButton.image.color == activeColor)
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
        if (player.actions <= 0 || player.energyPoints <= 0 && !player.hasDiscount)
        {
            buyCardButton.image.color = inactiveColor;
        }
        else
        {
            if (player.hasDiscount)
                buyCardButton.image.color = specialColor;
            else
                buyCardButton.image.color = activeColor;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && buyCardButton.image.color == activeColor || buyCardButton.image.color == specialColor)
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
            sellCardButton.image.color = inactiveColor;
        }
        else
        {
            sellCardButton.image.color = activeColor;
        }

        //set usability
        if (player.currentAction == PlayerController.Action.start && sellCardButton.image.color == activeColor)
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
            if (player.possibleMoves == player.beforeMoveActionMoves || player.isRunning)
            {
                undoMovesButton.enabled = false;
                undoMovesButton.image.color = inactiveColor;
            }
            else
            {
                undoMovesButton.enabled = true;
                undoMovesButton.image.color = activeColor;
            }
        }
    }

    public void ToggleEndTurnButton(PlayerController player)
    {
        if (GameManager.instance.currentPhase != GameManager.Phase.main)
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

    public void OnBonusMoveButton()
    {
        //specific UI to activate
        undoMovesButton.gameObject.SetActive(true);

        EnterAction();

        GameManager.instance.ChoseAction(0);
        GameManager.instance.currentActivePlayer.isBonusMove = true;
    }

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
        //specific UI stuff
        sellCardDisplay.SetActive(true);

        EnterAction();

        GameManager.instance.ChoseAction(2);
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

    //public void OnConfirmButton()
    //{
    //    ExitAction();
    //    GameManager.instance.mainCamera.SetHighView(false);

    //    //gameplay stuff
    //    GameManager.instance.ConfirmAction();
    //}

    //public void OnUndoButton()
    //{
    //    ExitAction();
    //    GameManager.instance.mainCamera.SetHighView(false);

    //    //gameplay stuff
    //    GameManager.instance.UndoAction();
    //}

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
        player.UIrefresh += ToggleBonusMoveButton;

        //other buttons
        player.UIrefresh += handDisplayScript.RefreshHandDisplay;
        player.UIrefresh += ToggleUndoMoves;
        player.UIrefresh += ToggleEndTurnButton;
        player.UIrefresh += cardShopScript.ToggleBuyButtons;
        player.UIrefresh += sellCardDisplayScript.RefreshSellDisplay;

        //infos
        player.UIrefresh += RefreshAllPrintFunctions;
        player.UIrefresh += RefreshBonusMoveCounter;
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
        player.UIrefresh -= ToggleBonusMoveButton;

        //other buttons
        player.UIrefresh -= handDisplayScript.RefreshHandDisplay;
        player.UIrefresh -= cardShopScript.ToggleBuyButtons;
        player.UIrefresh -= ToggleUndoMoves;
        player.UIrefresh -= ToggleEndTurnButton;
        player.UIrefresh -= sellCardDisplayScript.RefreshSellDisplay;

        //infos
        player.UIrefresh -= RefreshAllPrintFunctions;
        player.UIrefresh -= RefreshBonusMoveCounter;
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
        else if (player.currentAction == PlayerController.Action.fight)
        {
            PrintLeft("Select a player to attack.\nRightclick to undo.");
        }

    }

    public void Win(PlayerController player)
    {
        winOverlay.SetActive(true);
        winOverlay.GetComponentInChildren<TextMeshProUGUI>().text = player.type.ToString() + " Wins !";
    }

    public void RefreshBonusMoveCounter(PlayerController player)
    {
        if(player.bonusMoveActions <= 1)
        {
            BonusMoveCounter.SetActive(false);
        }
        else
        {
            BonusMoveCounter.SetActive(true);
            TextMeshProUGUI counter =  BonusMoveCounter.GetComponentInChildren<TextMeshProUGUI>();
            counter.text = player.bonusMoveActions.ToString();
        }
    }

    #endregion

    #region Action UI stuff

    public void ExitAction()
    {
        undoMovesButton.gameObject.SetActive(false);
        cardShop.SetActive(false);
        sellCardDisplay.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        undoButton.gameObject.SetActive(false);
        handDisplay.SetActive(false);
    }

    public void EnterAction()
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
