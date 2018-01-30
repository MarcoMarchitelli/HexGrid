using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    GameManager gameManager;
    Vector3 currentWayPoint;

    public HexGridCreator hexGrid;

    int testIndex = 1;

	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
	}

    private void Start()
    {
        currentWayPoint = transform.position;
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Move();
        }
	}

    void Move()
    {
        transform.position = hexGrid.Waypoints[testIndex];
        testIndex++;
    }
}
