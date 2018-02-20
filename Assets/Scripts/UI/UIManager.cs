﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {

    public TextMeshProUGUI topBigSection, leftMediumSection;

    public GameObject[] Buttons;

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
                Buttons[i].GetComponent<Button>().enabled = false;
            }
            else
            {
                Buttons[i].GetComponent<Button>().enabled = true;
            }
        }
    }
}
