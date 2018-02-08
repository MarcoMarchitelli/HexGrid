using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public PlayerController playerReference;
    public HexGridCreator gridReference;

	void Awake () {
        playerReference = FindObjectOfType<PlayerController>();
        gridReference = FindObjectOfType<HexGridCreator>();
	}

    private void Start()
    {
        InstantiatePlayer();
    }

    void InstantiatePlayer()
    {
        GameObject instantiatedPlayer = Instantiate(playerReference.gameObject, gridReference.WaypointGrid[0].worldPosition + Vector3.up * .5f, Quaternion.identity);
        instantiatedPlayer.GetComponent<PlayerController>().enabled = true;
    }
}
