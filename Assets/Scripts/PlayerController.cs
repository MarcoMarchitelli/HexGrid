using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    GameManager gameManager;
    public Point currentWayPoint;

    //public HexGridCreator hexGrid;
    public LayerMask targetLayer;

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
            if (Input.GetMouseButtonDown(0))
            {
                Point pointHit = GetPointFromWorldPosition(hitInfo.collider.transform.position);

                if (pointHit != null)
                {
                    if (gameManager.gridReference.GetPossibleDestinationsFromPoint(currentWayPoint).Contains(pointHit))
                    {
                        transform.position = pointHit.worldPosition + Vector3.up * .5f;
                        currentWayPoint = pointHit;
                    }
                }
            }
        }
    }

    public Point GetPointFromWorldPosition(Vector3 worldPosition)
    {
        for (int i = 0; i < gameManager.gridReference.WaypointGrid.Count; i++)
        {
            if (Mathf.Approximately(worldPosition.x, gameManager.gridReference.WaypointGrid[i].worldPosition.x) && Mathf.Approximately(worldPosition.z, gameManager.gridReference.WaypointGrid[i].worldPosition.z))
            {
                return gameManager.gridReference.WaypointGrid[i];
            }
        }
        return null;
    }
}
