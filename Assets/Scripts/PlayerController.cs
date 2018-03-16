using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    GameManager gameManager;

    #region Public Variables

    public string name;
    [HideInInspector]
    public Point startingWayPoint;
    public Point currentWayPoint, turnStartPoint;
    public State currentState = State.idle;
    [HideInInspector]
    public State previousState;
    [HideInInspector]
    public int possibleMoves = 3, energyPoints = 0, victoryPoints = 3, turnStartMoves;
    [HideInInspector]
    public bool hasUsedAbility, hasBet;
    public Transform[] cards;
    [HideInInspector]
    public CardController selectedCard;
    public List<PlayerController> playersToRob = new List<PlayerController>();
    public LayerMask pointLayer, hexLayer, cardLayer, playerLayer;

    #endregion

    Hexagon lastSelectedHex;
    string bottomLeftMsg;

    public enum State
    {
        idle, start, moving, card, bet
    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        Transform cardsParent = GameObject.Find(name + "PlayerCards").transform;
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
        switch (currentState)
        {
            case State.idle:
                selectedCard = null;
                break;

            case State.start:

                #region Start
                possibleMoves = gameManager.NumberOfPossiblesMoves(this);
                turnStartMoves = possibleMoves;
                turnStartPoint = currentWayPoint;
                hasUsedAbility = false;
                hasBet = false;
                energyPoints++;
                energyPoints += cards[1].GetComponent<CardController>().extractableEnergy;
                gameManager.uiManager.ToggleBet(this);
                currentState = State.moving;
                #endregion

                break;

            case State.moving:

                #region Moving
                gameManager.uiManager.ToggleUndoMoves(this);

                bottomLeftMsg = "You have " + possibleMoves + " moves remaining!";
                gameManager.uiManager.PrintLeft(bottomLeftMsg);

                if (possibleMoves <= 0)
                {
                    currentState = State.card;
                }

                
                RaycastHit hitInfo;

                //moving ray
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, pointLayer))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Point pointHit = gameManager.gridReference.GetPointFromWorldPosition(hitInfo.collider.transform.position);

                        if (pointHit != null)
                        {
                            if (currentWayPoint.possibleDestinations.Contains(pointHit.worldPosition))
                            {
                                if (possibleMoves > 0)
                                {
                                    transform.parent.position = pointHit.worldPosition + Vector3.up * .7f;
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

                                    gameManager.uiManager.ToggleBet(this);
                                    gameManager.uiManager.ToggleUndoMoves(this);
                                }
                                CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
                            }
                        }
                    }
                }

                RaycastHit cardHitInfo;

                //select card from map and go to ability
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out cardHitInfo, 100, cardLayer))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedCard = cardHitInfo.collider.GetComponentInParent<CardController>();

                        if (selectedCard && selectedCard.state == CardController.State.placed && currentWayPoint.nearHexagons.Contains(selectedCard.hexImOn))
                        {
                            gameManager.uiManager.ToggleUndoMoves(this);
                            selectedCard.state = CardController.State.selectedFromMap;
                            currentState = State.card;
                            selectedCard.FreePaths(selectedCard.hexImOn);
                        }
                    }
                }
                #endregion

                break;

            case State.card:

                #region Card
                if (!hasUsedAbility && selectedCard && selectedCard.state == CardController.State.selectedFromHand)
                {
                    bottomLeftMsg = "Use A/D to rotate the card. \nLeftclick to place it. \nRightclick to undo.";
                }
                else
                if (!hasUsedAbility && selectedCard && selectedCard.state == CardController.State.selectedFromMap)
                {
                    bottomLeftMsg = "Use A/D to rotate the card.\nLeftclick to place it.\nSpace to return it to it's owner's hand.\nRightclick to undo.";
                }
                else
                if (!hasUsedAbility && !selectedCard)
                {
                    bottomLeftMsg = "Select a card from your hand,\nor from the map.";
                }
                else
                {
                    bottomLeftMsg = "You've ended your actions for this turn. \nLet the other players have fun too!";
                }

                gameManager.uiManager.PrintLeft(bottomLeftMsg);

                RaycastHit placingHitInfo;

                //Se ho carta selezionata. E se l'ho selezionata da mano -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromHand && !hasUsedAbility)
                {
                    gameManager.uiManager.ToggleUndoMoves(this);
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
                                }
                            }
                        }
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1) && !hasUsedAbility && selectedCard.state == CardController.State.selectedFromHand)
                    {
                        UnselectCard();
                        if (possibleMoves != 0)
                            currentState = State.moving;
                    }
                }

                //Se ho carta selezionata. E se l'ho selezionata dalla mappa -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromMap && !hasUsedAbility)
                {
                    gameManager.uiManager.ToggleUndoMoves(this);
                    //(PLACE)
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedCard.Place(selectedCard.hexImOn);
                        hasUsedAbility = true;
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1))
                    {
                        selectedCard.SetRotationBackToPlaced();
                        selectedCard.Place(selectedCard.hexImOn);
                        if (possibleMoves != 0)
                            currentState = State.moving;
                        selectedCard = null;
                    }

                    //(RETURN TO OWNER'S HAND)
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        UnselectCard();
                        hasUsedAbility = true;
                    }
                }

                RaycastHit selectingHitInfo;

                //Se non ho carta selezionata. Quindi l'ho selezionata dalla mappa -->
                if (!selectedCard)
                {
                    //(SELECT)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out selectingHitInfo, 100, cardLayer))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            gameManager.uiManager.ToggleUndoMoves(this);
                            selectedCard = selectingHitInfo.collider.GetComponentInParent<CardController>();
                            selectedCard.state = CardController.State.selectedFromMap;
                        }
                    }
                }
                #endregion

                break;

            case State.bet:

                #region Bet
                if (!hasBet)
                {
                    RaycastHit betHitInfo;

                    gameManager.uiManager.PrintLeft("Select a player to attack.\nRightclick to undo.");

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out betHitInfo, 100, playerLayer))
                    {

                        PlayerController playerHit = betHitInfo.collider.GetComponent<PlayerController>();

                        //Seleziono un player nemico da attaccare
                        if (Input.GetMouseButtonDown(0) && playerHit && playersToRob.Contains(playerHit))
                        {
                            StartCoroutine(gameManager.Bet(this, playerHit));
                        }
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        currentState = previousState;
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
            currentState = State.card;
        }
    }

    public void UnselectCard()
    {
        selectedCard.state = CardController.State.inHand;
        selectedCard.transform.position = MyData.prefabsPosition;
        selectedCard = null;
    }
    #endregion

    public bool isMyColor(Point point)
    {

        if (name == "yellow" && point.type == Point.Type.yellow)
            return true;
        if (name == "green" && point.type == Point.Type.green)
            return true;
        if (name == "blue" && point.type == Point.Type.blue)
            return true;
        if (name == "red" && point.type == Point.Type.red)
            return true;

        return false;
    }

    public void MoveToPoint(Point point)
    {
        transform.parent.position = point.worldPosition + Vector3.up * .7f;
    }
}
