using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public delegate void UIevent(PlayerController player);

    GameManager gameManager;
    public UIevent UIrefresh;

    public enum Type
    {
        hypogeum, underwater, forest, underground
    }

    public enum Action
    {
        idle, start, moving, buyCard, sellCard, placeCard, rotateCard, bet
    }

    #region Public Variables

    public Type type, weaknessType, strenghtType;
    [HideInInspector]
    public Point startingWayPoint;
    [HideInInspector]
    public Point currentWayPoint, turnStartPoint;
    [HideInInspector]
    public Action currentAction = Action.idle;
    [HideInInspector]
    public Action previousState;
    [HideInInspector]
    public int possibleMoves = 3, energyPoints = 0, victoryPoints = 3, turnStartMoves, actions = 2;
    [HideInInspector]
    public bool hasUsedAbility, hasBet, canBet;
    public Transform[] cards;
    [HideInInspector]
    public CardController selectedCard;
    public List<PlayerController> playersToRob = new List<PlayerController>();
    public LayerMask pointLayer, hexLayer, cardLayer, playerLayer;

    #endregion

    Hexagon lastSelectedHex;
    bool uiRefreshFlag, isFirstStart = true;
    string bottomLeftMsg;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        Transform cardsParent = GameObject.Find(type.ToString() + "PlayerCards").transform;
        cards = new Transform[cardsParent.childCount];
        for (int i = 0; i < cardsParent.transform.childCount; i++)
        {
            cards[i] = cardsParent.GetChild(i);
        }
    }

    private void Start()
    {
        currentWayPoint = startingWayPoint;
    }

    void Update()
    {
        switch (currentAction)
        {
            case Action.idle:
                selectedCard = null;
                break;

            case Action.start:

                #region Start

                if (isFirstStart)
                {
                    possibleMoves = gameManager.NumberOfPossiblesMoves(this);
                    turnStartMoves = possibleMoves;
                    turnStartPoint = currentWayPoint;
                    hasUsedAbility = false;
                    hasBet = false;
                    energyPoints++;
                    energyPoints += cards[1].GetComponent<CardController>().extractableEnergy;
                    if (UIrefresh != null)
                    {
                        UIrefresh(this);
                    }
                    isFirstStart = false;
                }
                else
                {
                    if (!uiRefreshFlag)
                    {
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                        uiRefreshFlag = true;
                    }
                }

                #endregion

                break;

            case Action.moving:

                #region Moving

                if (!uiRefreshFlag)
                {
                    if (UIrefresh != null)
                    {
                        UIrefresh(this);
                        uiRefreshFlag = true;
                    }
                }

                if (possibleMoves <= 0)
                {
                    uiRefreshFlag = false;
                    if (UIrefresh != null)
                    {
                        UIrefresh(this);
                    }
                }

                RaycastHit hitInfo;

                //moving ray
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, pointLayer))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Point pointHit = gameManager.gridReference.GetPointFromWorldPosition(hitInfo.collider.transform.position);

                        if (pointHit != null && currentWayPoint.possibleDestinations.Contains(pointHit.worldPosition) && possibleMoves > 0 && CheckIfPointIsWalkable(pointHit))
                        {

                            transform.position = pointHit.worldPosition + Vector3.up * .7f;
                            currentWayPoint = pointHit;

                            if (currentWayPoint.isFinalWaypoint && isMyColor(currentWayPoint))
                                energyPoints++;

                            if (currentWayPoint.type == Point.Type.purple)
                            {
                                possibleMoves = 0;
                            }
                            else
                            if (currentWayPoint.type == Point.Type.win)
                            {
                                possibleMoves = 0;
                                if (victoryPoints >= 5)
                                {
                                    gameManager.Win(this);
                                }
                            }
                            else
                                possibleMoves--;

                            if (UIrefresh != null)
                            {
                                UIrefresh(this);
                            }

                            CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
                        }

                    }
                }

                #endregion

                break;

            case Action.placeCard:

                #region Place Card

                RaycastHit placingHitInfo;

                //Se ho carta selezionata. E se l'ho selezionata da mano -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromHand && !hasUsedAbility)
                {
                    //(PLACE)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out placingHitInfo, 100, hexLayer))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(placingHitInfo.collider.transform.position);

                            if (hexHit != null)
                            {
                                if (hexHit.card == null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit))
                                {
                                    selectedCard.Place(hexHit);
                                    hasUsedAbility = true;
                                    if (UIrefresh != null)
                                    {
                                        UIrefresh(this);
                                    }
                                }
                            }
                        }
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1) && !hasUsedAbility && selectedCard.state == CardController.State.selectedFromHand)
                    {
                        UnselectCard();
                        uiRefreshFlag = false;
                        currentAction = Action.moving;
                    }
                }

                #endregion

                break;

            case Action.rotateCard:

                #region Rotate Card

                RaycastHit selectingHitInfo;

                //Se non ho carta selezionata. Quindi la seleziono dalla mappa -->
                if (!selectedCard)
                {
                    //(SELECT)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out selectingHitInfo, 100, cardLayer))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedCard = selectingHitInfo.collider.GetComponentInParent<CardController>();

                            if (selectedCard && selectedCard.state == CardController.State.placed && currentWayPoint.nearHexagons.Contains(selectedCard.hexImOn))
                            {
                                selectedCard.state = CardController.State.selectedFromMap;
                                selectedCard.FreePaths(selectedCard.hexImOn);
                                if (UIrefresh != null)
                                {
                                    UIrefresh(this);
                                }
                            }
                        }
                    }
                }

                //Se ho carta selezionata. E se l'ho selezionata dalla mappa -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromMap && !hasUsedAbility)
                {

                    //(PLACE)
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedCard.Place(selectedCard.hexImOn);
                        hasUsedAbility = true;
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1))
                    {
                        selectedCard.SetRotationBackToPlaced();
                        selectedCard.Place(selectedCard.hexImOn);
                        currentAction = Action.moving;
                        selectedCard = null;
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                    }

                    //(RETURN TO OWNER'S HAND)
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        UnselectCard();
                        hasUsedAbility = true;
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                    }

                }

                #endregion

                break;

            case Action.bet:

                #region Bet
                if (!hasBet)
                {
                    RaycastHit betHitInfo;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out betHitInfo, 100, playerLayer))
                    {

                        PlayerController playerHit = betHitInfo.collider.GetComponent<PlayerController>();

                        //Seleziono un player nemico da attaccare
                        if (Input.GetMouseButtonDown(0) && playerHit && playersToRob.Contains(playerHit))
                        {
                            hasBet = true;
                            if (UIrefresh != null)
                            {
                                UIrefresh(this);
                            }
                            StartCoroutine(gameManager.Bet(this, playerHit));
                        }
                    }

                    //UNDO
                    if (Input.GetMouseButtonDown(1))
                    {
                        currentAction = previousState;
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                    }
                }

                #endregion

                break;
        }
    }

    #region Card Selection
    public void SelectCard(int index)
    {
        selectedCard = cards[index].GetComponent<CardController>();
        if (selectedCard.state == CardController.State.inHand)
        {
            selectedCard.state = CardController.State.selectedFromHand;
            currentAction = Action.placeCard;
        }
        if (UIrefresh != null)
        {
            UIrefresh(this);
        }
    }

    public void UnselectCard()
    {
        selectedCard.state = CardController.State.inHand;
        selectedCard.transform.position = MyData.prefabsPosition;
        selectedCard = null;

        if (UIrefresh != null)
        {
            UIrefresh(this);
        }
    }
    #endregion

    bool CheckIfPointIsWalkable(Point point)
    {
        PlayerController[] players = gameManager.players;

        foreach (PlayerController player in players)
        {
            if (player.currentWayPoint == point)
                return false;
        }

        return true;
    }

    public bool isMyColor(Point point)
    {

        if (type == Type.hypogeum && point.type == Point.Type.hypogeum)
            return true;
        if (type == Type.forest && point.type == Point.Type.forest)
            return true;
        if (type == Type.underwater && point.type == Point.Type.underwater)
            return true;
        if (type == Type.underground && point.type == Point.Type.underground)
            return true;

        return false;
    }

    public void MoveToPoint(Point point)
    {
        transform.position = point.worldPosition + Vector3.up * .7f;
    }

    public void ChoseAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0:
                currentAction = Action.moving;
                break;
            case 1:
                currentAction = Action.buyCard;
                break;
            case 2:
                currentAction = Action.sellCard;
                break;
            case 3:
                currentAction = Action.placeCard;
                break;
            case 4:
                currentAction = Action.rotateCard;
                break;
            case 5:
                currentAction = Action.bet;
                break;
        }

        if(UIrefresh != null)
        {
            UIrefresh(this);
        }

    }

}
