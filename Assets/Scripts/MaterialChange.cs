using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

    MeshRenderer myRenderer;
    GameManager gameManager;

    Color originalColor;

	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
        myRenderer = GetComponent<MeshRenderer>();
        originalColor = myRenderer.material.color;
    }

    //private void Update()
    //{
    //    isInRange = false;
    //    foreach (AgentPosition agent in gameManager.currentActivePlayer.pointsInRange)
    //    {
    //        if (transform.position == agent.point.worldPosition)
    //        {
    //            isInRange = true;
    //        }
    //    }

    //    if (isInRange)
    //    {
    //        myRenderer.material.color = Color.black;
    //    }
    //    else
    //    {
    //        myRenderer.material.color = originalColor;
    //    }
    //}

    private void OnMouseEnter()
    {
        if(gameManager.currentActivePlayer.currentWayPoint.worldPosition == transform.position)
        {
            return;
        }else
        if (gameManager.currentActivePlayer.currentWayPoint.possibleDestinations.Contains(transform.position) && gameManager.currentActivePlayer.possibleMoves != 0 && gameManager.currentActivePlayer.currentAction == PlayerController.Action.moving)
        {
            myRenderer.material.color = Color.green;
        }else if(gameManager.currentActivePlayer.currentAction != PlayerController.Action.moving)
        {
            myRenderer.material.color = Color.red;
        }
    }
        
    private void OnMouseExit()
    {
        myRenderer.material.color = originalColor;
    }
}
