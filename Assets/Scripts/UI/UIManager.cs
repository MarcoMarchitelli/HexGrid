using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Public Variables

    public TextMeshProUGUI topBigSection, leftMediumSection, bigCentralSection;
    public GameObject winOverlay;
    public GameObject[] cardButtons;
    public Button betButton, undoMovesButton;

    #endregion

    GameManager gameManager;

    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Update()
    {
        DisplayHand(gameManager.currentActivePlayer);
    }

    #region Print Functions

    public void PrintTop(string msg)
    {
        topBigSection.text = msg;
    }

    public void PrintLeft(string msg)
    {
        leftMediumSection.text = msg;
    }

    public void PrintBigNews(string msg)
    {
        bigCentralSection.text = msg;
    }

    #endregion

    #region Button Toggle Functions

    public void ToggleBet(PlayerController player)
    {
        player.playersToRob = gameManager.FindPlayersInRange(2, gameManager.currentActivePlayer);

        if (player.playersToRob.Count == 0)
        {
            betButton.enabled = false;
            betButton.image.color = Color.red;
        }
        else if (player.playersToRob.Count > 0)
        {
            betButton.enabled = true;
            betButton.image.color = Color.green;
        }

    }

    public void ToggleUndoMoves(PlayerController player)
    {
        if (player.possibleMoves != player.turnStartMoves && player.currentState == PlayerController.State.moving)
        {
            undoMovesButton.enabled = true;
            undoMovesButton.image.color = Color.green;
        }
        else
            if (player.possibleMoves == player.turnStartMoves || player.hasUsedAbility || player.currentState == PlayerController.State.bet || player.currentState == PlayerController.State.card)
        {
            undoMovesButton.enabled = false;
            undoMovesButton.image.color = Color.red;
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

    #endregion

    public void Win(PlayerController player)
    {
        winOverlay.SetActive(true);
        winOverlay.GetComponentInChildren<TextMeshProUGUI>().text = player.name + " Wins !";
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
