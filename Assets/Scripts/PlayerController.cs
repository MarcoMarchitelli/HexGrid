using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    GameManager gameManager;

    public string name;
    [HideInInspector]
    public Point startingWayPoint;
    public Point currentWayPoint;
    public State currentState = State.idle;
    [HideInInspector]
    public int possibleMoves = 3;
    [HideInInspector]
    public bool hasUsedAbility;

    public Transform[] cards;
    CardController selectedCard;

    public LayerMask movingLayer, placingLayer;

    string bottomLeftMsg;

    public enum State
    {
        idle, start, moving, ability
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
                break;

            case State.start:
                gameManager.uiManager.DisplayHand(this);
                possibleMoves = gameManager.NumberOfPossiblesMoves(this);
                currentState = State.moving;
                hasUsedAbility = false;
                break;

            case State.moving:

                bottomLeftMsg = "You have " + possibleMoves + " moves remaining!";
                gameManager.uiManager.PrintLeft(bottomLeftMsg);

                if (possibleMoves <= 0)
                {
                    currentState = State.ability;
                }

                RaycastHit hitInfo;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, movingLayer))
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
                                    transform.position = pointHit.worldPosition + Vector3.up * .5f;
                                    currentWayPoint = pointHit;
                                    possibleMoves--;
                                }
                                CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
                            }
                        }
                    }
                }

                break;

            case State.ability:

                bottomLeftMsg = "Use A/D to rotate the card. Leftclick to place it!";
                gameManager.uiManager.PrintLeft(bottomLeftMsg);

                RaycastHit placingHitInfo;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out placingHitInfo, 100, placingLayer))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(placingHitInfo.collider.transform.position);

                        if (hexHit != null)
                        {
                            //Already have card selected. Leftclick to place
                            if (hexHit.card == null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit))
                            {
                                selectedCard.Place(hexHit.worldPosition);
                                hexHit.card = selectedCard.transform;
                                hasUsedAbility = true;
                            }
                            else
                            //I select a card already placed on an Hexagon with leftclick
                            if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && hexHit.card.GetComponent<CardController>().state == CardController.State.placed)
                            {
                                selectedCard = hexHit.card.GetComponent<CardController>();
                                selectedCard.state = CardController.State.selectedFromMap;
                            }
                            //The card that i selected from the map. Place it with leftclick
                            else if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && selectedCard.state == CardController.State.selectedFromMap)
                            {
                                selectedCard.Place(hexHit.worldPosition);
                                hasUsedAbility = true;
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(placingHitInfo.collider.transform.position);

                        //The card that i selected from the map. Unselect it by rightclicking
                        if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && selectedCard.state == CardController.State.selectedFromMap)
                        {
                            UnselectCard();
                            hasUsedAbility = true;
                            hexHit.card = null;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1) && !hasUsedAbility)
                {
                    UnselectCard();
                    currentState = State.moving;
                }

                break;

            default:
                break;
        }
    }

    public void SelectCard(int index)
    {
        selectedCard = cards[index].GetComponent<CardController>();
        if(selectedCard.state == CardController.State.inHand)
        {
            selectedCard.state = CardController.State.selectedFromHand;
            currentState = State.ability;
        }    
    }

    public void UnselectCard()
    {
        selectedCard.state = CardController.State.inHand;
        selectedCard.transform.position = MyData.prefabsPosition;
        selectedCard = null;
    }
}
