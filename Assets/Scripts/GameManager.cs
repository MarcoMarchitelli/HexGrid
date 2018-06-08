using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

public class GameManager : MonoBehaviour
{
    public delegate void AgentPositionListEvent(List<AgentPosition> points);
    public delegate void VoidEvent();
    public AgentPositionListEvent OnMoveEnter;
    public VoidEvent OnMoveSelected;

    public PlayerController[] players;
    //public UIManager uiManager;

    [Header("Cards Prefabs")]
    public Transform prefabCard1;
    public Transform prefabCard2;
    public Transform prefabCard3;

    [HideInInspector]
    public HexGridCreator gridReference;
    [HideInInspector]
    public PlayerController currentActivePlayer;
    [HideInInspector]
    public CameraBehaviour mainCamera;
    [HideInInspector]
    public CardsManager cardsManager;
    [HideInInspector]
    public PlayersHUDController playersHUDcontroller;
    [HideInInspector]
    public HUDManager hudManager;
    [HideInInspector]
    public CombatManager combatManager;
    [HideInInspector]
    public Pathfinding pathfinding;
    [HideInInspector]
    public int turnCount = 0;
    [HideInInspector]
    public float buttonMashFightResult = .5f;

    [HideInInspector]
    public bool isStaticEvent = false;

    public static GameManager instance;
    [HideInInspector]
    public bool rotationPhaseEnded = false, gainPhaseEnded = false, discountChecked = false, turnEnded = false;
    [HideInInspector]
    public int playerIndex = 0;

    string message = null;

    public enum Phase
    {
        start, gain, main, rotation
    }

    public Phase currentPhase;

    public void Init()
    {
        instance = this;
        gridReference = FindObjectOfType<HexGridCreator>();
        mainCamera = FindObjectOfType<CameraBehaviour>();
        cardsManager = GetComponent<CardsManager>();
        playersHUDcontroller = FindObjectOfType<PlayersHUDController>();
        hudManager = FindObjectOfType<HUDManager>();
        combatManager = GetComponent<CombatManager>();
        pathfinding = GetComponent<Pathfinding>();
        InstantiatePlayers();
        StartCoroutine(TurnStart(playerIndex));
    }

    IEnumerator TurnStart(int playerIndex)
    {
        currentPhase = Phase.start;

        #region reset values
        turnEnded = false;
        rotationPhaseEnded = false;
        gainPhaseEnded = false;
        #endregion

        currentActivePlayer = players[playerIndex];

        //uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);

        hudManager.Refresh();

        print(currentActivePlayer.name + "'s Turn Start!");
        message = "Turn Start";
        hudManager.PrintBigNews(message);

        while (hudManager.bigNewsAnimation.isPlaying)
            yield return null;

        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        yield return StartCoroutine(GainPhase());

        //if(CheckDiscount())
        //DO STUFF


        yield return StartCoroutine(PlayPhase());

        yield return StartCoroutine(RotationPhase());

        StartCoroutine(TurnStart(playerIndex));
    }

    IEnumerator GainPhase()
    {
        currentPhase = Phase.gain;
        print("Gain Phase!"); 

        message = "Gain Phase";
        hudManager.PrintBigNews(message);

        while (hudManager.bigNewsAnimation.isPlaying)
            yield return null;

        currentActivePlayer.ResetValues();

        while (!gainPhaseEnded)
        {
            cardsManager.GainPhase(currentActivePlayer);
            playersHUDcontroller.RefreshPlayerUIs();
            //VFX E ALTRE COSE
            yield return null;
        }

        hudManager.Refresh();
    }

    IEnumerator PlayPhase()
    {
        currentPhase = Phase.main;       
        currentActivePlayer.TurnStart();

        print("Chose an action " + currentActivePlayer + "!");

        message = "Main Phase";
        hudManager.PrintBigNews(message);

        while (hudManager.bigNewsAnimation.isPlaying)
            yield return null;

        hudManager.Refresh();

        while (!turnEnded)
        {
            //PRESS END TURN BUTTON
            yield return null;
        }

        currentActivePlayer.currentAction = PlayerController.Action.idle;

    }

    IEnumerator RotationPhase()
    {
        currentPhase = Phase.rotation;
        print("Rotation Phase!");


        hudManager.Refresh();

        message = "Rotation Phase";
        hudManager.PrintBigNews(message);
        cardsManager.StartRotationAnimations();

        while (!rotationPhaseEnded || hudManager.bigNewsAnimation.isPlaying)
        {
            rotationPhaseEnded = cardsManager.AllRotationAnimationsFinished();
            yield return null;
        }

        if (playerIndex != 3)
            playerIndex++;
        else
            playerIndex = 0;



        playersHUDcontroller.CyclePlayersHUDs(turnCount);
    }

    public void StartFight(PlayerController attacker, PlayerController defender)
    {
        combatManager.StartFightFlow(attacker, defender);
    }

    void InstantiatePlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            switch (players[i].type)
            {
                case PlayerController.Type.hypogeum:
                    Point redStart = gridReference.GetPointFromCoords((int)MyData.startingRedPoint.x, (int)MyData.startingRedPoint.y);
                    players[i].transform.position = redStart.worldPosition + Vector3.up * .5f;
                    players[i].startingWayPoint = redStart;
                    break;
                case PlayerController.Type.underground:
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

    public void ChoseAction(int actionIndex)
    {
        currentActivePlayer.ChoseAction(actionIndex);
    }

    public void SetPlayerToStart(bool flag)
    {
        if (!flag)
        {
            ConfirmAction();
            hudManager.Refresh();
            currentActivePlayer.currentAction = PlayerController.Action.start;
        }
        else
        {
            ConfirmAction();
            hudManager.Refresh();
            currentActivePlayer.currentAction = PlayerController.Action.start;
        }
    }

    public void EndTurn()
    {
        turnEnded = true;
        turnCount++;
    }

    public void UndoMoveCurrentPlayer()
    {
        currentActivePlayer.possibleMoves = currentActivePlayer.beforeMoveActionMoves;
        currentActivePlayer.MoveToPoint(currentActivePlayer.moveStartPoint);
        currentActivePlayer.currentWayPoint = currentActivePlayer.moveStartPoint;
        currentActivePlayer.currentAction = PlayerController.Action.moving;
        currentActivePlayer.VFXobject.SetActive(false);
        hudManager.Refresh();
    }

    public void ConfirmAction()
    {
        switch (currentActivePlayer.currentAction)
        {
            case PlayerController.Action.moving:
                currentActivePlayer.VFXobject.SetActive(false);
                break;
            case PlayerController.Action.buyCard:
                break;
            case PlayerController.Action.placeCard:
                ConfirmPlaceCard();
                break;
            case PlayerController.Action.rotateCard:
                ConfirmRotateCard();
                break;
            case PlayerController.Action.fight:
                foreach (var player in currentActivePlayer.playersToRob)
                {
                    player.outlineController.EnableOutline(false);
                }
                break;
        }

        currentActivePlayer.currentAction = PlayerController.Action.start;
        if (currentActivePlayer.isBonusMove)
        {
            currentActivePlayer.bonusMoveActions--;
            currentActivePlayer.isBonusMove = false;
        }
        else
        {
            currentActivePlayer.actions--;
        }

        hudManager.Refresh();
        playersHUDcontroller.RefreshPlayerUIs();
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
                if (OnMoveSelected != null)
                    OnMoveSelected();
                break;
            case PlayerController.Action.buyCard:
                UndoBuyCards();
                break;
            case PlayerController.Action.placeCard:
                UndoPlaceCard();
                break;
            case PlayerController.Action.rotateCard:
                UndoRotateCard();
                break;
            case PlayerController.Action.fight:
                foreach (var player in currentActivePlayer.playersToRob)
                {
                    player.outlineController.EnableOutline(false);
                }
                break;
        }

        currentActivePlayer.currentAction = PlayerController.Action.start;

        hudManager.Refresh();
        playersHUDcontroller.RefreshPlayerUIs();
    }

    #region Specific Confirm Actions

    //void ConfirmBuyCards()
    //{
    //    List<int> cardsBought = uiManager.cardShopScript.cardsBought;

    //    foreach (int cardType in cardsBought)
    //    {
    //        switch (cardType)
    //        {
    //            case 1:
    //                Transform instantiatedCard1 = Instantiate(prefabCard1, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
    //                CardController card1Controller = instantiatedCard1.GetComponent<CardController>();
    //                if (card1Controller)
    //                {
    //                    currentActivePlayer.cardsInHand.Add(card1Controller);
    //                    card1Controller.player = currentActivePlayer;
    //                    currentActivePlayer.numberOfCards1InHand++;
    //                }
    //                break;
    //            case 2:
    //                Transform instantiatedCard2 = Instantiate(prefabCard2, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
    //                CardController card2Controller = instantiatedCard2.GetComponent<CardController>();
    //                if (card2Controller)
    //                {
    //                    currentActivePlayer.cardsInHand.Add(card2Controller);
    //                    card2Controller.player = currentActivePlayer;
    //                    currentActivePlayer.numberOfCards2InHand++;
    //                }
    //                break;
    //            case 3:
    //                Transform instantiatedCard3 = Instantiate(prefabCard3, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
    //                CardController card3Controller = instantiatedCard3.GetComponent<CardController>();
    //                if (card3Controller)
    //                {
    //                    currentActivePlayer.cardsInHand.Add(card3Controller);
    //                    card3Controller.player = currentActivePlayer;
    //                    currentActivePlayer.numberOfCards1InHand++;
    //                }
    //                break;
    //        }
    //    }

    //    currentActivePlayer.hasBought = false;

    //    uiManager.cardShopScript.cardsBought.Clear();
    //}

    //void ConfirmSellCard()
    //{
    //    currentActivePlayer.cardsInHand.Remove(currentActivePlayer.cardToSell);
    //    Destroy(currentActivePlayer.cardToSell.gameObject);
    //    currentActivePlayer.cardToSell = null;
    //    currentActivePlayer.hasSold = false;
    //    currentActivePlayer.SetNumberOfCardTypesInHand();
    //}

    void ConfirmPlaceCard()
    {
        switch (currentActivePlayer.lastPlacedCard.type)
        {
            case CardController.Type.card1:
                currentActivePlayer.numberOfCards1InHand--;
                break;
            case CardController.Type.card2:
                currentActivePlayer.numberOfCards2InHand--;
                break;
            case CardController.Type.card3:
                currentActivePlayer.numberOfCards3InHand--;
                break;
        }

        mainCamera.SetHighView(false);
    }

    void ConfirmRotateCard()
    {
        currentActivePlayer.selectedCard = null;

        cardsManager.HighlightPlacedCards(false);
        mainCamera.SetHighView(false);
    }

    #endregion

    #region Specific Undo Actions

    void UndoBuyCards()
    {
        if (currentActivePlayer.hasBought)
        {
            currentActivePlayer.energyPoints = currentActivePlayer.beforeActionEnergyPoints;

            currentActivePlayer.hasBought = false;
        }
    }

    //void UndoSellCard()
    //{
    //    currentActivePlayer.cardToSell = null;
    //    currentActivePlayer.energyPoints = currentActivePlayer.beforeActionEnergyPoints;
    //    currentActivePlayer.hasSold = false;
    //    currentActivePlayer.SetNumberOfCardTypesInHand();
    //}

    void UndoPlaceCard()
    {
        if (currentActivePlayer.hasPlacedCard)
        {
            cardsManager.PlacedCards.Remove(currentActivePlayer.lastPlacedCard);
            currentActivePlayer.SendCardInHand(currentActivePlayer.lastPlacedCard);
            currentActivePlayer.hasPlacedCard = false;
            currentActivePlayer.energyPoints = currentActivePlayer.beforeActionEnergyPoints;
            currentActivePlayer.bonusMoveActions = currentActivePlayer.beforeActionBonusMoveActions;
        }
        if (currentActivePlayer.selectedCard)
        {
            currentActivePlayer.UnselectCard();
        }
        mainCamera.SetHighView(false);
    }

    void UndoRotateCard()
    {
        CardController card = currentActivePlayer.selectedCard;
        if (card && !currentActivePlayer.hasPlacedCard)
        {
            card.SetRotationBackToPlaced();
            card.Place(currentActivePlayer.selectedCard.hexImOn);
            card = null;
        }
        else if (currentActivePlayer.hasPlacedCard)
        {
            card = currentActivePlayer.lastPlacedCard;
            card.FreePaths(card.hexImOn);
            card.placedEulerAngle = currentActivePlayer.beforeRotateActionCardEulerAngle;
            card.SetRotationBackToPlaced();
            card.Place(card.hexImOn);
        }
        currentActivePlayer.energyPoints = currentActivePlayer.beforeActionEnergyPoints;
        currentActivePlayer.bonusMoveActions = currentActivePlayer.beforeActionBonusMoveActions;
        currentActivePlayer.selectedCard = null;
        cardsManager.HighlightPlacedCards(false);
        mainCamera.SetHighView(false);
    }

    #endregion

    public void Win(PlayerController player)
    {
        hudManager.Win(player);
    }

    public bool CheckIfPointIsWalkable(Point point)
    {
        foreach (PlayerController player in players)
        {
            if (player != currentActivePlayer && player.currentWayPoint == point)
                return false;
        }

        if (point.type == Point.Type.win && currentActivePlayer.victoryPoints < 5)
            return false;

        return true;
    }

    public List<AgentPosition> FindWalkablePointsInRange(int range, PlayerController player)
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
                if (pointsInRange[i].remainingMoves > 0)
                {
                    if (pointsInRange[i].point.type == Point.Type.purple && player.currentWayPoint != pointsInRange[i].point)
                        continue;
                    foreach (Point point in pointsInRange[i].point.possibleDestinations)
                    {
                        if (!CheckIfPointIsWalkable(point))
                            continue;
                        if (point.isFinalWaypoint && !player.IsMyColor(point))
                            continue;
                        pointsInRange.Add(new AgentPosition(point, pointsInRange[i].remainingMoves - 1));
                    }
                }
                pointsInRange[i].isChecked = true;
            }
        }

        return pointsInRange;
    }

    public List<AgentPosition> ScanPointsInRange(int range, PlayerController player)
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
                if (pointsInRange[i].remainingMoves > 0)
                {
                    foreach (Point point in pointsInRange[i].point.possibleDestinations)
                    {
                        pointsInRange.Add(new AgentPosition(point, pointsInRange[i].remainingMoves - 1));
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

        List<AgentPosition> pointsInRange = ScanPointsInRange(range, player);

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
    public int remainingMoves;
    public bool isChecked;

    public AgentPosition(Point _point, int _moves)
    {
        point = _point;
        remainingMoves = _moves;
        isChecked = false;
    }
}