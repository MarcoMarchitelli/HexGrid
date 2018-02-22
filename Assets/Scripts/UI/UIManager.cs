using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {

    public TextMeshProUGUI topBigSection, leftMediumSection;

    public GameObject[] Buttons;

    GameManager gameManager;

    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Update()
    {
        DisplayHand(gameManager.currentActivePlayer);
    }

    public void PrintTop(string msg)
    {
        topBigSection.text = msg;
    }

    public void PrintLeft(string msg)
    {
        leftMediumSection.text = msg;
    }

    public void DisplayHand(PlayerController activePlayer)
    {
        for (int i = 0; i < activePlayer.cards.Length; i++)
        {
            if(activePlayer.cards[i].GetComponent<CardController>().state != CardController.State.inHand)
            {
                Buttons[i].GetComponent<Image>().color = Color.red;
                Buttons[i].GetComponent<Button>().enabled = false;
            }
            else
            {
                Buttons[i].GetComponent<Image>().color = Color.green;
                Buttons[i].GetComponent<Button>().enabled = true;
            }
        }
    }
}
