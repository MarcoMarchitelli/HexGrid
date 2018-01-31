using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    GameManager gameManager;
    Point currentWayPoint;

    public HexGridCreator hexGrid;
    public LayerMask targetLayer;

    public List<Point> test = new List<Point>();

    int testIndex = 1;

	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
	}

    private void Start()
    {
        currentWayPoint = gameManager.gridReference.WaypointGrid[0];
    }

    void Update () {

        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, targetLayer))
        {
            hitInfo.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

            if (Input.GetMouseButtonDown(0))
            {
                Point pointHit = hitInfo.collider.gameObject.GetComponent<Point>();
                test = gameManager.gridReference.GetPossibleDestinationsFromPoint(currentWayPoint);

                if (pointHit != null)
                {
                    if (test.Contains(pointHit))
                    {
                        transform.position = pointHit.worldPosition;
                        currentWayPoint = pointHit;
                    }
                }
            }
        }
    }

    void Move()
    {
        transform.position = hexGrid.WaypointGrid[testIndex].worldPosition;
        testIndex++;
    }
}
