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

    public Transform cardPrefab;

    public LayerMask movingLayer, placingLayer;

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

        if (currentState == State.start)
        {
            possibleMoves = gameManager.NumberOfPossiblesMoves(this);
            currentState = State.moving;
        }

        if (currentState == State.moving)
        {
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
                            transform.position = pointHit.worldPosition + Vector3.up * .5f;
                            currentWayPoint = pointHit;
                            possibleMoves--;
                            if (possibleMoves <= 0)
                            {
                                currentState = State.ability;
                            }
                            CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentState = State.idle;
        }
    }
}
