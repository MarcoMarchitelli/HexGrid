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
    public int possibleMoves = 3, energyPoints = 0, victoryPoints = 3;
    [HideInInspector]
    public bool hasUsedAbility;

    public Transform[] cards;
    CardController selectedCard;
    Hexagon lastSelectedHex;

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
                possibleMoves = gameManager.NumberOfPossiblesMoves(this);
                currentState = State.moving;
                hasUsedAbility = false;
                energyPoints++;
                break;

            case State.moving:

                bottomLeftMsg = "You have " + possibleMoves + " moves remaining!";
                gameManager.uiManager.PrintLeft(bottomLeftMsg);

                if (possibleMoves <= 0)
                {
                    currentState = State.ability;
                }

                RaycastHit hitInfo;

                //moving ray
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
                                    transform.parent.position = pointHit.worldPosition + Vector3.up * .7f;
                                    currentWayPoint = pointHit;
                                    possibleMoves--;
                                }
                                CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
                            }
                        }
                    }
                }

                RaycastHit modHitInfo;

                //modifying ray
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out modHitInfo, 100, placingLayer))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(modHitInfo.collider.transform.position);

                        if (hexHit != null)
                        {
                            //Select card from map
                            if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && hexHit.card.GetComponent<CardController>().state == CardController.State.placed)
                            {
                                selectedCard = hexHit.card.GetComponent<CardController>();
                                selectedCard.state = CardController.State.selectedFromMap;
                                selectedCard.FreePaths(hexHit);
                                lastSelectedHex = hexHit;
                            }
                            //Replace card from map
                            else if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && selectedCard.state == CardController.State.selectedFromMap)
                            {
                                selectedCard.Place(hexHit);
                                hasUsedAbility = true;
                                currentState = State.ability;
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(modHitInfo.collider.transform.position);

                        //The card that i selected from the map. Remove it by rightclicking
                        if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && selectedCard.state == CardController.State.selectedFromMap)
                        {
                            UnselectCard();
                            hasUsedAbility = true;
                            hexHit.card = null;
                            currentState = State.ability;
                        }
                    }
                }

                //if(Input.GetMouseButtonDown(1) && modifyingCard && !hasUsedAbility)
                //{
                //    UnselectCard();
                //    hasUsedAbility = true;
                //    currentState = State.ability;
                //}
                break;

            case State.ability:

                bottomLeftMsg = "Use A/D to rotate the card. Leftclick to place it!";
                gameManager.uiManager.PrintLeft(bottomLeftMsg);

                RaycastHit placingHitInfo;

                //if i leftclick on an Hexagon...
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
                                selectedCard.Place(hexHit);
                                hexHit.card = selectedCard.transform;
                                hasUsedAbility = true;
                            }
                            else
                            //I select a card already placed on an Hexagon
                            if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && hexHit.card.GetComponent<CardController>().state == CardController.State.placed)
                            {
                                lastSelectedHex = hexHit;
                                selectedCard = lastSelectedHex.card.GetComponent<CardController>();
                                selectedCard.state = CardController.State.selectedFromMap;
                                selectedCard.FreePaths(lastSelectedHex);
                            }
                            //Place the card selected from the map
                            else if (lastSelectedHex == hexHit && !hasUsedAbility && selectedCard.state == CardController.State.selectedFromMap)
                            {
                                selectedCard.Place(lastSelectedHex);
                                hasUsedAbility = true;
                                lastSelectedHex = null;
                            }
                        }
                    }
                    //if (Input.GetMouseButtonDown(1))
                    //{
                    //    Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(placingHitInfo.collider.transform.position);

                    //    //The card that i selected from the map. Unselect it by rightclicking
                    //    if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && selectedCard.state == CardController.State.selectedFromMap)
                    //    {
                    //        UnselectCard();
                    //        hasUsedAbility = true;
                    //        hexHit.card = null;
                    //    }
                    //}
                }

                if (Input.GetMouseButtonDown(1) && !hasUsedAbility && selectedCard.state == CardController.State.selectedFromHand)
                {
                    UnselectCard();
                    currentState = State.moving;
                }

                if (Input.GetMouseButtonDown(1) && !hasUsedAbility && selectedCard.state == CardController.State.selectedFromMap)
                {
                    UnselectCard();
                    hasUsedAbility = true;
                    lastSelectedHex.card = null;
                }

                break;

            default:
                break;
        }
    }

    public void SelectCard(int index)
    {
        selectedCard = cards[index].GetComponent<CardController>();
        if (selectedCard.state == CardController.State.inHand)
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
