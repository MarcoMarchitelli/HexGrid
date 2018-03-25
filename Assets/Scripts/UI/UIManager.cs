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
    public GameObject winOverlay;
    public GameObject[] cardButtons;
    public Button betButton, undoMovesButton, endTurnButton;
    public RectTransform modifiersSection;

    #endregion

    GameManager gameManager;

    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    //public void Update()
    //{
    //    DisplayHand(gameManager.currentActivePlayer);
    //}

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
        PlayerController[] players = gameManager.players;

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

    #region Button Toggle Functions

    public void ToggleBet(PlayerController player)
    {
        player.playersToRob = gameManager.FindPlayersInRange(2, player);

        if (player.playersToRob.Count == 0 || player.hasBet)
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

    public void ToggleUndoMoves(PlayerController player)
    {
        if (player.possibleMoves == player.turnStartMoves || player.hasUsedAbility || player.currentState == PlayerController.State.bet || player.currentState == PlayerController.State.card)
        {
            undoMovesButton.enabled = false;
            undoMovesButton.image.color = Color.red;
        }
        else
        if (player.possibleMoves != player.turnStartMoves)
        {
            undoMovesButton.enabled = true;
            undoMovesButton.image.color = Color.green;
        }
    }

    public void DisplayHand(PlayerController activePlayer)
    {
        for (int i = 0; i < activePlayer.cards.Length; i++)
        {
            if (activePlayer.cards[i].GetComponent<CardController>().state != CardController.State.inHand)
            {
                cardButtons[i].GetComponent<Image>().color = Color.red;
                cardButtons[i].GetComponent<Button>().enabled = false;
            }
            else
            {
                cardButtons[i].GetComponent<Image>().color = Color.green;
                cardButtons[i].GetComponent<Button>().enabled = true;
            }
        }

        if (activePlayer.selectedCard || activePlayer.hasUsedAbility)
        {
            for (int i = 0; i < cardButtons.Length; i++)
            {
                cardButtons[i].GetComponent<Button>().enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (activePlayer.cards[i].GetComponent<CardController>().state == CardController.State.inHand)
                {
                    cardButtons[i].GetComponent<Button>().enabled = true;
                }
            }
        }
    }

    public void ToggleEndTurnButton(PlayerController player)
    {
        if (player.currentState == PlayerController.State.bet || player.selectedCard != null && !player.hasUsedAbility)
        {
            endTurnButton.enabled = false;
        }
        else
        {
            endTurnButton.enabled = true;
        }
    }

    #endregion

    public void SubscribeToPlayerUIRefreshEvent(PlayerController player)
    {
        player.UIrefresh += DisplayHand;
        player.UIrefresh += ToggleBet;
        player.UIrefresh += ToggleUndoMoves;
        player.UIrefresh += ToggleEndTurnButton;
        player.UIrefresh += RefreshAllPrintFunctions;
    }

    public void UnsubscribeToPlayerUIRefreshEvent(PlayerController player)
    {
        player.UIrefresh -= DisplayHand;
        player.UIrefresh -= ToggleBet;
        player.UIrefresh -= ToggleUndoMoves;
        player.UIrefresh -= ToggleEndTurnButton;
        player.UIrefresh -= RefreshAllPrintFunctions;
    }

    public void RefreshAllPrintFunctions(PlayerController player)
    {
        string msg = "It's the " + player.type.ToString() + " player's turn.";
        PrintTopLeft(msg);

        if (player.currentState == PlayerController.State.moving)
        {
            msg = "You have " + player.possibleMoves + " moves remaining!";
            if (player.canBet)
            {
                msg += "\nAnd you can bet!";
            }
            PrintLeft(msg);
        }
        else if (player.currentState == PlayerController.State.card)
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
        else if(player.currentState == PlayerController.State.bet)
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

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
