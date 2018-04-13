using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    public UIManager uiManager;

    [Header("Cards Prefabs")]
    public Transform prefabCard1;
    public Transform prefabCard2;
    public Transform prefabCard3;

    [HideInInspector]
    public HexGridCreator gridReference;
    [HideInInspector]
    public EFM efm;
    [HideInInspector]
    public PlayerController currentActivePlayer;
    [HideInInspector]
    public CameraBehaviour mainCamera;

    int energyBet, turnCount = 1;
    bool hasBet, winnerAnnounced;
    string bottomLeftMsg;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
        gridReference = FindObjectOfType<HexGridCreator>();
        mainCamera = FindObjectOfType<CameraBehaviour>();
        efm = FindObjectOfType<EFM>();
        InstantiatePlayers();
        players[0].currentAction = PlayerController.Action.start;
        currentActivePlayer = players[0];
        string msg = "It's the " + efm.currentPhase.ToString() + " phase.";
        uiManager.PrintTopRight(msg);
    }

    private void Start()
    {
        mainCamera.SetTransform(currentActivePlayer);
        efm.SetPhase(efm.currentPhase, players);
        uiManager.PrintPlayersModifiers();
        uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);
        if (currentActivePlayer.UIrefresh != null)
        {
            currentActivePlayer.UIrefresh(currentActivePlayer);
        }
    }

    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentActivePlayer && players[i].currentAction == PlayerController.Action.idle)
            {
                if (i != players.Length - 1)
                {
                    players[i + 1].currentAction = PlayerController.Action.start;
                    currentActivePlayer = players[i + 1];
                    uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);
                    if (currentActivePlayer.UIrefresh != null)
                    {
                        currentActivePlayer.UIrefresh(currentActivePlayer);
                    }
                    mainCamera.SetTransform(currentActivePlayer);
                    turnCount++;
                    efm.AutoChangePhase(turnCount);
                    uiManager.PrintPlayersModifiers();
                    string msg = "It's the " + efm.currentPhase.ToString() + " phase.";
                    uiManager.PrintTopRight(msg);
                }
                else
                {
                    players[0].currentAction = PlayerController.Action.start;
                    currentActivePlayer = players[0];
                    uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);
                    if (currentActivePlayer.UIrefresh != null)
                    {
                        currentActivePlayer.UIrefresh(currentActivePlayer);
                    }
                    mainCamera.SetTransform(currentActivePlayer);
                    turnCount++;
                    efm.AutoChangePhase(turnCount);
                    uiManager.PrintPlayersModifiers();
                    string msg = "It's the " + efm.currentPhase.ToString() + " phase.";
                    uiManager.PrintTopRight(msg);
                }
            }
        }
    }

    void InstantiatePlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            switch (players[i].type)
            {
                case PlayerController.Type.underground:
                    Point redStart = gridReference.GetPointFromCoords((int)MyData.startingRedPoint.x, (int)MyData.startingRedPoint.y);
                    players[i].transform.position = redStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = redStart;
                    break;
                case PlayerController.Type.hypogeum:
                    Point yellowStart = gridReference.GetPointFromCoords((int)MyData.startingYellowPoint.x, (int)MyData.startingYellowPoint.y);
                    players[i].transform.position = yellowStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = yellowStart;
                    break;
                case PlayerController.Type.underwater:
                    Point blueStart = gridReference.GetPointFromCoords((int)MyData.startingBluePoint.x, (int)MyData.startingBluePoint.y);
                    players[i].transform.position = blueStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = blueStart;
                    break;
                case PlayerController.Type.forest:
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

        if (player.type == PlayerController.Type.hypogeum && player.currentWayPoint.type == Point.Type.hypogeum)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].type != PlayerController.Type.hypogeum && players[i].currentWayPoint.type == Point.Type.hypogeum)
                {
                    moves = 3;
                }
            }
        }
        else if (player.type == PlayerController.Type.hypogeum && player.currentWayPoint.type != Point.Type.hypogeum)
        {
            moves = 3;
        }

        if (player.type == PlayerController.Type.forest && player.currentWayPoint.type == Point.Type.forest)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].type != PlayerController.Type.forest && players[i].currentWayPoint.type == Point.Type.forest)
                {
                    moves = 3;
                }
            }
        }
        else if (player.type == PlayerController.Type.forest && player.currentWayPoint.type != Point.Type.forest)
        {
            moves = 3;
        }

        if (player.type == PlayerController.Type.underwater && player.currentWayPoint.type == Point.Type.underwater)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].type != PlayerController.Type.underwater && players[i].currentWayPoint.type == Point.Type.underwater)
                {
                    moves = 3;
                }
            }
        }
        else if (player.type == PlayerController.Type.underwater && player.currentWayPoint.type != Point.Type.underwater)
        {
            moves = 3;
        }

        if (player.type == PlayerController.Type.underground && player.currentWayPoint.type == Point.Type.underground)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].type != PlayerController.Type.underground && players[i].currentWayPoint.type == Point.Type.underground)
                {
                    moves = 3;
                }
            }
        }
        else if (player.type == PlayerController.Type.underground && player.currentWayPoint.type != Point.Type.underground)
        {
            moves = 3;
        }

        return moves;
    }

    #region Button Functions

    //called when clickin card buttons
    public void CurrentPlayerSelectCard(int index)
    {
        currentActivePlayer.SelectCard(index);
        if (currentActivePlayer.UIrefresh != null)
        {
            currentActivePlayer.UIrefresh(currentActivePlayer);
        }
    }

    //called when clicking endturn button
    public void SetCurrentPlayerIdle()
    {
        uiManager.UnsubscribeToPlayerUIRefreshEvent(currentActivePlayer);
        currentActivePlayer.currentAction = PlayerController.Action.idle;
        if (currentActivePlayer.UIrefresh != null)
        {
            currentActivePlayer.UIrefresh(currentActivePlayer);
        }
    }

    //called when clicking undo movement button
    public void UndoMoveCurrentPlayer()
    {
        currentActivePlayer.possibleMoves = currentActivePlayer.beforeMoveActionMoves;
        currentActivePlayer.MoveToPoint(currentActivePlayer.moveStartPoint);
        currentActivePlayer.currentWayPoint = currentActivePlayer.moveStartPoint;
        currentActivePlayer.currentAction = PlayerController.Action.moving;
        if (currentActivePlayer.UIrefresh != null)
        {
            currentActivePlayer.UIrefresh(currentActivePlayer);
        }
    }

    public void ChoseAction(int actionIndex)
    {
        currentActivePlayer.ChoseAction(actionIndex);
    }

    public void ConfirmAction()
    {
        switch (currentActivePlayer.currentAction)
        {
            case PlayerController.Action.moving:
                break;
            case PlayerController.Action.buyCard:
                ConfirmBoughtCards();
                break;
            case PlayerController.Action.sellCard:
                break;
            case PlayerController.Action.placeCard:
                break;
            case PlayerController.Action.rotateCard:
                break;
            case PlayerController.Action.bet:
                break;
        }

        currentActivePlayer.currentAction = PlayerController.Action.start;
        currentActivePlayer.actions--;

        if (currentActivePlayer.UIrefresh != null)
            currentActivePlayer.UIrefresh(GameManager.instance.currentActivePlayer);
    }

    public void UndoAction()
    {
        switch (currentActivePlayer.currentAction)
        {
            case PlayerController.Action.idle:
                break;
            case PlayerController.Action.start:
                break;
            case PlayerController.Action.moving:
                UndoMoveCurrentPlayer();
                break;
            case PlayerController.Action.buyCard:
                UndoBoughtCards();
                break;
            case PlayerController.Action.sellCard:
                break;
            case PlayerController.Action.placeCard:
                break;
            case PlayerController.Action.rotateCard:
                break;
            case PlayerController.Action.bet:
                break;
        }

        currentActivePlayer.currentAction = PlayerController.Action.start;

        if (currentActivePlayer.UIrefresh != null)
            currentActivePlayer.UIrefresh(GameManager.instance.currentActivePlayer);
    }

    #endregion

    #region Specific Confirm Actions

    void ConfirmBoughtCards()
    {
        List<int> cardsBought = uiManager.cardShopScript.cardsBought;

        foreach (int cardType in cardsBought)
        {
            switch (cardType)
            {
                case 1:
                    Transform instantiatedCard1 = Instantiate(prefabCard1, MyData.prefabsPosition, Quaternion.identity);
                    currentActivePlayer.cards.Add(instantiatedCard1);
                    break;
                case 2:
                    Transform instantiatedCard2 = Instantiate(prefabCard2, MyData.prefabsPosition, Quaternion.identity);
                    currentActivePlayer.cards.Add(instantiatedCard2);
                    break;
                case 3:
                    Transform instantiatedCard3 = Instantiate(prefabCard3, MyData.prefabsPosition, Quaternion.identity);
                    currentActivePlayer.cards.Add(instantiatedCard3);
                    break;
            }
        }

        currentActivePlayer.hasBought = false;

        uiManager.cardShopScript.cardsBought.Clear();
    }

    #endregion

    #region Specific Undo Actions

    void UndoBoughtCards()
    {
        if (currentActivePlayer.hasBought)
        {
            currentActivePlayer.energyPoints = currentActivePlayer.beforeBuyActionEnergyPoints;

            currentActivePlayer.hasBought = false;

            uiManager.cardShopScript.cardsBought.Clear();
        }
    }

    #endregion

    public void Win(PlayerController player)
    {
        uiManager.Win(player);
    }

    #region Bet related functions

    public IEnumerator Bet(PlayerController attacker, PlayerController defender)
    {
        PlayerController winner;
        int attackerBet, defenderBet;
        string announcement = null;
        bool atkWon = false, doesAttackerDoubleSteal;
        string[] messages = { "The winner is...", null};
        winnerAnnounced = false;

        #region The 2 players betting
        //attacker bet
        while (!hasBet)
        {
            uiManager.PrintBigNews("It's " + attacker.type.ToString() + "'s time to bet !");
            uiManager.PrintLeft("Enter a number to bet some energy.");

            yield return StartCoroutine(WaitForNumberInput(attacker, 0));
        }

        attackerBet = energyBet;
        hasBet = false;

        ///defender bet
        while (!hasBet)
        {
            uiManager.PrintBigNews("It's " + defender.type.ToString() + "'s time to bet !");
            uiManager.PrintLeft("Enter a number to bet some energy.");

            yield return StartCoroutine(WaitForNumberInput(defender, 1));
        }

        defenderBet = energyBet;
        hasBet = false;
        #endregion

        winner = efm.FightResult(attacker, defender, attackerBet, defenderBet, out doesAttackerDoubleSteal);

        #region Check fight result
        if(winner == null)
        {
            announcement = "Noone! It's a Draw!";
        }else 
        if(winner == attacker)
        {
            announcement = attacker.type.ToString() + "!! \nCongratulations!";
            atkWon = true;
        }else
        if(winner == defender)
        {
            announcement = defender.type.ToString() + "!! \nCongratulations!";
        }
        #endregion

        attacker.energyPoints -= attackerBet;
        defender.energyPoints -= defenderBet;

        messages[1] = announcement;

        //result show
        while(!winnerAnnounced)
            yield return StartCoroutine(WaitForWinnerAnnoucement(messages, 2));

        uiManager.PrintBigNews(null);

        //PV update
        if (atkWon)
        {
            if (!doesAttackerDoubleSteal)
            {
                if (defender.victoryPoints != 0)
                {
                    attacker.victoryPoints++;
                    defender.victoryPoints--;
                }   
            }
            else
            {
                if (defender.victoryPoints != 0)
                {
                    attacker.victoryPoints++;
                    defender.victoryPoints--;
                    attacker.victoryPoints++;
                    defender.victoryPoints--;
                }  
            }
        }

        attacker.currentAction = attacker.previousState;
        uiManager.ToggleBet(attacker);
        if (currentActivePlayer.UIrefresh != null)
        {
            currentActivePlayer.UIrefresh(currentActivePlayer);
        }
    }

    public IEnumerator WaitForNumberInput(PlayerController player, int roleIndex)
    {

        if(roleIndex == 0)
        {
            while (!hasBet)
            {
                foreach(int i in efm.atkNumbers)
                {
                    if (Input.GetKeyDown(i.ToString()) && i <= player.energyPoints)
                    {
                        hasBet = true;
                        energyBet = i;
                    }
                }
                yield return null;
            }
        }else
            if(roleIndex == 1)
        {
            while (!hasBet)
            {
                foreach (int i in efm.defNumbers)
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

    #endregion

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