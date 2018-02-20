using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    public UIManager uiManager;
    public HexGridCreator gridReference;

    PlayerController currentActivePlayer;

    string bottomLeftMsg;

    void Awake()
    {
        gridReference = FindObjectOfType<HexGridCreator>();
        InstantiatePlayers();
        players[0].currentState = PlayerController.State.start;
        currentActivePlayer = players[0];
        string msg = "It's the " + currentActivePlayer.name + " player's turn.";
        uiManager.PrintTop(msg);
    }

    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentActivePlayer && players[i].currentState == PlayerController.State.idle)
            {
                if (i != players.Length - 1)
                {
                    players[i + 1].currentState = PlayerController.State.start;
                    currentActivePlayer = players[i + 1];
                    string msg = "It's the " + currentActivePlayer.name + " player's turn.";
                    uiManager.PrintTop(msg);
                }
                else
                {
                    players[0].currentState = PlayerController.State.start;
                    currentActivePlayer = players[0];
                    string msg = "It's the " + currentActivePlayer.name + " player's turn.";
                    uiManager.PrintTop(msg);
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

    public int NumberOfPossiblesMoves(PlayerController player)
    {
        int moves = 4;

        if (player.name == "yellow" && player.currentWayPoint.type == Point.Type.yellow)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].name != "yellow" && players[i].currentWayPoint.type == Point.Type.yellow)
                {
                    moves = 3;
                }
            }
        }
        else if (player.name == "yellow" && player.currentWayPoint.type != Point.Type.yellow)
        {
            moves = 3;
        }

        if (player.name == "green" && player.currentWayPoint.type == Point.Type.green)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].name != "green" && players[i].currentWayPoint.type == Point.Type.green)
                {
                    moves = 3;
                }
            }
        }
        else if (player.name == "green" && player.currentWayPoint.type != Point.Type.green)
        {
            moves = 3;
        }

        if (player.name == "blue" && player.currentWayPoint.type == Point.Type.blue)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].name != "blue" && players[i].currentWayPoint.type == Point.Type.blue)
                {
                    moves = 3;
                }
            }
        }
        else if (player.name == "blue" && player.currentWayPoint.type != Point.Type.blue)
        {
            moves = 3;
        }

        if (player.name == "red" && player.currentWayPoint.type == Point.Type.red)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].name != "red" && players[i].currentWayPoint.type == Point.Type.red)
                {
                    moves = 3;
                }
            }
        }
        else if (player.name == "red" && player.currentWayPoint.type != Point.Type.red)
        {
            moves = 3;
        }

        return moves;
    }

    public void SetCurrentPlayerPlacing()
    {
        currentActivePlayer.currentState = PlayerController.State.ability;
    }

    public void SetCurrentPlayerIdle()
    {
        currentActivePlayer.currentState = PlayerController.State.idle;
    }

    public List<AgentPosition> FindPointsInRange(int range, PlayerController player) {
        List<AgentPosition> pointsInRange = new List<AgentPosition>();
        bool doesUncheckExist = true;

        if (range > 0) {
            pointsInRange.Add(new AgentPosition(player.currentWayPoint, range));

        }
        do {
            
            for (int i = 0; i < pointsInRange.Count; i++) {
                if (!pointsInRange[i].isChecked) {
                    if(pointsInRange[i].moves > 0) {
                        foreach (Point point in pointsInRange[i].point.possibleDestinations) {
                            pointsInRange.Add(new AgentPosition(point, pointsInRange[i].moves--));
                        }
                    }
                    pointsInRange[i].isChecked = true;
                    doesUncheckExist = false;
                }
            }
        } while (doesUncheckExist);

        return pointsInRange;
    }

    public class AgentPosition {
        public Point point;
        public Vector3 worldPosition;
        public int moves;
        public bool isChecked;

        public AgentPosition(Point _point, int _moves) {
            point = _point;
            worldPosition = _point.worldPosition;
            moves = _moves;
            isChecked = false;
        }
    }
}