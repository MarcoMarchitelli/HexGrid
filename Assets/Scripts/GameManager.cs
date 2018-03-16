using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    public UIManager uiManager;
    public HexGridCreator gridReference;

    [HideInInspector]
    public PlayerController currentActivePlayer;

    [HideInInspector]
    public CameraBehaviour mainCamera;

    int energyBet;
    bool hasBet, winnerAnnounced;

    string bottomLeftMsg;

    void Awake()
    {
        gridReference = FindObjectOfType<HexGridCreator>();
        mainCamera = FindObjectOfType<CameraBehaviour>();
        InstantiatePlayers();
        players[0].currentState = PlayerController.State.start;
        currentActivePlayer = players[0];
        mainCamera.SetTransform(currentActivePlayer);
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
                    mainCamera.SetTransform(currentActivePlayer);
                    string msg = "It's the " + currentActivePlayer.name + " player's turn.";
                    uiManager.PrintTop(msg);
                }
                else
                {
                    players[0].currentState = PlayerController.State.start;
                    currentActivePlayer = players[0];
                    mainCamera.SetTransform(currentActivePlayer);
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
                    players[i].transform.parent.position = redStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = redStart;
                    break;
                case "yellow":
                    Point yellowStart = gridReference.GetPointFromCoords((int)MyData.startingYellowPoint.x, (int)MyData.startingYellowPoint.y);
                    players[i].transform.parent.position = yellowStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = yellowStart;
                    break;
                case "blue":
                    Point blueStart = gridReference.GetPointFromCoords((int)MyData.startingBluePoint.x, (int)MyData.startingBluePoint.y);
                    players[i].transform.parent.position = blueStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = blueStart;
                    break;
                case "green":
                    Point greenStart = gridReference.GetPointFromCoords((int)MyData.startingGreenPoint.x, (int)MyData.startingGreenPoint.y);
                    players[i].transform.parent.position = greenStart.worldPosition + Vector3.up * .5f;
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

    //called when clickin card buttons
    public void CurrentPlayerSelect(int index)
    {
        currentActivePlayer.SelectCard(index);
    }

    //called when clicking endturn button
    public void SetCurrentPlayerIdle()
    {
        currentActivePlayer.currentState = PlayerController.State.idle;
    }

    //called when clicking bet button
    public void SetCurrenPlayerBet()
    {
        currentActivePlayer.previousState = currentActivePlayer.currentState;
        currentActivePlayer.currentState = PlayerController.State.bet;
    }

    //called when clicking undo movement button
    public void UndoMoveCurrentPlayer()
    {
        currentActivePlayer.possibleMoves = currentActivePlayer.turnStartMoves;
        currentActivePlayer.MoveToPoint(currentActivePlayer.turnStartPoint);
        currentActivePlayer.currentWayPoint = currentActivePlayer.turnStartPoint;
        currentActivePlayer.currentState = PlayerController.State.moving;
        uiManager.ToggleUndoMoves(currentActivePlayer);
    }

    public void Win(PlayerController player)
    {
        uiManager.Win(player);
    }

    public IEnumerator Bet(PlayerController attacker, PlayerController defender)
    {
        int attackerBet, defenderBet;
        string announcement;
        bool atkWon = false;
        string[] messages = { "The winner is...", null};

        while(!hasBet)
        {
            uiManager.PrintBigNews("It's " + attacker.name + "'s time to bet !");
            uiManager.PrintLeft("Enter a number to bet some energy.");

            yield return StartCoroutine(WaitForNumberInput(attacker));
        }

        attackerBet = energyBet;
        hasBet = false;

        while (!hasBet)
        {
            uiManager.PrintBigNews("It's " + defender.name + "'s time to bet !");
            uiManager.PrintLeft("Enter a number to bet some energy.");

            yield return StartCoroutine(WaitForNumberInput(defender));
        }

        defenderBet = energyBet;
        hasBet = false;

        if (attackerBet > defenderBet)
        {
            announcement = attacker.name + "!! \nCongratulations!";
            attacker.energyPoints -= attackerBet;
            defender.energyPoints -= defenderBet;
            atkWon = true;
        }
        else
        if (attackerBet < defenderBet)
        {
            announcement = defender.name + "!! \nCongratulations!";
            attacker.energyPoints -= attackerBet;
            defender.energyPoints -= defenderBet;
        }   
        else
            announcement = "Noone! It's a Draw!";

        messages[1] = announcement;

        while(!winnerAnnounced)
            yield return StartCoroutine(WaitForWinnerAnnoucement(messages, 3));

        uiManager.PrintBigNews(null);
        if (atkWon)
        {
            attacker.victoryPoints++;
            defender.victoryPoints--;
        }

        winnerAnnounced = false;
        attacker.currentState = attacker.previousState;
        attacker.hasBet = true;
        uiManager.ToggleBet(attacker);
    }

    public IEnumerator WaitForNumberInput(PlayerController player)
    {
        
        while (!hasBet)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(i.ToString()) && i <= player.energyPoints)
                {
                    hasBet = true;
                    energyBet = i;
                }
            }
            yield return null;
        }
    }

    public IEnumerator WaitForWinnerAnnoucement(string[] messages, float seconds)
    {
        foreach (string message in messages)
        {
            uiManager.PrintBigNews(message);
            yield return new WaitForSeconds(seconds);
        }
        winnerAnnounced = true;
    }

    public List<AgentPosition> FindPointsInRange(int range, PlayerController player)
    {
        List<AgentPosition> pointsInRange = new List<AgentPosition>();

        if (range > 0)
        {
            pointsInRange.Add(new AgentPosition(player.currentWayPoint, range));
        }

        for (int i = 0; i < pointsInRange.Count; i++)
        {
            if (!pointsInRange[i].isChecked)
            {
                if (pointsInRange[i].moves > 0)
                {
                    foreach (Vector3 destination in pointsInRange[i].point.possibleDestinations)
                    {
                        pointsInRange.Add(new AgentPosition(gridReference.GetPointFromWorldPosition(destination), pointsInRange[i].moves - 1));
                    }
                }
                pointsInRange[i].isChecked = true;
            }
        }

        return pointsInRange;
    }

    public List<PlayerController> FindPlayersInRange(int range, PlayerController player)
    {
        List<PlayerController> playersInRange = new List<PlayerController>();

        List<AgentPosition> pointsInRange = FindPointsInRange(range, player);

        foreach (AgentPosition agent in pointsInRange)
        {
            foreach (PlayerController _player in players)
            {
                if (agent.point == _player.currentWayPoint && player != _player)
                {
                    playersInRange.Add(_player);
                }
            }
        }

        return playersInRange;
    }
}

public class AgentPosition

{
    public Point point;
    //mosse rimaste
    public int moves;
    public bool isChecked;

    public AgentPosition(Point _point, int _moves)
    {
        point = _point;
        moves = _moves;
        isChecked = false;
    }
}