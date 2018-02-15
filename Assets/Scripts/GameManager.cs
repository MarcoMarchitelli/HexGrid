using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

    public PlayerController[] players;
    public HexGridCreator gridReference;

    public TextMeshProUGUI currentTurnVisualizer;

    PlayerController currentActivePlayer;

	void Awake () {
        gridReference = FindObjectOfType<HexGridCreator>();
        InstantiatePlayers();
        players[0].currentState = PlayerController.State.active;
        currentActivePlayer = players[0];
    }

    void Update()
    {
        string msg = "It's the " + currentActivePlayer.name + " player's turn.";
        currentTurnVisualizer.text = msg;

        for (int i = 0; i < players.Length; i++)
        {
            if(players[i] == currentActivePlayer && players[i].currentState == PlayerController.State.idle)
            {
                if(i != players.Length - 1)
                {
                    players[i + 1].currentState = PlayerController.State.active;
                    currentActivePlayer = players[i + 1];
                }
                else
                {
                    players[0].currentState = PlayerController.State.active;
                    currentActivePlayer = players[0];
                }
            }
        }
    }

    void InstantiatePlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            switch (players[i].name)
            {
                case "red":
                    Point redStart = gridReference.GetPointFromCoords((int)MyData.startingRedPoint.x, (int)MyData.startingRedPoint.y);
                    players[i].transform.position = redStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = redStart;
                    break;
                case "yellow":
                    Point yellowStart = gridReference.GetPointFromCoords((int)MyData.startingYellowPoint.x, (int)MyData.startingYellowPoint.y);
                    players[i].transform.position = yellowStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = yellowStart;
                    break;
                case "blue":
                    Point blueStart = gridReference.GetPointFromCoords((int)MyData.startingBluePoint.x, (int)MyData.startingBluePoint.y);
                    players[i].transform.position = blueStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = blueStart;
                    break;
                case "green":
                    Point greenStart = gridReference.GetPointFromCoords((int)MyData.startingGreenPoint.x, (int)MyData.startingGreenPoint.y);
                    players[i].transform.position = greenStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = greenStart;
                    break;
            }
        }
    }
}
