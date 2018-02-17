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

    public Transform cardPrefab;

    public LayerMask movingLayer, placingLayer;

    string bottomLeftMsg;

    public enum State
    {
        idle, start, moving, ability
    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
                            if (hexHit.card == null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit))
                            {
                                gameManager.uiManager.currentInstantiatedCard.gameObject.GetComponent<CardController>().Place(hexHit.worldPosition);
                                hexHit.card = gameManager.uiManager.currentInstantiatedCard;
                                hasUsedAbility = true;
                            }
                            else
                            if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && hexHit.card.GetComponent<CardController>().isBeingMod == false)
                            {
                                hexHit.card.GetComponent<CardController>().isBeingMod = true;
                            }
                            else if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && hexHit.card.GetComponent<CardController>().isBeingMod == true)
                            {
                                hexHit.card.GetComponent<CardController>().Place(hexHit.worldPosition);
                                hasUsedAbility = true;
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        Hexagon hexHit = gameManager.gridReference.GetHexagonFromWorldPosition(placingHitInfo.collider.transform.position);

                        if (hexHit.card != null && !hasUsedAbility && currentWayPoint.nearHexagons.Contains(hexHit) && hexHit.card.GetComponent<CardController>().isBeingMod == true)
                        {
                            DestroyImmediate(hexHit.card.gameObject);
                            hasUsedAbility = true;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1) && !hasUsedAbility)
                {
                    DestroyImmediate(gameManager.uiManager.currentInstantiatedCard.gameObject);
                    currentState = State.moving;
                }

                break;

            default:
                break;
        }
    }
}
