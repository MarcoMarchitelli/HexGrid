using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

public class GameManager : MonoBehaviour
{
    public delegate void VoidEvent();
    public delegate void PlayerControllerEvent(PlayerController player);

    public VoidEvent RotationPhase;
    public PlayerControllerEvent TurnEnd;
    public PlayerControllerEvent TurnStart;
    public VoidEvent GainPhase;

    public PlayerController[] players;
    public UIManager uiManager;

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
    public int turnCount = 1;
    public float buttonMashFightResult = .5f;

    int energyBet;
    bool hasBet, fightResultAnnounced, fronteAzionarioFinished = false;
    PlayerController winner;

    public bool isStaticEvent = false;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
        gridReference = FindObjectOfType<HexGridCreator>();
        mainCamera = FindObjectOfType<CameraBehaviour>();
        cardsManager = GetComponent<CardsManager>();
        InstantiatePlayers();
    }

    private void Start()
    {
        players[0].currentAction = PlayerController.Action.start;
        currentActivePlayer = players[0];
        uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);
        if (currentActivePlayer.UIrefresh != null)
        {
            currentActivePlayer.UIrefresh(currentActivePlayer);
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

    public void ChoseAction(int actionIndex)
    {
        currentActivePlayer.ChoseAction(actionIndex);
    }

    #region Button Functions

    public void EndTurn()
    {
        if (TurnEnd != null)
        {
            TurnEnd(currentActivePlayer);
        }
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentActivePlayer)
            {
                uiManager.UnsubscribeToPlayerUIRefreshEvent(currentActivePlayer);
                currentActivePlayer.currentAction = PlayerController.Action.idle;
                if (i != players.Length - 1)
                {
                    currentActivePlayer = players[i + 1];
                    currentActivePlayer.currentAction = PlayerController.Action.start;
                    uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);

                    if (RotationPhase != null)
                        RotationPhase();

                    turnCount++;
                    break;
                }
                else
                {
                    currentActivePlayer = players[0];
                    currentActivePlayer.currentAction = PlayerController.Action.start;
                    uiManager.SubscribeToPlayerUIRefreshEvent(currentActivePlayer);

                    if (RotationPhase != null)
                        RotationPhase();

                    turnCount++;
                    break;
                }
            }
        }
        if (currentActivePlayer.UIrefresh != null)
            currentActivePlayer.UIrefresh(currentActivePlayer);
    }

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

    public void ConfirmAction()
    {
        switch (currentActivePlayer.currentAction)
        {
            case PlayerController.Action.moving:
                //nothing to do
                break;
            case PlayerController.Action.buyCard:
                ConfirmBuyCards();
                break;
            case PlayerController.Action.sellCard:
                ConfirmSellCard();
                break;
            case PlayerController.Action.placeCard:
                //nothing to do
                break;
            case PlayerController.Action.rotateCard:
                break;
            case PlayerController.Action.fight:
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
                UndoBuyCards();
                break;
            case PlayerController.Action.sellCard:
                UndoSellCard();
                break;
            case PlayerController.Action.placeCard:
                UndoPlaceCard();
                break;
            case PlayerController.Action.rotateCard:
                UndoRotateCard();
                break;
            case PlayerController.Action.fight:
                break;
        }

        currentActivePlayer.currentAction = PlayerController.Action.start;

        if (currentActivePlayer.UIrefresh != null)
            currentActivePlayer.UIrefresh(GameManager.instance.currentActivePlayer);
    }

    #endregion

    #region Specific Confirm Actions

    void ConfirmBuyCards()
    {
        List<int> cardsBought = uiManager.cardShopScript.cardsBought;

        foreach (int cardType in cardsBought)
        {
            switch (cardType)
            {
                case 1:
                    Transform instantiatedCard1 = Instantiate(prefabCard1, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
                    CardController card1Controller = instantiatedCard1.GetComponent<CardController>();
                    if (card1Controller)
                    {
                        currentActivePlayer.cardsInHand.Add(card1Controller);
                        card1Controller.player = currentActivePlayer;
                        currentActivePlayer.numberOfCards1InHand++;
                    }
                    break;
                case 2:
                    Transform instantiatedCard2 = Instantiate(prefabCard2, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
                    CardController card2Controller = instantiatedCard2.GetComponent<CardController>();
                    if (card2Controller)
                    {
                        currentActivePlayer.cardsInHand.Add(card2Controller);
                        card2Controller.player = currentActivePlayer;
                        currentActivePlayer.numberOfCards2InHand++;
                    }
                    break;
                case 3:
                    Transform instantiatedCard3 = Instantiate(prefabCard3, MyData.prefabsPosition, Quaternion.Euler(Vector3.up * -90));
                    CardController card3Controller = instantiatedCard3.GetComponent<CardController>();
                    if (card3Controller)
                    {
                        currentActivePlayer.cardsInHand.Add(card3Controller);
                        card3Controller.player = currentActivePlayer;
                        currentActivePlayer.numberOfCards1InHand++;
                    }
                    break;
            }
        }

        currentActivePlayer.hasBought = false;

        uiManager.cardShopScript.cardsBought.Clear();
    }

    void ConfirmSellCard()
    {
        currentActivePlayer.cardsInHand.Remove(currentActivePlayer.cardToSell);
        Destroy(currentActivePlayer.cardToSell.gameObject);
        currentActivePlayer.cardToSell = null;
        currentActivePlayer.hasSold = false;
        currentActivePlayer.SetNumberOfCardTypesInHand();
    }

    void ConfirmPlaceCard()
    {
        switch (currentActivePlayer.lastPlacedCard.type)
        {
            case CardController.Type.card1:
                currentActivePlayer.numberOfCards1InHand++;
                break;
            case CardController.Type.card2:
                currentActivePlayer.numberOfCards2InHand++;
                break;
            case CardController.Type.card3:
                currentActivePlayer.numberOfCards3InHand++;
                break;
        }
    }

    void ConfirmRotateCard()
    {
        currentActivePlayer.selectedCard = null;
    }

    #endregion

    #region Specific Undo Actions

    void UndoBuyCards()
    {
        if (currentActivePlayer.hasBought)
        {
            currentActivePlayer.energyPoints = currentActivePlayer.beforeActionEnergyPoints;

            currentActivePlayer.hasBought = false;

            uiManager.cardShopScript.cardsBought.Clear();
        }
    }

    void UndoSellCard()
    {
        currentActivePlayer.cardToSell = null;
        currentActivePlayer.energyPoints = currentActivePlayer.beforeActionEnergyPoints;
        currentActivePlayer.hasSold = false;
        currentActivePlayer.SetNumberOfCardTypesInHand();
    }

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
    }

    #endregion

    public void Win(PlayerController player)
    {
        uiManager.Win(player);
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